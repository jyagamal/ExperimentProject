Shader "Unlit/LerfEdge"
{
    Properties
    {
        _MainTex("MainTexture", 2D) = "white" {}
        _MaskTex("MaskTexture", 2D) = "white" {}
        _OutlineColor("OutlineColor", Color) = (0, 0, 0, 0)
        _OutlineThick("Outline Thick", Range(0.0, 100.0)) = 5.0
        _OutlineThreshold("Outline Threshold", float) = 0.0
    }
    SubShader
    {
        Tags 
        {
            "Queue" = "Transparent"
            "RenderType" = "Tranceparent" 
        }

        Blend SrcAlpha OneMinusSrcAlpha

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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _MaskTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            float _OutlineThick;
            float _OutlineThreshold;
            float4 _OutlineColor;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {

                // �ߗׂ̃e�N�X�`���F���T���v�����O
                half diffU = _MainTex_TexelSize.x * _OutlineThick;
                half diffV = _MainTex_TexelSize.y * _OutlineThick;
                half3 col00 = tex2D(_MaskTex, i.uv + half2(-diffU, -diffV));
                half3 col10 = tex2D(_MaskTex, i.uv + half2(diffU, -diffV));
                half3 col01 = tex2D(_MaskTex, i.uv + half2(-diffU, diffV));
                half3 col11 = tex2D(_MaskTex, i.uv + half2(diffU, diffV));
                half3 diff00_11 = col00 - col11;
                half3 diff10_01 = col10 - col01;

                // �Ίp���̐F���m���r���č����������������A�E�g���C���Ƃ݂Ȃ�
                half3 outlineValue3 = diff00_11 * diff00_11 + diff10_01 * diff10_01;
                half4 outlineValue4 = half4(outlineValue3, 1);

                //�}�X�N�ƃ��C���̉摜���������킹��
                float4 maskTex = tex2D(_MaskTex, i.uv);

                //�}�X�N�̔��ƍ��̋��E���̎኱�O���[�ɂȂ��Ă��镔�������Ƃ��ď�������
                maskTex = (maskTex.x > 0.03) ? float4(1, 1, 1, 1) : float4(0, 0, 0, 1);

                float4 drawTex = tex2D(_MainTex, i.uv) * maskTex;

                //�A�E�g���C���Ƃ��Ĕ��肵����ŁA�}�X�N�̍��������̂݁A�ݒ肵���F�̃A�E�g���C���ɂ���
                if (outlineValue4.x - _OutlineThreshold > 0 && maskTex.x <= 0.01)
                    drawTex.rgb = _OutlineColor.rgb;
                
                //�֊s��h��I�������ɍ����������A���t�@��������
                if (drawTex.r <= 0.01 && drawTex.g <= 0.01 && drawTex.b <= 0.01)
                    drawTex.a = 0.0;
                
                return drawTex;
                

            }
            ENDCG
        }
    }
}
