﻿Shader "DeferredShading/PostEffect/BloomCombine" {

Properties {
    g_intensity ("Intensity", Float) = 0.3
}
SubShader {
    Tags { "RenderType"="Opaque" }
    Blend Off
    ZTest Always
    ZWrite Off
    Cull Back

CGINCLUDE
#include "Compat.cginc"

sampler2D g_frame_buffer;
sampler2D g_half_frame_buffer;
sampler2D g_quarter_frame_buffer;
float4 g_intensity;


struct ia_out
{
    float4 vertex : POSITION;
};

struct vs_out
{
    float4 vertex : SV_POSITION;
    float4 screen_pos : TEXCOORD0;
};

struct ps_out
{
    float4 color : COLOR0;
};


vs_out vert (ia_out v)
{
    vs_out o;
    o.vertex = v.vertex;
    o.screen_pos = v.vertex;
    return o;
}

ps_out frag (vs_out i)
{
    float2 coord = (i.screen_pos.xy / i.screen_pos.w + 1.0) * 0.5;
    // see: http://docs.unity3d.com/Manual/SL-PlatformDifferences.html
    #if UNITY_UV_STARTS_AT_TOP
        coord.y = 1.0-coord.y;
    #endif

    float4 c = tex2D(g_frame_buffer, coord);
    c += (tex2D(g_half_frame_buffer, coord) + tex2D(g_quarter_frame_buffer, coord)) * g_intensity;
    c.w = 1.0;

    ps_out r = {c};
    return r;
}
ENDCG

    Pass {
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma target 3.0
        #ifdef SHADER_API_OPENGL 
            #pragma glsl
        #endif
        ENDCG
    }
}
}
