Shader "Custom/URP/Particle_ZombieBall"
{
    Properties
    {
        _MainTex ("Texture Sheet", 2D) = "white" {}
        _Position ("Position", Vector) = (0, 0, 0, 0)
        _Radius ("Radius", Float) = 1.0
        _TilesX ("TilesX", Float) = 1
        _TilesY ("TilesY", Float) = 1
        _LightingStrength("LightingStrength", Float) = 1
        _Cull("CullOn",Float) = 0

        _TopBeginFrame ("TopBeginFrame", Vector) = (0, 0, 0, 0)
        _TopEndFrame ("TopEndFrame", Vector) = (0, 0, 0, 0)

        _BottomBeginFrame ("BottomBeginFrame", Vector) = (0, 0, 0, 0)
        _BottomEndFrame ("BottomEndFrame", Vector) = (0, 0, 0, 0)

        _RightBeginFrame ("RightBeginFrame", Vector) = (0, 0, 0, 0)
        _RightEndFrame ("RightEndFrame", Vector) = (0, 0, 0, 0)

        _LeftBeginFrame ("LeftBeginFrame", Vector) = (0, 0, 0, 0)
        _LeftEndFrame ("LeftEndFrame", Vector) = (0, 0, 0, 0)

        _MidBeginFrame ("MidBeginFrame", Vector) = (0, 0, 0, 0)
        _MidEndFrame ("MidEndFrame", Vector) = (0, 0, 0, 0)
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        ZTest LEqual

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            // Structs
            struct appdata
            {
                float3 position : POSITION;
                float3 normal   : NORMAL;
                float4 color    : COLOR;
                float4 uv       : TEXCOORD0;
                float random    : TEXCOORD1;
            };

            struct v2f
            {
                float4 vertex    : SV_POSITION;
                float4 uv        : TEXCOORD0;
                float4 color     : COLOR;
                float beginFrame : TEXCOORD2;
                float3 position  : TEXCOORD3;
                float range      : TEXCOORD4;
                float3 normal : NORMAL;
            };

            // Uniforms
            sampler2D _MainTex;
            float4 _Position;
            float _Radius;
            float _TilesX;
            float _TilesY;
            float _LightingStrength;

            float4 _TopBeginFrame, _TopEndFrame;
            float4 _LeftBeginFrame, _LeftEndFrame;
            float4 _RightBeginFrame, _RightEndFrame;
            float4 _BottomBeginFrame, _BottomEndFrame;
            float4 _MidBeginFrame, _MidEndFrame;

            float _Cull;

            int GetRegionIndex(float4 uv)
            {
                float2 relPos = uv.zw - _Position.xy;
                float dist = length(relPos);

                if (dist <= _Radius * 0.5)
                    return 4;

                float absX = abs(relPos.x);
                float absY = abs(relPos.y);

                return (absY >= absX) ? (relPos.y > 0 ? 0 : 1) : (relPos.x > 0 ? 3 : 2);
            }

            float GetRandomValue(float4 f, int random)
            {
                if (random == 0) return f.x;
                if (random == 1) return f.y;
                if (random == 2) return f.z;
                if (random == 3) return f.w;
                return f.x; // fallback
            }

            v2f GetBeginFrameAndRange(v2f v, int random)
            {
                float4 beginFrame = float4(0, 0, 0, 0);
                float4 endFrame   = float4(0, 0, 0, 0);

                switch (GetRegionIndex(v.uv))
                {
                    case 0: beginFrame = _TopBeginFrame;    endFrame = _TopEndFrame; break;
                    case 1: beginFrame = _BottomBeginFrame; endFrame = _BottomEndFrame; break;
                    case 2: beginFrame = _RightBeginFrame;  endFrame = _RightEndFrame; break;
                    case 3: beginFrame = _LeftBeginFrame;   endFrame = _LeftEndFrame; break;
                    case 4: beginFrame = _MidBeginFrame;    endFrame = _MidEndFrame; break;
                }

                float begin = GetRandomValue(beginFrame, random);
                float end   = GetRandomValue(endFrame, random);
                v.beginFrame = begin;
                v.range = max(1.0, end - begin + 1);
                return v;
            }

            int GetFrameIndexFromUV(float2 uv)
            {
                float dx = 1.0 / _TilesX;
                float dy = 1.0 / _TilesY;
                
                int u = (uv.x / dx) - frac(uv.x / dx);
                int v = (uv.y / dy) - frac(uv.y / dy);
                return (_TilesY - v) * (int)_TilesX + u;
            }

            float2 GetUVFromFrameIndex(v2f i)
            {
                int index = GetFrameIndexFromUV(i.uv.xy);
                index =  clamp((int)i.beginFrame + (index % (int)i.range), i.beginFrame, i.beginFrame + i.range- 1) + _TilesX;

                float dx = 1.0 / _TilesX;
                float dy = 1.0 / _TilesY;

                int x = index % (int)_TilesX;
                int y = _TilesY-(index / (int)_TilesX);

                float2 uv;
                uv.x = frac(i.uv.x / dx) * dx + dx * x;
                uv.y = frac(i.uv.y / dy) * dy + dy * y;
                return uv;
            }

            float3 CustomLighting(float3 normal, float3 lightDir, float3 lightColor)
            {
                float NdotL = max(dot(normal, lightDir), 0.0);
                float3 light = NdotL * lightColor;
                return light;
            }
            
            
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.position);
                o.color = v.color;
                o.uv = v.uv;
                o.position = v.position;
                o = GetBeginFrameAndRange(o, (int)v.random % 4);
                o.normal = normalize(_Position - v.position);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                if(_Cull != 0)
                if(i.position.z - _Position.z < 0)
                {
                    discard;
                }

                float2 uv = GetUVFromFrameIndex(i);
                float4 texColor =  tex2D(_MainTex, uv) * i.color;   
                Light mainLight = GetMainLight();
                float3 lighting = CustomLighting(i.normal, -mainLight.direction, mainLight.color);
                float3 finalRGB = texColor.xyz * lerp(float3(1,1,1), lighting, _LightingStrength);
                float4 finalColor = float4(finalRGB, texColor.a);
                return finalColor;

            }

            ENDHLSL
        }
    }

    FallBack "Particles/Alpha Blended"
}
