Shader "UI/prana"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
       _StencilComp("Stencil Comparison", Float) = 8
       _Stencil("Stencil ID", Float) = 0
       _StencilOp("Stencil Operation", Float) = 0
       _StencilWriteMask("Stencil Write Mask", Float) = 255
       _StencilReadMask("Stencil Read Mask", Float) = 255

       _ColorMask("Color Mask", Float) = 15
       [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
    }
        SubShader
    {
           Stencil
           {
               Ref[_Stencil]
               Comp[_StencilComp]
               Pass[_StencilOp]
               ReadMask[_StencilReadMask]
               WriteMask[_StencilWriteMask]
           }
        // No culling or depth
        Cull Off
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha // Traditional transparency
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            sampler2D _MainTex;
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                // just invert the colors
                col.r = .1;// sin(i.vertex.y);
                float x = i.vertex.x;
                float y = i.vertex.y;
                float cia = i.color.r;
                float aia = i.color.g;
                float bia = i.color.b;
                cia *= ((cia * cia * cia * cia)*1000)%1.0;
                bia *= sin(bia) * bia;
                aia *= cos(aia) * aia;
                float fx = x*(1-cia)+cia*(x * i.color.g + y * (1 - i.color.g));
                float fy = y*(1-cia)+cia*(y * i.color.g + x * (1 - i.color.g));
                x = fx;
                y = fy;
                float c = sin(y / 5 + _Time.y * 2-cos(x/10)) * .3 + .7;
                col.g = 0;//sin(_Time.y+i.uv.x)*sin(_Time.y+.1+i.uv.x)*.3+.4;


                c -= sin(_Time.y *bia+ x / 70) * .2;
                c -= sin(_Time.y *aia+ x / 150) * .1;
                c -= (1-cos(_Time.y + x / 150+y/150)) * .05;
                c -= cos(_Time.y + y/150) * .05;
                c += sin(i.color.r * i.color.r * i.color.r)*.25;
                c *= abs(cos(i.color.g * i.color.b * i.color.g * i.color.b * 10))*.5+.5;
                //c -= pow(i.color.r,cos(_Time.y + y / 150) * .05);
                c += (sin(_Time.y + y/50)+ sin(_Time.y + x/50))* .15*(1.2/sin(i.color.r));
                
                

                if (c > .6) {
                //if (c > jr) {
                    col = sin(y / 50 + _Time.y * sin(i.color.b) * 2 - cos(x / (100))) * .3 + .7;
                    
                }
                else
                {
                    col = c * c;
                }
                col = 1 - col;
                if (col.a < .4) { col = .4; }
                col = i.color * col;
                return col;
            }
            ENDCG
        }
    }
}
