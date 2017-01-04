using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public class Util
	{
		public Util ()
		{
 		}
 		public static void FadeWidtsOutWithTime(Transform target ,float time)
		{
			Transform[] trs=target.GetComponentsInChildren<Transform>();
			foreach(Transform tr in trs)
			{
			    UIWidget widget=tr.GetComponent<UIWidget>();
				if(widget!=null)
				{
					Color mColor=widget.color;
					mColor.a=0;
 					TweenColor.Begin(tr.gameObject, time, mColor);
				}
 			}
		}
		public static void FadeWidtsOutWithTime(GameObject target ,float time)
		{
			if(target!=null)
			 FadeWidtsOutWithTime(target.transform,time);
		}
		public static void fadeOutWidts(GameObject target)
		{
			
			Transform[] trs=target.GetComponentsInChildren<Transform>();
			foreach(Transform tr in trs)
			{
			    UIWidget widget=tr.GetComponent<UIWidget>();
				if(widget!=null)
				{
					Color mColor=widget.color;
					mColor.a=0;
			    	widget.color=mColor;
				}
			}
			
	 	}
		 
		public static void FadeWidtsOutWithTime(Transform target )
		{
			Transform[] trs=target.GetComponentsInChildren<Transform>();
			foreach(Transform tr in trs)
			{
			    UIWidget widget=tr.GetComponent<UIWidget>();
				if(widget!=null)
				{
					Color mColor=widget.color;
					mColor.a=0;
 					widget.color=mColor;
				}
 			}
		}
		public static bool isFadein(Transform target)
		{
 			   
			UIWidget widget=target.GetComponent<UIWidget>();
			if(widget!=null)
			{
				if(widget.color.a==1)
					return true;
				 else
					return false;
			}
			return false;
				 
		}
		
		public static bool isMatch()
		{
			if(TableState.Singleton.gametabletype==TableState.GameTableType.GameTableType_Normal)
				return false;
			else
				return true;
		}
		
		public static MonoBehaviour GetCurrentController()
		{
			if(TableState.Singleton.gametabletype==TableState.GameTableType.GameTableType_Normal)
				return ActorInforController.Singleton;
			else
				return MatchInforController.Singleton;
		}
		
		public static TweenColor FadeWidtOutWithTime(Transform target,float time)
		{
 			UIWidget widget=target.GetComponent<UIWidget>();
 			Color mColor=widget.color;
			mColor.a=0;
			TweenColor colow=TweenColor.Begin(target.gameObject,time,mColor);
 			return colow;
		}
		public static void FadeWidtsInWithTime(GameObject target ,float time)
		{
			FadeWidtsInWithTime(target.transform,time);
		}
		public static void FadeWidtsInWithTime(Transform target ,float time)
		{
			Transform[] trs=target.GetComponentsInChildren<Transform>();
			foreach(Transform tr in trs)
			{
			    UIWidget widget=tr.GetComponent<UIWidget>();
				if(widget!=null)
				{
					Color mColor=widget.color;
					mColor.a=1;
 					TweenColor.Begin(tr.gameObject, time, mColor);
				}
 			}
		}
		public static void FadeWidtsInWithTime(GameObject target)
		{
			FadeWidtsInWithTime(target.transform);
		}
		public static void FadeWidtsInWithTime(Transform target)
		{
			Transform[] trs=target.GetComponentsInChildren<Transform>();
			foreach(Transform tr in trs)
			{
			    UIWidget widget=tr.GetComponent<UIWidget>();
				if(widget!=null)
				{
					Color mColor=widget.color;
					mColor.a=1;
 					widget.color=mColor;
				}
 			}
		}
		
		public static TweenColor FadeWidtInWithTime(Transform target,float time)
		{
 			UIWidget widget=target.GetComponent<UIWidget>();
 			Color mColor=widget.color;
			mColor.a=1;
			TweenColor colow=TweenColor.Begin(target.gameObject,time,mColor);
 			return colow;
		}
		
		static public string getLableMoneyK_MModel(long money)
		{
			if(money>=1000 && money<1000000)
			  return  string.Format("${0:N1}K",money/1000.0f); 
			else if(money<1000)
				return "$"+money.ToString();
			else 
				return  string.Format("${0:N1}M",money/1000000.0f); 
	 
		}
		
		public static string GetChipSpriteByChip(long chip)
		{
			string spriteName=null;
			if(chip<100)
			{
	 			spriteName="GIFGreenChips";
	  		}
			else if(chip>=100 && chip <1000)
			{
	  			spriteName="GIFBlueChips";
	 		}
			else if(chip>=1000 && chip <5000)
			{
	  			spriteName="GIFOrangeChips";
	 		}
			else
			{
	 			spriteName="GIFRedChips";
	  		} 
			
			return spriteName;
		}
		public static string GetChipSpriteLabelByChip(long chip)
		{
			string spriteName=null;
			if(chip<100)
			{
	 			spriteName="GIFShowChips_Green";
	  		}
			else if(chip>=100 && chip <1000)
			{
	  			spriteName="GIFShowChips_Blue";
	 		}
			else if(chip>=1000 && chip <5000)
			{
	  			spriteName="GIFShowChips_Orange";
	 		}
			else
			{
	 			spriteName="GIFShowChips_Red";
	  		} 
			
			return spriteName;
		}
		
		public static long GetNumLabelValue(Transform role,string labelname)
		{
			string strValue=role.FindChild(labelname).GetComponent<UILabel>().text;
			if(strValue.Contains("$"))
			{
				strValue=strValue.Remove(0,1);
			}
			 
			long  labelVale=0; 
			if(strValue.Length==0)
			{
				labelVale=0;
			}
			else if(strValue.Contains("K"))
			{
				labelVale=(long)(Convert.ToDouble(strValue.Remove(strValue.Length-1,1))*1000);
			}
			else if(strValue.Contains("M"))
			{
				labelVale=(long)(Convert.ToDouble(strValue.Remove(strValue.Length-1,1))*1000000);
			}
			else if(strValue.Contains("G"))
			{
				labelVale=(long)(Convert.ToDouble(strValue.Remove(strValue.Length-1,1))*1000000000);
			}
			else
			{
				labelVale=(long)(Convert.ToDouble(strValue));
			} 
			//Debug.LogError(labelVale);
			return labelVale;
			
		}
		
		public static void SetLabelValue(Transform part,string name,string val)
		{
 			Transform label=part.FindChild(name);
			UILabel uilabel=label.GetComponent<UILabel>();
			uilabel.text=val;
			
			Transform subLabel=label.FindChild(name);
			if(subLabel!=null)
			{
				UILabel subuilabel=subLabel.GetComponent<UILabel>();
				subuilabel.text=val;
			}
		}
		public static void SetLabelValue(GameObject part,string val)
		{
			SetLabelValue(part.transform,val);
		}
		public static void SetLabelValue(Transform part,string val)
		{
 			 
			UILabel uilabel=part.GetComponent<UILabel>();
			uilabel.text=val;
			
			Transform subLabel=part.FindChild(part.name);
			if(subLabel!=null)
			{
				UILabel subuilabel=subLabel.GetComponent<UILabel>();
				subuilabel.text=val;
			}
		}
		
		
		public static string getChipValueCompomontByDot(long chips)
		{
			long temp=chips;
			Debug.Log(temp);
			int changeValue =  (int)(temp/1000);
			string valuestr=string.Empty;
			if(changeValue>0)
				valuestr =changeValue.ToString()+","+ getChipValueCompomontByDot(temp%100);
			else
				valuestr=temp.ToString();
			    	
				
		    return valuestr;
		}
		
		public static void BeGrayGameObject(GameObject obj,  bool iswork)
		{
			Debug.Log(iswork+"  "+obj.name);
			BoxCollider boxcollider=obj.GetComponent<BoxCollider>();
			if(boxcollider)
			{
				boxcollider.enabled=iswork;
			}
			UIWidget[] widgets=obj.GetComponentsInChildren<UIWidget>();
			foreach(UIWidget widget in  widgets)
			{
			widget.color=iswork?Color.white:Color.gray;
 			}
		}
		public static void BeTranSparentGameObject(GameObject obj,  bool iswork)
		{
		 
			UIWidget[] widgets=obj.GetComponentsInChildren<UIWidget>();
			foreach(UIWidget widget in  widgets)
			{
				if(widget!=null)
				{
					Color m=widget.color;
					m.a=(!iswork)?0.4f:1;
					widget.color=m;
				}
 			}
		}
		
		public static void DestoryChildrenGameobject(Transform parent,string name)
		{
			Transform[] trs=parent.GetComponentsInChildren<Transform>();
			foreach(Transform t in trs)
			{
				if(t.name==name)
				{
					GameObject.Destroy(t.gameObject);
				}
			}
		}
		public static void DebulogAllActorInfor()
		{
			
		}
		
	}
	
	
}

