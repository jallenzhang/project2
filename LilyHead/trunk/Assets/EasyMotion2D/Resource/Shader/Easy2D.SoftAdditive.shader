Shader "Easy2D/SoftAdditive" {  
	Properties {  
		_MainTex ("Texture", 2D) = "white" {}  
	}  

	Category {  
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }  

		Blend SrcAlpha OneMinusSrcAlpha            

		Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }  
//"		ZTest Less  

		BindChannels {  
			Bind "Color", color  
			Bind "Vertex", vertex  
			Bind "TexCoord", texcoord  
		}  

		// ---- Dual texture cards  
		SubShader {  
			Pass {  
				SetTexture [_MainTex] {  
					combine texture * primary  
				}  
			}  
		}  
	}  
}