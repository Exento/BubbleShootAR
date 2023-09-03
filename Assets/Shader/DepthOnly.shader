Shader "Custom/DepthOnly"
{
    SubShader
    {
        Tags { "Queue"="Geometry" }
        
        Pass
        {
            ZTest LEqual
            ZWrite On
            ColorMask 0
        }
    }
}