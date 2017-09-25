/*********************************************************************NVMH3****
File:  $Id: //sw/devtools/FXComposer/1.8/SDK/MEDIA/HLSL/metalRefl.fx#1 $

Copyright NVIDIA Corporation 2002-2004
TO THE MAXIMUM EXTENT PERMITTED BY APPLICABLE LAW, THIS SOFTWARE IS PROVIDED
*AS IS* AND NVIDIA AND ITS SUPPLIERS DISCLAIM ALL WARRANTIES, EITHER EXPRESS
OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, IMPLIED WARRANTIES OF MERCHANTABILITY
AND FITNESS FOR A PARTICULAR PURPOSE.  IN NO EVENT SHALL NVIDIA OR ITS SUPPLIERS
BE LIABLE FOR ANY SPECIAL, INCIDENTAL, INDIRECT, OR CONSEQUENTIAL DAMAGES
WHATSOEVER (INCLUDING, WITHOUT LIMITATION, DAMAGES FOR LOSS OF BUSINESS PROFITS,
BUSINESS INTERRUPTION, LOSS OF BUSINESS INFORMATION, OR ANY OTHER PECUNIARY LOSS)
ARISING OUT OF THE USE OF OR INABILITY TO USE THIS SOFTWARE, EVEN IF NVIDIA HAS
BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.


Comments:
    A phong-shaded metal-style surface + mirror term.
    Textured or untextured surfaces.
    Metallic reflections have no appreciable Fresnel term.

******************************************************************************/

float Script : STANDARDSGLOBAL <
    string UIWidget = "none";
    string ScriptClass = "object";
    string ScriptOrder = "standard";
    string ScriptOutput = "color";
    string Script = "Technique=Technique?Untextured:Textured;";
> = 0.8;

/************* UN-TWEAKABLES **************/

half4x4 WorldIT : WorldInverseTranspose < string UIWidget="None"; >;
half4x4 WorldViewProj : WorldViewProjection < string UIWidget="None"; >;
half4x4 World : World < string UIWidget="None"; >;
half4x4 ViewI : ViewInverse < string UIWidget="None"; >;

/************* TWEAKABLES **************/

half3 LightPos : Position <
    string Object = "PointLight";
    string Space = "World";
> = {100.0f, 100.0f, -100.0f};

half3 LightColor <
    string UIName =  "Light Color";
    string Object = "PointLight";
    string UIWidget = "Color";
> = {1.0f, 1.0f, 1.0f};

half3 AmbiColor : Ambient <
    string UIName =  "Ambient Light Color";
    string UIWidget = "Color";
> = {0.07f, 0.07f, 0.07f};

half3 SurfColor : DIFFUSE <
    string UIName =  "Surface Color";
    string UIWidget = "Color";
> = {1.0f, 1.0f, 1.0f};

half SpecExpon : SpecularPower <
    string UIWidget = "slider";
    half UIMin = 1.0;
    half UIMax = 128.0;
    half UIStep = 1.0;
    string UIName =  "Specular Power";
> = 12.0;

half Kd <
    string UIWidget = "slider";
    half UIMin = 0.0;
    half UIMax = 1.0;
    half UIStep = 0.05;
    string UIName =  "Diffuse (from dirt)";
> = 0.1;

half Kr <
    string UIWidget = "slider";
    half UIMin = 0.0;
    half UIMax = 1.0;
    half UIStep = 0.05;
    string UIName =  "Reflection";
> = 0.4;

//////////

texture ColorTexture : DIFFUSE <
    string ResourceName = "default_color.dds";
    string ResourceType = "2D";
>;

sampler2D ColorSampler = sampler_state {
	Texture = <ColorTexture>;
	MinFilter = Linear;
	MagFilter = Linear;
	MipFilter = Linear;
};

texture CubeEnvMap : ENVIRONMENT <
	string ResourceName = "default_reflection.dds";
	string ResourceType = "Cube";
>;

samplerCUBE EnvSampler = sampler_state {
	Texture = <CubeEnvMap>;
	MinFilter = Linear;
	MagFilter = Linear;
	MipFilter = Linear;
};

/************* DATA STRUCTS **************/

/* data from application vertex buffer */
struct appdata {
    half3 Position	: POSITION;
    half4 UV		: TEXCOORD0;
    half4 Normal	: NORMAL;
};

/* data passed from vertex shader to pixel shader */
struct vertexOutput {
    half4 HPosition	: POSITION;
    half4 TexCoord		: TEXCOORD0;
    half3 LightVec		: TEXCOORD1;
    half3 WorldNormal	: TEXCOORD2;
    half3 WorldView	: TEXCOORD5;
};

/*********** vertex shader ******/

vertexOutput mainVS(appdata IN) {
    vertexOutput OUT;
    half4 normal = normalize(IN.Normal);
    OUT.WorldNormal = mul(normal, WorldIT).xyz;
    half4 Po = half4(IN.Position.xyz,1);
    half3 Pw = mul(Po, World).xyz;
    OUT.LightVec = normalize(LightPos - Pw);
    OUT.TexCoord = IN.UV;
    OUT.WorldView = normalize(ViewI[3].xyz - Pw);
    OUT.HPosition = mul(Po, WorldViewProj);
    return OUT;
}

/********* pixel shader ********/

void metal_refl_shared(vertexOutput IN,
		out half3 DiffuseContrib,
		out half3 SpecularContrib,
		out half3 ReflectionContrib)
{
    half3 Ln = normalize(IN.LightVec);
    half3 Nn = normalize(IN.WorldNormal);
    half3 Vn = normalize(IN.WorldView);
    half3 Hn = normalize(Vn + Ln);
    half4 litV = lit(dot(Ln,Nn),dot(Hn,Nn),SpecExpon);
    DiffuseContrib = litV.y * Kd * LightColor + AmbiColor;
    SpecularContrib = litV.z * LightColor;
    half3 reflVect = -reflect(Vn,Nn);
    ReflectionContrib = Kr * texCUBE(EnvSampler,reflVect).xyz;
}

half4 mainPS(vertexOutput IN) : COLOR {
    half3 diffContrib;
    half3 specContrib;
    half3 reflColor;
	metal_refl_shared(IN,diffContrib,specContrib,reflColor);
    half3 result = diffContrib +
				(SurfColor * (specContrib + reflColor));
    return half4(result,1);
}

half4 mainPS_t(vertexOutput IN) : COLOR {
    half3 diffContrib;
    half3 specContrib;
    half3 reflColor;
	metal_refl_shared(IN,diffContrib,specContrib,reflColor);
    half3 map = tex2D(ColorSampler,IN.TexCoord.xy).xyz;
    half3 result = diffContrib +
				(SurfColor * map * (specContrib + reflColor));
    return half4(result,1);
}

/*************/

technique Untextured <
	string Script = "Pass=p0;";
> {
	pass p0 <
	string Script = "Draw=geometry;";
> {		
        VertexShader = compile vs_2_0 mainVS();
		ZEnable = true;
		ZWriteEnable = true;
		CullMode = None;
        PixelShader = compile ps_2_0 mainPS();
	}
}

technique Textured <
	string Script = "Pass=p0;";
> {
	pass p0 <
	string Script = "Draw=geometry;";
> {		
        VertexShader = compile vs_2_0 mainVS();
		ZEnable = true;
		ZWriteEnable = true;
		CullMode = None;
        PixelShader = compile ps_2_0 mainPS_t();
	}
}

/***************************** eof ***/
