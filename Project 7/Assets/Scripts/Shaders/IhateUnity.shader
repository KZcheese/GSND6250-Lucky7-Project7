
Shader "Custom/Paintshade"
{
    
    Properties
    {
     _BaseColor("Base Color", Color) = (1, 1, 1, 1)
     _Lighting("Light Direction", Color) = (0,0,0,0)
      _MainTex ("Texture", 2D) = "white" {}
      _Ambient("Ambient", Color) = (.5,.5,.5,.5)
      }

    // The SubShader block containing the Shader code. 
    SubShader
    {
        // SubShader Tags define when and under which conditions a SubShader block or
        // a pass is executed.
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            // The HLSL code block. Unity SRP uses the HLSL language.
            HLSLPROGRAM
            // This line defines the name of the vertex shader. 
            #pragma vertex vert
            // This line defines the name of the fragment shader. 
            #pragma fragment frag

            // The Core.hlsl file contains definitions of frequently used HLSL
            // macros and functions, and also contains #include references to other
            // HLSL files (for example, Common.hlsl, SpaceTransforms.hlsl, etc.).
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"            

            // The structure definition defines which variables it contains.
            // This example uses the Attributes structure as an input structure in
            // the vertex shader.
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
            TEXTURE2D(_MainTex);
            // This macro declares the sampler for the _BaseMap texture.
            SAMPLER(sampler_MainTex); 
            CBUFFER_START(UnityPerMaterial)
                // The following line declares the _BaseColor variable, so that you
                // can use it in the fragment shader.
                half4 _BaseColor; 
                float3 _Lighting; 
                float4 _Ambient;
                float4 _MainTex_ST;
                        
            CBUFFER_END         

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
                OUT.normal = TransformObjectToWorldNormal(IN.normal);   
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                // Returning the output.
                return OUT;
            }

            // The fragment shader definition.            
            half4 frag(Varyings IN) : SV_Target
            {
                half4 bright = _BaseColor*.75+_Ambient;
                half4 dark = _BaseColor *.25 +_Ambient ;
                half4 highlights = _BaseColor *2 + .5*_Ambient;
                // *SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv)*.5
                float light = dot (IN.normal,_Lighting)+dot(_BaseColor,_Ambient);
                float tex = (SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv)).r;
                float c =tex;
                float light2 =  sin(IN.uv.x*c) + sin(IN.uv.y*c) + .25*light - .33;
                float light3 =  sin(IN.uv.x*c) + sin(IN.uv.y*c) + .25*light-.75;
                light2 = clamp(100*light2,0,1);
                light3 = clamp(100*light3,0,1);
                half4 customColor =  lerp(dark, bright, light2) ;
                customColor =  lerp(customColor, highlights, light3) ;
                
                
                
                return customColor;

            }
            ENDHLSL
        }
    }
}