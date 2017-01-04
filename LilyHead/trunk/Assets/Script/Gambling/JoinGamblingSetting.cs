using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using DataPersist;
using AssemblyCSharp.Helper;
using LilyHeart;

public class JoinGamblingSettingInfor
{
	public bool GamblingSenior=true;
	public long MaxBlindAmnit=4;
	public int thinkingTime=15;
	public bool onlyFirend=false;
 	
	public bool isFastStart=false;
	
	public bool haveDone=false;
		
	public static JoinGamblingSettingInfor SettingInfor;
	public static JoinGamblingSettingInfor Singleton {
			get 
			{
			if (SettingInfor == null)
				SettingInfor = new JoinGamblingSettingInfor();
				return SettingInfor;
			}
	}
	
	public void ReSetSettingInfor()
	{
		 GamblingSenior=true;
		 MaxBlindAmnit=4;
		 thinkingTime=15;
		 onlyFirend=false;
	 	
		 isFastStart=false;
		
		haveDone=false;
	}
	
	private JoinGamblingSettingInfor()
	{
		
	}
	
}

public class JoinGamblingSetting : MonoBehaviour {
	
	int cardtype=0;
	bool isthinkFast=true;
	bool  allowOnllyCardFriendIn=true;
	
	public GameObject slider;
	
	long MaxBlindAmnit;
	AsyncOperation async = null;
	
	void seTGameObjectIcon(string name,bool press)
	{
		Transform tr=transform.FindChild(name);
		Transform bg=tr.FindChild("Background");
		UISprite imagebtn=bg.GetComponent<UISprite>();
		if(press)
		{
 		    imagebtn.spriteName="STBtnPress";
		}
		else 
		{
		    imagebtn.spriteName="STBtn";
		
		}
 
	}
	
	void setCartTypeLabel(int type)
	{
		if(type==0)
		{
			JoinGamblingSettingInfor.Singleton.GamblingSenior=true;
			Transform hidelab2=transform.FindChild("Labe2");
			hidelab2.gameObject.active=false;
			
			Transform hidelab1=transform.FindChild("Labe1");
			hidelab1.gameObject.active=true;
		}
		else
		{	JoinGamblingSettingInfor.Singleton.GamblingSenior=false;

			Transform hidelab2=transform.FindChild("Labe2");
			hidelab2.gameObject.active=true;
			
			Transform hidelab1=transform.FindChild("Labe1");
			hidelab1.gameObject.active=false;
		}
		ChcekSureBtnState();
	}
	
	void setCardType(GameObject btn)
	{
		
 		if(btn!=null)
		{
 			if(btn.name=="Button_1")
			{
				cardtype=0;
			}
			else
			{
				if(User.Singleton.UserData.Chips<5000000)
				{
					showTip();
				    return;
				}
				cardtype=1;
			}
		}
		if(cardtype==0)
		{
			seTGameObjectIcon("Button_1",true);
			seTGameObjectIcon("Button_2",false);
   		}
		else
		{
			
			seTGameObjectIcon("Button_1",false);
			seTGameObjectIcon("Button_2",true);
  		}
		OnSliderChange();
		setCartTypeLabel(cardtype);
 	}
	
	void setThinkingTime(GameObject btn)
	{
		if(btn!=null)
		{
			if(btn.name=="Button_3")
			{
				isthinkFast=true;
			}
			else
				isthinkFast=false;
		}
 		if(isthinkFast)
		{
			seTGameObjectIcon("Button_3",true);
			seTGameObjectIcon("Button_4",false);
			
			JoinGamblingSettingInfor.Singleton.thinkingTime=10;
 		}
		else
		{
			seTGameObjectIcon("Button_3",false);
			seTGameObjectIcon("Button_4",true);
			
			JoinGamblingSettingInfor.Singleton.thinkingTime=15;
		}
		
	}
	
	
	void setAllowOnlyFriendIn(GameObject btn)
	{
		if(btn!=null)
		{
			if(btn.name=="Button_5")
			{
				allowOnllyCardFriendIn=true;
			}
			else
				allowOnllyCardFriendIn=false;
		}
 		if(allowOnllyCardFriendIn)
		{
			seTGameObjectIcon("Button_5",true);
			seTGameObjectIcon("Button_6",false);
			JoinGamblingSettingInfor.Singleton.onlyFirend=false;
 		}
		else
		{
			seTGameObjectIcon("Button_5",false);
			seTGameObjectIcon("Button_6",true);
			
			JoinGamblingSettingInfor.Singleton.onlyFirend=true;
			if(User.Singleton.Friends==null||User.Singleton.Friends.Count==0)
			{
				GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new AllowFriendDialog());
				PopUpTips();
			}
		}
	}
	
	void SetSureBtnCanBeUsed(bool sure)
	{
		Transform sureBtn=transform.FindChild("ButtonRed");
		if (sureBtn == null)
			return;
		
		BoxCollider bixcollider=sureBtn.GetComponent<BoxCollider>();
		if (bixcollider)
			bixcollider.enabled=sure;
		Transform Background=sureBtn.FindChild("Background");
		UISlicedSprite Backgroundsprite=Background.GetComponent<UISlicedSprite>();
		Backgroundsprite.color=(sure?new Color(1,1,1,1):new Color(0.64f,0.64f,0.64f,1));
			
	}
	void ChcekSureBtnState()
	{
		 
 		if((User.Singleton.UserData.Chips<=50000 && cardtype==1)|| MaxBlindAmnit > User.Singleton.UserData.Chips/100)
		{
			SetSureBtnCanBeUsed(false);
		}
		else
		{
			SetSureBtnCanBeUsed(true);
		}
		
		 
	}
	
	void UsrChipStateCheck()
	{
		setCurrentLabel();
	    setCardType(null);
	    setThinkingTime(null);
		setAllowOnlyFriendIn(null);
		SetSureBtnCanBeUsed(true);
	}
	 
	// Use this for initialization
	void Start () {
		
		UISlider uiSlider = slider.GetComponent<UISlider>();
		uiSlider.numberOfSteps = 100;
	    UsrChipStateCheck();
	}
	
	void SetProgress()
	{
		if (async != null)
		{
			Debug.Log("Progress is: " + async.progress);
			LoadingPercentHelper.Progress = async.progress;
		}
	}
	
	void GotoGame()
	{
		transform.parent.parent.gameObject.AddComponent<ShowLoadingTable>();
		async = Application.LoadLevelAsync("GamblingInterface_Title");
	}
	
	void createGameInit()
	{
		JoinGamblingSettingInfor.Singleton.MaxBlindAmnit=MaxBlindAmnit;
		
		GotoGame();
	}
	
	
	void setCurrentLabel()
	{
		 if(User.Singleton.UserData.Chips<=5000000)
		{
		    cardtype=0;
		    MaxBlindAmnit=10; 
		}
		else
		{
			cardtype=1;
		    MaxBlindAmnit=10000; 
		}
		 
		setCartTypeLabel(cardtype);
		Debug.Log(User.Singleton.UserData.Chips);
	}
	
	void setSliderLabelInfor(string str)
	{
		Transform Thumbtr=slider.transform.Find("Thumb");
		Transform Lab1=Thumbtr.FindChild("Lab1");
		Transform Lab2=Thumbtr.FindChild("Lab2");
  		UILabel l1=Lab1.GetComponent<UILabel>();
		UILabel l2=Lab2.GetComponent<UILabel>();
		 
		l1.text=str;
		l2.text=str;
		
	}
	
	void OnSliderChange()
	{
		UISlider uislider=slider.GetComponent<UISlider>();
		
		int sliderValue=(int)(uislider.sliderValue*100);
		
		
 		
		if(cardtype==0)
		{
			int max= 50;
			if((sliderValue)==0)
			{
				MaxBlindAmnit=10;
			}
			else
			{
		    	MaxBlindAmnit=(int)(sliderValue*(max));
				if(MaxBlindAmnit<10)
					MaxBlindAmnit=10;

			}
			
		 	string str=string.Empty;
			if(MaxBlindAmnit>=1000)
			     str=string.Format("${0:N1}K",(MaxBlindAmnit/1000.0f));
			else
				 str=string.Format("${0}",(MaxBlindAmnit));
			
			
		   setSliderLabelInfor(str);
 		}
		else
		{
			int max=5000;
			if((sliderValue*5000)==0)
			{
				MaxBlindAmnit=10000;
			}
			else
			{
		    	MaxBlindAmnit=sliderValue*max;

			}
			
			string str=string.Empty;
			if(MaxBlindAmnit>=1000)
			     str=string.Format("${0}K",(int)(MaxBlindAmnit/1000));
			else
				 str=string.Format("${0}",(MaxBlindAmnit));
			
			
		   setSliderLabelInfor(str);
		}

  		
 	    ChcekSureBtnState();
		
		//
	}
	
	void showTip()
	{
		GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new JointGameInfoDialog());
			PopUpTips();
	}
	
	void PopUpTips()
	{
 		GameObject prefab=Resources.Load("prefab/tips2") as GameObject;
		GameObject item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
		
		item.transform.parent=transform;
		item.transform.localPosition=new Vector3(0,200,-21);
		item.transform.localScale =new Vector3(1,1,1);
	}
	// Update is called once per frame
	void Update () {
	   	SetProgress();
		ChcekSureBtnState();
	}
	
	void CloseDialog()
	{
		Transform child=gameObject.transform.FindChild("ButtonRed");
		if(child!=null)
		{
			GameObject createBtn = child.gameObject;
			if (createBtn)
				Destroy(createBtn);
			
			fadePanel panel = gameObject.GetComponent<fadePanel>();
			panel.fadeOut(null);
		}
	}
}
public class JointGameInfoDialog : DialogInfo {
		public JointGameInfoDialog()
		{
			this.Title = LocalizeHelper.Translate("GUEST_LOGIN_INFO_TITLE");
			this.Description = LocalizeHelper.Translate("CREATTABLEINFO_MONEY_NOT_ENOUGH");
			this.Buttons = 1;
		}
}
