Shader "Easy2D/Alpha Key" {  
	Properties {  
		_MainTex ("Texture", 2D) = "white" {}  
	}  
	Category {  
		Tags { "Queue"="Geometry" "IgnoreProjector"="True" "RenderType"="Geometry" }  

		Blend One Zero            

		AlphaTest Greater 0
		Cull Off Lighting Off ZWrite On Fog { Color (0,0,0,0) }  
		ZTest Less  

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
