struct VS_OUTPUT
{
    float4 pos  :   POSITION;
    float4 color    :   COLOR;
    float2 texCoord :   TEXCOORD0;
};

texture Tex0
<
	string ResourceName = "default_color.dds";
>;
texture Tex1
<
	string ResourceName = "default_color.dds";
>;
float4x4 WorldViewProj  :   WORLDVIEWPROJECTION;
float Opacity = 1.0;
float Brightness = 0.0;

VS_OUTPUT VS(
    float4 Pos  :   POSITION,
    float4 Norm : NORMAL,
    float2 texCoord :   TEXCOORD0)
{
    VS_OUTPUT Out = (VS_OUTPUT)0;
    
    // transform Position
    Out.pos = mul(Pos, WorldViewProj);
    Out.color = float4(0,0,0,Opacity);
    Out.texCoord = texCoord;
    
    return Out;
}

sampler Sampler = sampler_state
{
    Texture = (Tex0);
    MipFilter = LINEAR;
    MinFilter = LINEAR;
    MagFilter = LINEAR;
};

sampler Sampler1 = sampler_state
{
    Texture = (Tex1);
    MipFilter = LINEAR;
    MinFilter = LINEAR;
    MagFilter = LINEAR;
};


float4 PS(
    float2 Tex : TEXCOORD0) : COLOR
{
    float4 f1 = tex2D(Sampler, Tex);
    float4 f2 = tex2D(Sampler1, Tex);
    float4 f = f1 + 0.6 * f2;
    f.w=f1.w;
    return f;
}

technique RenderGrayscale
{
    pass P0
    {
        VertexShader = compile vs_2_0 VS();
        PixelShader = compile ps_2_0 PS();
    }
}
