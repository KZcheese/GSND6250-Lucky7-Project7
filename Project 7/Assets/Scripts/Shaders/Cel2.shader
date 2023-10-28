Shader "Custom/Cel2"
{



    Properties
    {
        _LightColor("Light", Color) = (1, 1, 1, 1)
//        _DarkColor("Dark", Color) = (1, 1, 1, 1)
//        _HighlightColor("Highlight", Color) = (1, 1, 1, 1)
        //  _OutlineColor ("Outline color", Color) = (0,0,0,1)
        // _OutlineWidth ("Outlines width", Range (0.0, 2.0)) = .04
        _HighlightSize ("Highlight Size", Range (0.0, 1.0)) = .1
        _ShadowSize ("Shadow Size", Range (0.0, 1.0)) = .33
        //     _MainTex ("Texture", 2D) = "white" {}
        //   _Ambient("Ambient", Color) = (.5,.5,.5,.5)
    }


    // The SubShader block containing the Shader code. 
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"
        }



        /* Pass //Outline
             
             {
             
             
                 ZWrite Off
               
                 HLSLPROGRAM
     
                 #pragma vertex vert
                 #pragma fragment frag
                 
                  
     
                // The Core.hlsl file contains definitions of frequently used HLSL
                 // macros and functions, and also contains #include references to other
                 // HLSL files (for example, Common.hlsl, SpaceTransforms.hlsl, etc.).
             #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl" 
                 
                 struct Attributes
                 {
                     // The positionOS variable contains the vertex positions in object
                     // space.
                     float4 positionOS   : POSITION; 
                      half3 normal        : NORMAL;  
                      float2 uv           : TEXCOORD0; 
                                 
                 };
     
                 struct Varyings
                 {
                     // The positions in this struct must have the SV_POSITION semantic.
                     float4 positionHCS  : SV_POSITION;
                     half3 normal        : TEXCOORD0;
                     float2 uv           : TEXCOORD1;
                 };
                 
                   CBUFFER_START(UnityPerMaterial)
                  uniform float _OutlineWidth;
                  uniform float4 _OutlineColor;
                    half4 _LightColor;
                     half4 _DarkColor;
                     half4 _HighlightColor; 
                     half4 _HighlightSize;
                     half4  _ShadowSize;
              CBUFFER_END   
                 
                 Varyings vert(Attributes v)
                 {
                     Attributes original = v;
                     v.positionOS.xyz += _OutlineWidth * normalize(v.normal.xyz);
     
                     Varyings o;
                     o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                     v=original;
                    
                     return o;
     
                 }
     
                 half4 frag(Varyings i) : COLOR
                 {
                     return _OutlineColor;
                 }
     
                 ENDHLSL
             }
             // SubShader Tags define when and under which conditions a SubShader block or
             // a pass is executed.
            
            */
        Pass
        {
            ZWrite On


            // The HLSL code block. Unity SRP uses the HLSL language.
            HLSLPROGRAM
            // This line defines the name of the vertex shader. 
            #pragma vertex vert
            // This line defines the name of the fragment shader. 
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            // The Core.hlsl file contains definitions of frequently used HLSL
            // macros and functions, and also contains #include references to other
            // HLSL files (for example, Common.hlsl, SpaceTransforms.hlsl, etc.).
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                // The positionOS variable contains the vertex positions in object
                // space.
                float4 positionOS : POSITION;
                half3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                // The positions in this struct must have the SV_POSITION semantic.
                float4 positionHCS : SV_POSITION;
                half3 worldpos : POSITION1;
                half3 normal : TEXCOORD0;
                float2 uv : TEXCOORD1;
            };

            // The structure definition defines which variables it contains.
            // This example uses the Attributes structure as an input structure in
            // the vertex shader.
            CBUFFER_START(UnityPerMaterial)
                uniform float _OutlineWidth;
                uniform float4 _OutlineColor;
                uniform half4 _LightColor;
                uniform half4 _DarkColor;
                uniform half4 _HighlightColor;
                uniform half _HighlightSize;
                uniform half _ShadowSize;
            CBUFFER_END

            //TEXTURE2D(_MainTex);
            // This macro declares the sampler for the _BaseMap texture.
            // SAMPLER(sampler_MainTex); 


            // The vertex shader definition with properties defined in the Varyings 
            // structure. The type of the vert function must match the type (struct)
            // that it returns.
            Varyings vert(Attributes IN)
            {
                // Declaring the output object (OUT) with the Varyings struct.
                Varyings OUT;
                // The TransformObjectToHClip function transforms vertex positions
                // from object space to homogenous clip space.
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.worldpos = mul(unity_ObjectToWorld, float4(IN.positionOS.xyz, 1.0)).xyz;
                OUT.normal = TransformObjectToWorldNormal(IN.normal);
                OUT.uv = IN.uv;
                // Returning the output.
                return OUT;
            }

            inline float3 applyHue(float3 aColor, float aHue)
            {
                float angle = radians(aHue);
                float3 k = float3(0.57735, 0.57735, 0.57735);
                float cosAngle = cos(angle);
                //Rodrigues' rotation formula
                return aColor * cosAngle + cross(k, aColor) * sin(angle) + k * dot(k, aColor) * (1 - cosAngle);
            }

            // The fragment shader definition.            
            half4 frag(Varyings IN) : SV_Target
            {
                //Shadows and light
                half4 shadowCoord = TransformWorldToShadowCoord(IN.worldpos);
                Light mainLight = GetMainLight(shadowCoord);
                half3 Direction = mainLight.direction;
                half4 mainLightColor = half4(mainLight.color, 1);
                half DistanceAtten = mainLight.distanceAttenuation;
                ShadowSamplingData shadowSamplingData = GetMainLightShadowSamplingData();
                half shadowStrength = GetMainLightShadowStrength();
                half ShadowAtten = SampleShadowmap(shadowCoord,
                       TEXTURE2D_ARGS(_MainLightShadowmapTexture, sampler_MainLightShadowmapTexture),
                       shadowSamplingData, shadowStrength, false);

                half4 lights = mainLightColor * _LightColor;
                half4 darks = lerp(_LightColor, lights, .5) * .5; //(float4(1,1,1,1)- mainLightColor) ;

                float hueshift = 15;

                darks = float4(applyHue(darks.xyz, hueshift), 1);
                half4 highlights = lights + .1 * mainLightColor;
                highlights = float4(applyHue(highlights.xyz, -hueshift), 1);
                float light = (dot(IN.normal, Direction) + 1) * .5 * ShadowAtten;

                float light2 = light - _ShadowSize;
                float light3 = light - 1 + _HighlightSize;
                light2 = clamp(100 * light2, 0, 1);
                light3 = clamp(100 * light3, 0, 1);
                //half4 customColor =  lerp(_DarkColor, _LightColor, light2) ;
                //  customColor =  lerp(customColor, _HighlightColor, light3) ;
                half4 customColor = lerp(darks, lights, light2);
                customColor = lerp(customColor, highlights, light3);
                customColor.a = 1;


                return customColor;
            }
            ENDHLSL
        }
    }
    Fallback "Universal Render Pipeline/Lit"
}