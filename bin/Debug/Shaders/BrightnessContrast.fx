string description = "Basic Vertex Lighting with a Texture";

//------------------------------------
float4x4 WorldViewProj : WorldViewProjection;
float4x4 World   : World;
float4x4 WorldInverseTranspose : WorldInverseTranspose;
float4x4 ViewInverse : ViewInverse;
const float PI = 3.14159265358979323846;

texture Tex0 : Diffuse
<
	string ResourceName = "default_color.dds";
>;

float contrast
<
    string UIWidget = "slider";
    float UIMin = 0.0;
    float UIMax = 2.0;
    float UIStep = 0.1;
    string UIName = "Contrast Stretch";
> = 1.25;

float brightness
<
    string UIWidget = "slider";
    float UIMin = -1.0;
    float UIMax = 1.0;
    float UIStep = 0.1;
    string UIName = "Brightness Stretch";
> = 0.25;


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
//This method is too complex for a single pass
float3 RGB2HSL(float3 rgb : COLOR) : COLOR
{
 	
	float minval = min(min(rgb.r, rgb.g), rgb.b);
    float maxval = max(max(rgb.r, rgb.g), rgb.b);
    float delta = maxval - minval;
    float3 hsl;
    
    hsl.z = (maxval + minval) / 2.0;
    if (maxval == minval)
    {
    	hsl.x = 0.0;
    	hsl.y = 0.0;
    }
    else
    {
    	hsl.y = delta / ((hsl.z <= 0.5) ? (maxval + minval) : (2.0 - maxval - minval));
    	if (maxval == rgb.r)
    	{
    	    hsl.x = (60.0 / 360.0) * (rgb.g - rgb.b) / (maxval - minval);
    	    if (rgb.g < rgb.b)
    		hsl.x += 360.0 / 360.0;
    	}
    	else if (maxval == rgb.g)
    	{
    	    hsl.x = (60.0 / 360.0) * (rgb.b - rgb.r) / (maxval - minval) + (120.0 / 360.0);
    	}
    	else
    	{
    	    hsl.x = (60.0 / 360.0) * (rgb.r - rgb.g) / (maxval - minval) + (240.0 / 360.0);
    	}
     }
     return hsl;
}


float helper(float tmp2, float tmp1, float H)
{
	float ret;
   
	if (H < 0.0)
		H += 1.0;
	else if (H > 1.0)
    	H -= 1.0;
   
	if (H < 1.0 / 6.0)
   		ret = (tmp2 + (tmp1 - tmp2) * (360.0 / 60.0) * H);
    else if (H < 1.0 / 2.0)
		ret = tmp1;
    else if (H < 2.0 / 3.0)
		ret = (tmp2 + (tmp1 - tmp2) * ((2.0 / 3.0) - H) * (360.0 / 60.0));
   	else
		ret = tmp2;
	return ret;
}

float3 HSL2RGB(float3 hsl : COLOR) : COLOR
{
	float3 rgb;
    float tmp1, tmp2;
    
	if (hsl.z < 0.5)
		tmp1 = hsl.z * (1.0 + hsl.y);
	else
		tmp1 = (hsl.z + hsl.y) - (hsl.z * hsl.y);
	tmp2 = 2.0 * hsl.z - tmp1;
	rgb.r = helper(tmp2, tmp1, hsl.x + (1.0 / 3.0));
	rgb.g = helper(tmp2, tmp1, hsl.x);
	rgb.b = helper(tmp2, tmp1, hsl.x - (1.0 / 3.0));
	
	return rgb;
}

//Need to implement simple colour space conversion
//in single pass
float4 PS(
	float2 Tex : TEXCOORD0) : COLOR
{
  float4 dT1 = tex2D( TextureSampler, Tex);
  /*
  float3 hsl = RGB2HSL(dT1.rgb);
  hsl.z = (hsl.z-0.5)*contrast+0.5+brightness;
  float3 rgb = HSL2RGB(hsl);
  */
  dT1.rgb = (dT1.rgb-0.5)*contrast+0.5+brightness;
  return dT1;
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