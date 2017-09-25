string description = "Basic Vertex Lighting with a Texture";

//------------------------------------
float4x4 WorldViewProj : WorldViewProjection;
float4x4 World   : World;
float4x4 WorldInverseTranspose : WorldInverseTranspose;
float4x4 ViewInverse : ViewInverse;
float texstep = 1.0/512.0;

texture Tex0 : Diffuse
<
	string ResourceName = "default_color.dds";
>;

float contrast
<
    string UIWidget = "slider";
    float UIMin = 0.0;
    float UIMax = 3.0;
    float UIStep = 0.1;
    string UIName = "Contrast Stretch";
> = 1.0;

float brightness
<
    string UIWidget = "slider";
    float UIMin = -1.0;
    float UIMax = 1.0;
    float UIStep = 0.1;
    string UIName = "Brightness Stretch";
> = 0.0;


//------------------------------------
struct VS_OUTPUT
{
    float4 pos  :   POSITION;
    float4 color    :   COLOR;
    float2 texCoord :   TEXCOORD0;
};

VS_OUTPUT VS(
    float4 Pos  :   POSITION,
    float4 Norm : NORMAL,
    float2 texCoord :   TEXCOORD0)
{
    VS_OUTPUT Out = (VS_OUTPUT)0;
    
    // transform Position
    Out.pos = mul(Pos, WorldViewProj);
    Out.color = float4(0,0,0,1.0);
    Out.texCoord = texCoord;
    
    return Out;
}


//------------------------------------
sampler TextureSampler = sampler_state 
{
    texture = <Tex0>;
    AddressU  = CLAMP;        
    AddressV  = CLAMP;
    AddressW  = CLAMP;
    MIPFILTER = LINEAR;
    MINFILTER = LINEAR;
    MAGFILTER = LINEAR;
};


//-----------------------------------
float4 PS(
	float2 Tex : TEXCOORD0) : COLOR
{
  /*
  const float3x3 sharpkernel = float3x3 (-1, -1, -1,
									-1, 9, -1,
									-1, -1, -1);
  const float3x3 blurkernel = float3x3 (0.1111, 0.1111, 0.1111,
									0.1111, 0.1111, 0.1111,
									0.1111, 0.1111, 0.1111);
  */									
  float4 dT1 = tex2D( TextureSampler, Tex);
  //float4 dT2 = tex2D( TextureSampler, IN.texCoordDiffuse );
  float3 sum = 0.0;
  
  sum += tex2D ( TextureSampler , float2 ( Tex [0]+texstep , Tex[1])) * -1;//0.1111;
  sum += tex2D ( TextureSampler , float2 ( Tex [0]+texstep , Tex [1]+texstep)) * -1;//0.1111;
  sum += tex2D ( TextureSampler , float2 ( Tex [0]+texstep , Tex [1] -texstep)) * -1;//0.1111;
  sum += tex2D ( TextureSampler , float2 ( Tex [0] -texstep , Tex [1])) * -1;//0.1111;
  sum += tex2D ( TextureSampler , float2 ( Tex [0] -texstep , Tex [1]+texstep)) * -1;//0.1111;
  sum += tex2D ( TextureSampler , float2 ( Tex [0] -texstep , Tex [1] -texstep)) * -1;//0.1111;
  sum += tex2D ( TextureSampler , float2 ( Tex [0] , Tex [1]+texstep)) * -1;//0.1111;
  sum += tex2D ( TextureSampler , float2 ( Tex [0] , Tex [1] -texstep)) * -1;//0.1111;
  sum += tex2D ( TextureSampler , float2 ( Tex [0] , Tex [1])) * 9;//0.1111;
  return float4(sum,dT1.a);
  //return dT1;
}


//-----------------------------------
technique textured
{
    pass p0 
    {		
		VertexShader = compile vs_1_1 VS();
		PixelShader  = compile ps_2_0 PS();
    }
}