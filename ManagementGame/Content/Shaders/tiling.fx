#if OPENGL
    #define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture2;
SamplerState SampleType;
int ChunkSize;
int ChunkX;
int ChunkY;
int LightMapSize;
float4x4 WorldMatrix;
float4x4 ViewMatrix;
float4x4 ProjectionMatrix;

const static float MaskTextureSize = 576.0f;
const static float MaskOuterSize = 80.0f;
const static float MaskInnerSize = 64.0f;

const static float GridSize = 64.0f;
const static float TileDrawSize = 80.0f;
const static float TileTextureOffset = (TileDrawSize - GridSize) / 2;

const static float2 HorizontalLeftOffset = float2(80, 0);
const static float2 HorizontalMiddleOffset = float2(160, 0);
const static float2 HorizontalRightOffset = float2(240, 0);

const static float2 VerticalTopOffset = float2(TileDrawSize * 0, TileDrawSize * 1);
const static float2 TopLeftOffset = float2(TileDrawSize * 1, TileDrawSize * 1);
const static float2 TopMiddleOffset = float2(TileDrawSize * 2, TileDrawSize * 1);
const static float2 TopRightOffset = float2(TileDrawSize * 3, TileDrawSize * 1);

const static float2 VerticalMiddleOffset = float2(TileDrawSize * 0, TileDrawSize * 2);
const static float2 MiddleLeftOffset = float2(TileDrawSize * 1, TileDrawSize * 2);
const static float2 AloneOffset = float2(TileDrawSize * 2, TileDrawSize * 2);
const static float2 MiddleRightOffset = float2(TileDrawSize * 3, TileDrawSize * 2);

const static float2 VerticalBottomOffset = float2(TileDrawSize * 0, TileDrawSize * 3);
const static float2 BottomLeftOffset = float2(TileDrawSize * 1, TileDrawSize * 3);
const static float2 BottomMiddleOffset = float2(TileDrawSize * 2, TileDrawSize * 3);
const static float2 BottomRightOffset = float2(TileDrawSize * 3, TileDrawSize * 3);


sampler SpriteSampler : register(s0);
// Texture2D SpriteTexture;
// sampler2D SpriteSampler = sampler_state
// {
// 	Texture = <SpriteTexture>;
// };

Texture2D SolidTileTexture;
sampler2D SolidTileSampler = sampler_state
{
	Texture = <SolidTileTexture>;
    Filter = Point;
    AddressU = Wrap;
    AddressV = Wrap;
};

Texture2D LightMap;
sampler2D LightMapSampler = sampler_state
{
	Texture = <LightMap>;
    Filter = Anisotropic;
    AddressU = Clamp;
    AddressV = Clamp;
};

Texture2D Mask;
sampler2D MaskSampler = sampler_state
{
	Texture = <Mask>;
    Filter = Point;
    AddressU = Wrap;
    AddressV = Wrap;
};

//////////////
// TYPEDEFS //
//////////////

struct VertexShaderInput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
    float4 WorldPosition: TEXCOORD1;
};

float sampleSolid(int x, int y) {
    return tex2D(SolidTileSampler, float2(x / (float)ChunkSize, y / (float)ChunkSize)).r > 0;
}

// Determine solidity of tiles around.
float2 GetMaskOffset(int x, int y) {
    bool above = sampleSolid(x, y - 1);
    bool below = sampleSolid(x, y + 1);
    bool right = sampleSolid(x + 1, y);
    bool left = sampleSolid(x - 1, y);

    if (above && right && below && left) {
        return float2(0,0);
    }
    if (!above && right && !below && !left) {
        return HorizontalLeftOffset;
    }
    if (!above && right && !below && left) {
        return HorizontalMiddleOffset;
    }
    if (!above && !right && !below && left) {
        return HorizontalRightOffset;
    }
    if (!above && !right && below && !left) {
        return VerticalTopOffset;
    }
    if (!above && right && below && !left) {
        return TopLeftOffset;
    }
    if (!above && right && below && left) {
        return TopMiddleOffset;
    }
    if (!above && !right && below && left) {
        return TopRightOffset;
    }
    if (above && !right && below && !left) {
        return VerticalMiddleOffset;
    }
    if (above && right && below && !left) {
        return MiddleLeftOffset;
    }
    if (!above && !right && !below && !left) {
        return AloneOffset;
    }
    if (above && !right && below && left) {
        return MiddleRightOffset;
    }
    if (above && !right && !below && !left) {
        return VerticalBottomOffset;
    }
    if (above && right && !below && !left) {
        return BottomLeftOffset;
    }
    if (above && right && !below && left) {
        return BottomMiddleOffset;
    }
    if (above && !right && !below && left) {
        return BottomRightOffset;
    }

    return float2(0,0);
}

float sampleLight(int x, int y) {
    x++;
    y++;
    return tex2D(LightMapSampler, float2(x,y) / (LightMapSize))[0];
}

VertexShaderOutput MainVS(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 offsetPos = input.Position + float4(0, 0, 0, 0);

    float4 viewPosition = mul(offsetPos, ViewMatrix);
    output.Position = mul(viewPosition, ProjectionMatrix);
    output.WorldPosition = offsetPos + float4(TileDrawSize, TileDrawSize, 0, 0) / 2;
    output.Color = input.Color;
    output.TextureCoordinates = input.TextureCoordinates;
    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    // Move every coordinate as if it was the chunk at 0,0.
    float4 relativeCoord = input.WorldPosition - (float4(ChunkX, ChunkY, 0, 0) * ChunkSize * GridSize);
    // Get tile x, y this pixel belongs to.
    float2 pixelOffset = (input.TextureCoordinates - float2(.5f, .5f)) * TileDrawSize;
    float2 tileIndex = floor((relativeCoord.xy - pixelOffset) / GridSize).xy;

    // return float4(tileIndex.xy / ChunkSize, 0, 1);

    bool isSolid = sampleSolid(tileIndex.x, tileIndex.y);
   //return isSolid ? float4(1,1,1,1) : float4(0,0,0,1);

    // Color of the actual sprite.
    float4 sprite = tex2D(SpriteSampler, input.TextureCoordinates);

    // Get pixel coordinates in textures.
    float2 texturePixelCoords = input.TextureCoordinates * TileDrawSize;
    // Translate and scale it so that it samples from the correct mask.
    float2 maskPixelCoords = (texturePixelCoords + GetMaskOffset(tileIndex.x, tileIndex.y)) / MaskTextureSize;

    float4 mask = tex2D(MaskSampler, maskPixelCoords);
    return float4(sprite.rgb * (1 - mask.r), 1.0) * (1 - mask.g);


    float2 lightMapCoord = relativeCoord.xy + float2(1, 1) / GridSize / LightMapSize;
    float offset = .02f;
    float light = tex2D(LightMapSampler, lightMapCoord)[0];
    float light1 = tex2D(LightMapSampler, lightMapCoord + float2(0, offset))[0];
    float light2 = tex2D(LightMapSampler, lightMapCoord + float2(0, -offset))[0];
    float light3 = tex2D(LightMapSampler, lightMapCoord + float2(offset, 0))[0];
    float light4 = tex2D(LightMapSampler, lightMapCoord + float2(-offset, 0))[0];

    light = (light + light1 + light2 + light3 + light4) / 5;

    return float4(sprite.xyz * light,  1.0f);   
}



technique SpriteDrawing
{
	pass P0
	{
        VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};