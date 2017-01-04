using UnityEngine;
using System.Collections.Generic;
using AssemblyCSharp;
using DataPersist;
using System.Collections;
using System;
using LilyHeart;

public class BackGroundName
{
	public BackGroundItem.ButtonType buttontype;
	public string breBtnname;
	public string BtnName;
	public BackGroundName(BackGroundItem.ButtonType type,string na,string bname)
	{
		breBtnname=na;
		buttontype=type;
		BtnName=bname;
	}
	
}

public class BackGroundItem : MonoBehaviour
{
	public enum Arrangement
	{
		Horizontal,
		Vertical,
	}

	public Arrangement arrangement = Arrangement.Horizontal;
	public int maxPerLine = 0;
	public float cellWidth = 200f;
	public float cellHeight = 200f;
	public bool repositionNow = false;
	public bool sorted = false;
	public bool hideInactive = true;

	bool mStarted = false;
	public GameObject Match;
	public GameObject CardFriend;
	public GameObject NewGame;
	public GameObject QuickStart;
	
	public float MoveTime=0.8f;
	
	public iTween.EaseType easytype=iTween.EaseType.easeInOutQuart;
	
	public float MovebackTime=0.8f;
		public iTween.EaseType Backeasytype=iTween.EaseType.easeInOutQuart;

	public GameObject Panel;
	
 	List<BackGroundName> Button1_3Names{get;set;}
 	List<BackGroundName> Button1_2Names{get;set;}
	
	
 
	public enum ButtonType
	{
		ButtonTypeFreshOne,
		ButtonTypeRegister,
		ButtonTypeChangeHeader,
		ButtonTypeBuyChip,
		ButtonTypeBuyDaoju,
	}
	
	void AddButtons(ButtonType type)
	{
		if(type==ButtonType.ButtonTypeFreshOne)
		{
			ToInsertButtonBeforItem("1_2");
		}
		
		else if(type==ButtonType.ButtonTypeRegister)
		{
			ToInsertButtonBeforItem("1_3");
		}
	}
 
 	void ToInsertButtonBeforItem(string name)
	{
		GameObject prefab=Resources.Load("prefab/Gambling/RoomButton") as GameObject;
		GameObject newitem=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
  		
  		Transform myTrans=transform;
 		for(int i=0;i<myTrans.GetChildCount();i++)
		{
			Transform t=myTrans.GetChild(i);
			 
 			if(t.name==name)
			{
				newitem.transform.parent=transform;
				newitem.transform.localScale=new Vector3(1f,1f,1f); 
				newitem.transform.localPosition = t.transform.localPosition;// new Vector3(t.transform.localPosition.x + cellWidth/8,t.transform.localPosition.y-cellHeight/4,t.transform.localPosition.z);
   			}
			
			Vector3 newpostion=new Vector3(t.transform.localPosition.x-cellWidth,t.transform.localPosition.y,t.transform.localPosition.z);
			iTween.MoveTo(t.gameObject,iTween.Hash("position",newpostion,"time",MoveTime,"looptype","none","islocal",true,"easetype",easytype,"oncomplete","ButtonsMoveCompleted","oncompletetarget",gameObject));
 		    
			if(t.name==name)
			{
				break;
			}
		
		}
		
	}
	
	IEnumerator destoryButton(float tim,GameObject btn)
	{
		yield return new WaitForSeconds(tim);
		
		for(int i=0;i<transform.GetChildCount();i++)
		{
 			Transform t=transform.GetChild(i);
  			if(Convert.ToInt32(t.name) < Convert.ToInt32(btn.transform.parent.parent.name))
			{
				Vector3 newpostion=new Vector3(t.transform.localPosition.x+cellWidth,t.transform.localPosition.y,t.transform.localPosition.z);
				iTween.MoveTo(t.gameObject,iTween.Hash("position",newpostion,"time",MovebackTime,"looptype","none","islocal",true,"easetype",Backeasytype,"oncomplete","ButtonsMoveCompleted","oncompletetarget",gameObject));
	    
			}
  		}
		
		Destroy(btn.transform.parent.parent.gameObject);
	}
	
	void DestoryBtn(GameObject btn)
	{
	
		StartCoroutine(destoryButton(0.06f,btn));
	}
	 
	
	GameObject LoadTheBtn(BackGroundName backgroundname,Vector3 postion,string name)
	{
		GameObject prefab=Resources.Load("prefab/Gambling/"+backgroundname.BtnName) as GameObject;
		if(prefab!=null)
		{
			GameObject newitem=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
			
			newitem.transform.parent=transform;
			newitem.transform.localScale=new Vector3(1,1,1); 
			newitem.transform.localPosition=postion;
	 		newitem.name=name;
			
		    UIButtonMessage btnmess	= newitem.transform.FindChild("Button/Button_close").GetComponent<UIButtonMessage>();
			btnmess.target=gameObject;
			btnmess.functionName="DestoryBtn";
			btnmess.enabled=true;
 			 
 			return newitem;
		}
		else
			return null;
	}
	void SetButtons()
	{
		int numofbtn1_3=0;
		for(int i=0;i<Button1_3Names.Count;i++)
		{
 			BackGroundName backgrondname= Button1_3Names[i];
			Transform tr1_3=transform.FindChild(backgrondname.breBtnname);
			Vector3 lopstion=new Vector3(tr1_3.localPosition.x-cellWidth*numofbtn1_3,tr1_3.localPosition.y,tr1_3.localPosition.z);
			LoadTheBtn(backgrondname,lopstion,Convert.ToString(39-numofbtn1_3));
			numofbtn1_3++;
		}
		
		int numofbtn1_2=numofbtn1_3;
		
		
		for(int i=0;i<Button1_2Names.Count;i++)
		{
			BackGroundName backgrondname= Button1_2Names[i];
			Transform tr1_2=transform.FindChild(backgrondname.breBtnname);
			Vector3 lopstion=new Vector3(tr1_2.localPosition.x-cellWidth*numofbtn1_2,tr1_2.localPosition.y,tr1_2.localPosition.z);
			LoadTheBtn(backgrondname,lopstion,Convert.ToString(29-(numofbtn1_2-numofbtn1_3)));
			numofbtn1_2++;
		}
		
		
		
		for(int i=0;i<transform.GetChildCount();i++)
		{
			
			Transform t=transform.GetChild(i);
			//Debug.Log(t.name);
 			if(t ==NewGame.transform)
			{
				Vector3 newpostion=new Vector3(t.transform.localPosition.x-cellWidth*numofbtn1_3,t.transform.localPosition.y,t.transform.localPosition.z);
			iTween.MoveTo(t.gameObject,iTween.Hash("position",newpostion,"time",MoveTime,"looptype","none","islocal",true,"easetype",easytype,"oncomplete","ButtonsMoveCompleted","oncompletetarget",gameObject));
	    
			}
			else if(t==Match.transform ||t ==CardFriend.transform)
			{
				Vector3 newpostion=new Vector3(t.transform.localPosition.x-cellWidth*numofbtn1_2,t.transform.localPosition.y,t.transform.localPosition.z);
			iTween.MoveTo(t.gameObject,iTween.Hash("position",newpostion,"time",MoveTime,"looptype","none","islocal",true,"easetype",easytype,"oncomplete","ButtonsMoveCompleted","oncompletetarget",gameObject));
	    
			}
			
 		}
		 
		Transform reg= transform.FindChild("29/Button");
		if(reg!=null)
		{
			UIButtonMessage me=reg.GetComponent<UIButtonMessage>();
			if(me!=null)
			{
				me.target=Panel;
			}
			
		}
		foreach(UIDragObject ob in transform.GetComponentsInChildren<UIDragObject>())
		{
			ob.target=transform;
		}
		foreach(TF_ButtonMessage ob in transform.GetComponentsInChildren<TF_ButtonMessage>())
		{
			ob.target=Panel;
			ob.condition_target=Panel;
		}
		
		Button1_2Names.Clear();
		Button1_3Names.Clear();
	}
 	
	void ButtonsMoveCompleted()
	{
		
 	}
	void Awake()
	{
		Button1_3Names=new List<BackGroundName>();
		Button1_2Names=new List<BackGroundName>();
 		roombtnAction.KickByVip+=KickByVip;
		roombtnAction.DidRegisterInSetupSuccessEvent+=DidRegisterInSetupSuccessEvent;

	}
	void CheckButtons()
	{
//	 	Button1_2Names.Add(new BackGroundName(ButtonType.ButtonTypeRegister,CardFriend.name,"Btn_buyprops"));
//	 	Button1_2Names.Add(new BackGroundName(ButtonType.ButtonTypeRegister,CardFriend.name,"Btn_reg"));
//	 Button1_2Names.Add(new BackGroundName(ButtonType.ButtonTypeRegister,CardFriend.name,"Btn_buychips"));
//	 Button1_2Names.Add(new BackGroundName(ButtonType.ButtonTypeRegister,CardFriend.name,"Btn_changeavatar"));
// 		Button1_3Names.Add(new BackGroundName(ButtonType.ButtonTypeFreshOne,NewGame.name,"Btn_guide"));
//
// 		Invoke("SetButtons",0.5f);
//		
//		
//		 return;
		if(User.Singleton.UserData.UserType==UserType.Guest)	
		{
			int va=UnityEngine.Random.Range(0,2);
   		 	if(va==1)
		 	{
 		 			Button1_2Names.Add(new BackGroundName(ButtonType.ButtonTypeRegister,CardFriend.name,"Btn_reg"));
	 		}
			else
			{
				if(User.Singleton.Avators.Count<=1)
				{
					va=UnityEngine.Random.Range(0,2);
					if(va==1)
					{
						Button1_2Names.Add(new BackGroundName(ButtonType.ButtonTypeChangeHeader,CardFriend.name,"Btn_changeavatar"));
 					}
				}

			}
		}
		
		if(User.Singleton.UserData.Chips < User.Singleton.UserData.BiggestWin*0.3f)
		{
 			Button1_2Names.Add(new BackGroundName(ButtonType.ButtonTypeBuyChip,CardFriend.name,"Btn_buychips"));
		}
		
		if(ActorInforController.BreakBiggestWonPot==true)
		{
			ActorInforController.BreakBiggestWonPot=false;
			Debug.LogWarning("ActorInforController.BreakBiggestWonPot");
			int va=UnityEngine.Random.Range(0,2);
			if(va==1)
			{
				Button1_2Names.Add(new BackGroundName(ButtonType.ButtonTypeChangeHeader,CardFriend.name,"Btn_buyprops2"));
			}
		}
 		
 		//Button1_2Names.Reverse();
 		Invoke("SetButtons",0.5f);
 		 
	}
	 
	void Start ()
	{
 		mStarted = true;
		Reposition();
 		 
		CheckButtons();
   	}
	void DidRegisterInSetupSuccessEvent()
	{
		//Debug.LogError("DidRegisterInSetupSuccessEvent");
		Button1_3Names.Add(new BackGroundName(ButtonType.ButtonTypeFreshOne,NewGame.name,"Btn_guide"));

	}
	void KickByVip()
	{
		Button1_2Names.Add(new BackGroundName(ButtonType.ButtonTypeBuyChip,CardFriend.name,"Btn_buyprops"));

	}
	void Update ()
	{
		if (repositionNow)
		{
			repositionNow = false;
			Reposition();
		}
	}
	void OnDestroy()
	{
		StopCoroutine("destoryButton");
 		roombtnAction.KickByVip-=KickByVip;
		roombtnAction.DidRegisterInSetupSuccessEvent-=DidRegisterInSetupSuccessEvent;

	} 
	static public int SortByName (Transform a, Transform b) { return string.Compare(a.name, b.name); }

	/// <summary>
	/// Recalculate the position of all elements within the grid, sorting them alphabetically if necessary.
	/// </summary>

	public void Reposition ()
	{
		if (!mStarted)
		{
			repositionNow = true;
			return;
		}

		Transform myTrans = transform;

		int x = 0;
		int y = 0;

		if (sorted)
		{
			List<Transform> list = new List<Transform>();

			for (int i = 0; i < myTrans.childCount; ++i)
			{
				Transform t = myTrans.GetChild(i);
				if (t) list.Add(t);
			}
			list.Sort(SortByName);

			for (int i = 0, imax = list.Count; i < imax; ++i)
			{
				Transform t = list[i];
				if (!t.gameObject.active && hideInactive) continue;

				float depth = t.localPosition.z;
				t.localPosition = (arrangement == Arrangement.Horizontal) ?
					new Vector3(cellWidth * x, -cellHeight * y, depth) :
					new Vector3(cellWidth * y, -cellHeight * x, depth);

				if (++x >= maxPerLine && maxPerLine > 0)
				{
					x = 0;
					++y;
				}
			}
		}
		else
		{
			for (int i = 0; i < myTrans.childCount; ++i)
			{
				Transform t = myTrans.GetChild(i);

				if (!t.gameObject.active && hideInactive) continue;

				float depth = t.localPosition.z;
				t.localPosition = (arrangement == Arrangement.Horizontal) ?
					new Vector3(cellWidth * x, -cellHeight * y, depth) :
					new Vector3(cellWidth * y, -cellHeight * x, depth);

				if (++x >= maxPerLine && maxPerLine > 0)
				{
					x = 0;
					++y;
				}
			}
		}

		UIDraggablePanel drag = NGUITools.FindInParents<UIDraggablePanel>(gameObject);
		if (drag != null) drag.UpdateScrollbars(true);
	}
	
}