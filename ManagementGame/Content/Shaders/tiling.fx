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
int GridSize;
int ChunkX;
int ChunkY;
int LightMapSize;
float4x4 WorldMatrix;
float4x4 ViewMatrix;
float4x4 ProjectionMatrix;

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

float sampleMask(int x, int y) {
    return tex2D(MaskSampler, float2(x, y) / 64.0f).x;
}

bool sampleSolid(int x, int y) {
    // if (x < 0 || y < 0 || x >= ChunkSize || y >= ChunkSize) {
    //     return 1.0f;
    // }
    return tex2D(SolidTileSampler, float2(x / (float)ChunkSize, y / (float)ChunkSize)).x > 0;
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
    output.WorldPosition = offsetPos +  float4(32, 32, 0, 0);
    output.Color = input.Color;
    output.TextureCoordinates = input.TextureCoordinates;
    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float factor = 1.0f;
    // Move every coordinate as if it was the chunk at 0,0.
    int relativeX = input.WorldPosition[0] - (ChunkX * ChunkSize * GridSize);
    int relativeY = input.WorldPosition[1] - (ChunkY * ChunkSize * GridSize);
    // Get tile x, y this belongs to.
    int x = floor(relativeX / GridSize);
    int y = floor(relativeY / GridSize);

    float4 sprite = tex2D(SpriteSampler, input.TextureCoordinates);

    float4 mask = tex2D(MaskSampler, input.TextureCoordinates);

    // The original texture is 48 pixels, but only powers of 2 are allowed, so it is 64.
    // Get pixel coordinates in textures.
    int texX = input.TextureCoordinates.x * 64;
    int texY = input.TextureCoordinates.y * 64;
    int borderWidth = 2;
    int transitionBand = 15 + borderWidth;

    float maskAbove = sampleMask(texX, texY - borderWidth);
    float maskBelow = sampleMask(texX, texY + borderWidth);
    float maskRight = sampleMask(texX - borderWidth, texY);
    float maskLeft = sampleMask(texX + borderWidth, texY);


    if (mask.x == 0)
        return float4(0,0,0,0);

    float2 lightMapCoord = float2(relativeX + 1, relativeY + 1) / GridSize / LightMapSize;
    float offset = .02f;
    // float light = sampleLight(x, y);    
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