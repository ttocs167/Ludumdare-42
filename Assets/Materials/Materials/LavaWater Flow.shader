#warning Upgrade NOTE: unity_Scale shader variable was removed; replaced 'unity_Scale.w' with '1.0'
// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


 
Shader "WaterTestShader1"
{
    Properties
    {
        //color
        _Color ("Main Color", Color) = (1,1,1,0.5)
       
        //main water texture
        _AmbientTex ("Water Base Texture", 2D) = "white" {}
       
        //first main normal map to scroll
        _NrmTex1 ("First scrolling normal map (RGB)", 2D) = "bump" {}
       
        //second main normal map to scroll
        _NrmTex2 ("Second scrolling normal map", 2D) = "bump" {}
       
        //flowmap
        _FlowMap   ("FlowMap  (RGB)", 2D) = "white" {}
       
        //noise map
         _NoiseMap  ("NoiseMap (RGB)", 2D) = "white" {}
       
        //colour textures overlay
        _ColorTextureOverlay ("_ColorTextureOverlay", Range (0.0, 1.0)) = 0.75
       
        //fresnel power for reflections
        _FresnelPower ("_FresnelPower", Range (0.1, 10.0)) = 2.0
       
        //ambient power light
        _Ambient ("_Ambient", Range (0.0, 1.0)) = 0.8
       
        //world light direction
        _WorldLightDir("_WorldLightDir", Vector) = (0,0,0,1)
       
        //specular shiness
        _Shininess ("_Shininess", Range (0.1, 60.0)) = 1.0
       
        //spec colours
        _SpecColor ("Spec Color", Color) = (0.5,0.5,0.5,0.5)
       
        _WaveScale ("Wave scale", Range (0.02,0.15)) = .07
       
       // _WaveSpeed ("Wave speed (map1 x,y; map2 x,y)", Vector) = (19,9,-16,-7)
    }
   
    Category
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        //ColorMask RGB
        //Lighting off ZWrite Off
       
    SubShader
    {
   
    Pass
    {
 
    CGPROGRAM
    #pragma target 3.0
    #pragma vertex vert
    #pragma fragment frag
    #include "UnityCG.cginc"
 
    //main colour
    float4 _Color;
   
    //texsampler
    sampler2D _NrmTex1, _NrmTex2, _FlowMap, _NoiseMap, _AmbientTex;
   
    //texture's st
    float4 _NrmTex1_ST;
    float4 _NrmTex2_ST;
    float4 _AmbientTex_ST;
    float4 _FlowMap_ST;
   
    //wave speed
    float3 fWaveSpeed;
   
    //wave scale
    float  _WaveScale;
   
    //Flow map offsets used to scroll the wave maps
    float   flowMapOffset0;
    float   flowMapOffset1;
 
    //scale used on the wave maps
    float fWaveScale = 1.0f;
   
    //the half cycle needed to blend the 2 normal maps when one reaches the top the other's contribuition should be 0
    float halfCycle;
   
    //color texture overlay
    float _ColorTextureOverlay;
   
    //reflection power
    float _FresnelPower;
   
    //ambient cols
    float _Ambient;
   
    //spec shiness
    float _Shininess;
   
    //spec colour
    float4 _SpecColor;
   
    //world light direction used to calculate the fresnel law
    float4 _WorldLightDir;
 
//vertex structure from vertex shader to pixel
struct v2f
{
    float4  pos         : SV_POSITION;
    float2  uv0         : TEXCOORD0;
    float2  uv1         : TEXCOORD1;
    float3 viewDirWorld : TEXCOORD2;
    float3 TtoW0        : TEXCOORD3;
    float3 TtoW1        : TEXCOORD4;
    float3 TtoW2        : TEXCOORD5;
    float3  col         : COLOR0;
    float2 uv2          : TEXCOORD6;
};
 
v2f vert (appdata_full v)
{
    //declaration of the evrtex structure
    v2f o;
   
    //determine the position in world space applying the model view projection transform
    o.pos = UnityObjectToClipPos (v.vertex);
   
    //unpack texture coords for the scrolling normal maps (they are the same so there is no need to waste memory for another float2 texcoord variable)
    o.uv0 = TRANSFORM_TEX (v.texcoord, _NrmTex1);
   
    //unpack texture coord for the flow map
    o.uv1 = TRANSFORM_TEX (v.texcoord, _FlowMap);
   
    //unpack tex coords for ambient water base texture
    o.uv2 = TRANSFORM_TEX (v.texcoord, _AmbientTex);
   
    //reverse view dir                                     
    o.viewDirWorld = -WorldSpaceViewDir(v.vertex);
       
    //calculates the tangent normals for the fresnel law                                                   
    TANGENT_SPACE_ROTATION;
    o.TtoW0 = mul(rotation, unity_ObjectToWorld[0].xyz * 1.0);
    o.TtoW1 = mul(rotation, unity_ObjectToWorld[1].xyz * 1.0);
    o.TtoW2 = mul(rotation, unity_ObjectToWorld[2].xyz * 1.0);     
     
    //returns the output structure
    return o;
}
 
half4 frag (v2f i) : COLOR
{
    //get and uncompress the flow vector for this pixel
    float2 flowmap = tex2D( _FlowMap, i.uv1 ).rg * 2.0f - 1.0f;
   
    //determines the noise clycle offset from the noise mask
    float cycleOffset = tex2D( _NoiseMap, i.uv1 ).r;
   
    //determines the phase0
    float phase0 = cycleOffset * .5f + flowMapOffset0;
   
    //determines the phase1
    float phase1 = cycleOffset * .5f + flowMapOffset1;
   
    // Sample normal map1
    float3 normalT0 = UnpackNormal(tex2D(_NrmTex1, ( i.uv0 * fWaveScale ) + flowmap * phase0 ));
   
    //Sample normal map2
    float3 normalT1 = UnpackNormal(tex2D(_NrmTex2, ( i.uv0 * fWaveScale ) + flowmap * phase1 ));
   
    //determines the flow function
    float f = ( abs( halfCycle - flowMapOffset0 ) / halfCycle );
   
    //unroll the normals retrieved from the normalmaps
    normalT0.yz = normalT0.zy; 
    normalT1.yz = normalT1.zy;
   
    normalT0 = 2.0f * normalT0 - 1.0f;
    normalT1 = 2.0f * normalT1 - 1.0f;
   
    //determins the resulting normal
    float3 normalT = lerp( normalT0, normalT1, f );
   
    // declare world normal
    half3 worldNormal;
 
    //calculate the world normals
    worldNormal.x = dot(i.TtoW0, normalT.xyz);
    worldNormal.y = dot(i.TtoW1, normalT.xyz);
    worldNormal.z = dot(i.TtoW2, normalT.xyz);     
                   
    // normalize
    worldNormal = normalize(worldNormal);
    i.viewDirWorld = normalize(i.viewDirWorld);
   
    // color
    float4 color = tex2D(_AmbientTex, i.uv2);
    color = lerp(half4(0.6, 0.6, 0.6, 0.6), color, _ColorTextureOverlay);  
   
    // REFLECTION
    float3 reflectVector = normalize(reflect(i.viewDirWorld, worldNormal));
    half4 reflColor = 0.75;
   
    // FRESNEL CALCS
    float fcbias = 0.20373;
    float facing = saturate(1.0 - max(dot(-i.viewDirWorld, worldNormal), 0.0));
    float refl2Refr = max(fcbias + (1.0 - fcbias) * pow(facing, _FresnelPower), 0);
   
    color.rgba *= (lerp(half4(0.6,0.6,0.6, 0.6), half4(reflColor.rgb,1.0), refl2Refr));
                     
    // light
    color.rgb = color.rgb * max(_Ambient, saturate(dot(_WorldLightDir.xyz, worldNormal)));
   
    // a little more spec in low quality to have at least something going on
    color.rgb += _SpecColor.rgb * 2.0 *  pow(saturate(dot(_WorldLightDir.xyz, reflectVector)), _Shininess);
   
    //returns the light color computer modulated by the diffuse color      
    return _Color * color;
           
}
ENDCG
 
}
 
 
    }
}
Fallback ""//"VertexLit"
} 
