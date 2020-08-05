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
    Filter = Point;
    AddressU = Wrap;
    AddressV = Wrap;
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
    output.WorldPosition = offsetPos +  float4(16, 16, 0, 0);
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

    float light = sampleLight(x, y);
    // return float4(light, light, light, 1.0f);

    //return float4(input.TextureCoordinates.x, input.TextureCoordinates.y, 0, 1);

    // return float4(x /16.0f, y /16.0f, 0, 1);

    // if (sampleSolid(x, y)) {
    //     return float4(0,0,0,1);
    // }
    // return float4(1, 1, 1, 1);

    // Borders.
    //Above.
    // if (!sampleSolid(x, y - 1)) {
    //     if (maskAbove == 0 && texY < transitionBand){
    //         return float4(0,0,0,1);
    //     }  
    // }
    // // Below.
    // if (!sampleSolid(x, y + 1)) {
    //     if (maskBelow){
    //         return float4(0,0,0,1);
    //     }  
    // }
    // // Right.
    // if (!sampleSolid(x - 1, y)) {
    //     if (maskRight){
    //         return float4(0,0,0,1);
    //     }   
    // }
    // // Left.
    // if (!sampleSolid(x + 1, y)) {
    //     if (maskLeft){
    //         return float4(0,0,0,1);
    //     }   
    // }


    // if (!sampleSolid(x + 1, y + 1)[0]) {
    //     if (input.TextureCoordinates[0] > 1 - borderWidth && input.TextureCoordinates[1] > 1 - borderWidth){
    //         return float4(0,0,0,1);
    //     }  
    // }
    // if (!sampleSolid(x - 1, y - 1)[0]) {
    //     if (input.TextureCoordinates[0] < borderWidth && input.TextureCoordinates[1] < borderWidth){
    //         return float4(0,0,0,1);
    //     }  
    // }
    // if (!sampleSolid(x + 1, y - 1)[0]) {
    //     if (input.TextureCoordinates[0] > 1 - borderWidth && input.TextureCoordinates[1] < borderWidth){
    //         return float4(0,0,0,1);
    //     }  
    // }
    // if (!sampleSolid(x - 1, y + 1)[0]) {
    //     if (input.TextureCoordinates[0] < borderWidth && input.TextureCoordinates[1] > 1 - borderWidth){
    //         return float4(0,0,0,1);
    //     }  
    // }

    // Light level
    // float light = sampleLight(x, y);
    float lightAbove = sampleLight(x, y - 1);
    float lightBelow = sampleLight(x, y + 1);
    float lightRight = sampleLight(x + 1, y);
    float lightLeft = sampleLight(x - 1, y);

    // float2 lightDirection = float2(
    //     lerp(lightLeft, lightRight, input.TextureCoordinates.x),
    //     lerp(lightAbove, lightBelow, input.TextureCoordinates.y)
    // );

    // float2 texOffset = ((input.TextureCoordinates * 64 - 20) % 32) / 32;
    float2 texOffset = ((input.TextureCoordinates * 48 - 8));
    if (texOffset.x < 0) {
        texOffset.x = 48 + 16 + texOffset.x;
    }
    if (texOffset.y < 0) {
        texOffset.y = 48 + 16 + texOffset.y;
    }
    texOffset = (texOffset % 32) / 32;

    float2 lightDirection = float2(
        lerp(lightLeft, lightRight, texOffset.x),
        lerp(lightAbove, lightBelow, texOffset.y)
    );
    float lightTotal = (lightDirection.x + lightDirection.y) / 2.0f;  
    return float4(sprite.xyz * lightTotal,  1.0f);


    return float4(texOffset.xy * lightTotal, 0,  1.0f);
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

//     int x = abs(floor((worldPos[0] - ChunkX * ChunkSize * GridSize) / ChunkSize));
//     int y = abs(floor((worldPos[1] - ChunkY * ChunkSize * GridSize) / ChunkSize));

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