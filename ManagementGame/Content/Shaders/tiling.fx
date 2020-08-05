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
int TileSize;
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


float4 sampleSolid(int x, int y) {
    if (x < 0 || y < 0 || x >= ChunkSize || y >= ChunkSize) {
        return float4(1, 1, 1, 1);
    }
    return tex2D(SolidTileSampler, float2(x / (float)ChunkSize, y / (float)ChunkSize));
}

float sampleLight(int x, int y) {
    x++;
    y++;
    if (x < 0 || y < 0 || x >= ChunkSize + 2 || y >= ChunkSize + 2) {
        return 1.0f;
    }
    return tex2D(LightMapSampler, float2(x,y) / (LightMapSize))[0];
}

VertexShaderOutput MainVS(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 viewPosition = mul(input.Position, ViewMatrix);
    output.Position = mul(viewPosition, ProjectionMatrix);
    output.WorldPosition = input.Position + float4(TileSize, TileSize, 0, 0) / 2.0f;
    output.Color = input.Color;
    output.TextureCoordinates = input.TextureCoordinates;
    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float factor = 1.0f;
    int relativeX = input.WorldPosition[0] - (ChunkX * ChunkSize * TileSize);
    int relativeY = input.WorldPosition[1] - (ChunkY * ChunkSize * TileSize);
    int x = floor(relativeX / ChunkSize) % ChunkSize;
    int y = floor(relativeY / ChunkSize) % ChunkSize;

    float4 sprite = tex2D(SpriteSampler, input.TextureCoordinates);
    float4 isSolid = tex2D(SolidTileSampler, float2(x / (float)ChunkSize, y / (float)ChunkSize));

    float borderWidth = 1.0f / TileSize;

    // if (isSolid[0]) {
    //     return float4(0,0,0,1);
    // }
    // return float4(1,1,1,1);


    if (!sampleSolid(x, y - 1)[0]) {
        if (input.TextureCoordinates[1] < borderWidth){
            return float4(0,0,0,1);
        }  
    }
    if (!sampleSolid(x, y + 1)[0]) {
        if (input.TextureCoordinates[1] > 1 - borderWidth){
            return float4(0,0,0,1);
        }  
    }
    if (!sampleSolid(x - 1, y)[0]) {
        if (input.TextureCoordinates[0] < borderWidth){
            return float4(0,0,0,1);
        }  
    }
    if (!sampleSolid(x + 1, y)[0]) {
        if (input.TextureCoordinates[0] > 1 - borderWidth){
            return float4(0,0,0,1);
        }  
    }


    if (!sampleSolid(x + 1, y + 1)[0]) {
        if (input.TextureCoordinates[0] > 1 - borderWidth && input.TextureCoordinates[1] > 1 - borderWidth){
            return float4(0,0,0,1);
        }  
    }
    if (!sampleSolid(x - 1, y - 1)[0]) {
        if (input.TextureCoordinates[0] < borderWidth && input.TextureCoordinates[1] < borderWidth){
            return float4(0,0,0,1);
        }  
    }
    if (!sampleSolid(x + 1, y - 1)[0]) {
        if (input.TextureCoordinates[0] > 1 - borderWidth && input.TextureCoordinates[1] < borderWidth){
            return float4(0,0,0,1);
        }  
    }
    if (!sampleSolid(x - 1, y + 1)[0]) {
        if (input.TextureCoordinates[0] < borderWidth && input.TextureCoordinates[1] > 1 - borderWidth){
            return float4(0,0,0,1);
        }  
    }

    // Light level
    float light = sampleLight(x, y);
    float lightAbove = sampleLight(x, y - 1);
    float lightBelow = sampleLight(x, y + 1);
    float lightRight = sampleLight(x + 1, y);
    float lightLeft = sampleLight(x - 1, y);

    float2 lightDirection = float2(
        lerp(lightLeft, lightRight, input.TextureCoordinates[0]),
        lerp(lightAbove, lightBelow, input.TextureCoordinates[1])
    );

    float lightTotal = (lightDirection.x + lightDirection.y) / 2.0f;  

    return float4(sprite.xyz * lightTotal, 1.0f);
}




// float4 MainPS(ShaderIO input) : COLOR
// {
//     return SpriteTexture1.Sample(SampleType, input.TextureCoordinates);
    
//     float4 color1;
//     float4 color2;
//     float4 blendColor;

//     float borderWidth = .05f;
//     float factor = 16.0f;

//     float4 worldPos = mul(float4(input.Position[0], input.Position[1], 0, 0), WorldMatrix);

//     int x = abs(floor((worldPos[0] - ChunkX * ChunkSize * TileSize) / ChunkSize));
//     int y = abs(floor((worldPos[1] - ChunkY * ChunkSize * TileSize) / ChunkSize));

//     if (sampleSolid(x, y - 1)[0]) {
//         return float4(x / factor, y / factor,0,1);     
//     }
//     return SpriteTexture1.Sample(SampleType, input.TextureCoordinates);

//     // if (input.tex[0] > 1 - borderWidth)
//     // {
//     //     if (!sampleSolid(x + 1, y)[0]) {
//     //         return float4(0,0,0,1);
//     //     }
//     // }
//     // if (input.tex[1] > 1 - borderWidth) {
//     //     if (!sampleSolid(x, y + 1)[0]) {
//     //         return float4(0,0,0,1);
//     //     }
//     // }
//     // if (input.tex[0] < borderWidth) {
//     //     if (!sampleSolid(x - 1, y)[0]) {
//     //         return float4(0,0,0,1);
//     //     }
//     // }
//     // if (input.tex[1] < borderWidth) 
//     // {
//     //     if (!sampleSolid(x, y - 1)[0]) {
//     //         return float4(0,0,0,1);
//     //     }
//     // }

//     // return SpriteTexture1.Sample(SampleType, input.tex);


//     // if (input.tex[0] > .5f) {
//     //     blendColor = SpriteTexture1.Sample(SampleType, input.tex);
//     // } else {
//     //     blendColor = SpriteTexture2.Sample(SampleType, input.tex);
//     // }

//     // Get the pixel color from the first texture.
//     // color1 = SpriteTexture1.Sample(SampleType, input.tex);

//     // // Get the pixel color from the second texture.
//     // color2 = SpriteTexture2.Sample(SampleType, input.tex);

//     // // Blend the two pixels together and multiply by the gamma value.
//     // blendColor = color1 * color2 * 2.0;
    
//     // // Saturate the final color.
//     // blendColor = saturate(blendColor);

//     return blendColor;
// }

technique SpriteDrawing
{
	pass P0
	{
        VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};