using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using DataPersist;
using DataPersist.CardGame;
using AssemblyCSharp.Helper;
using System.Linq;
using LilyHeart;
public class BuyChip : MonoBehaviour {
	
	public GameObject lastChip;
	public GameObject MinChip;
	public GameObject CurrentChip;
	public GameObject MaxChip;
	
	public GameObject Slider;
	
	public GameObject TopPanel;
	
	public GameObject CallDestory_target;
	public string CallDestory_Function;
	
	public Vector3 localPosition=new Vector3(0,0,-100);
	public Vector3 localScale =new Vector3(1,1,1);
	
	public int noseat;
	
	long min=10;
	long max=1000;
	
 	public long currentValue;
	
	public AudioSource AudioSource {get;set;}
	// Use this for initialization
	void Start () {
 		
		CallDestory_target=ActorInforController.Singleton.transform.gameObject;
		transform.localPosition=localPosition;
		transform.localScale=localScale;
		
		TableInfo  info=Room.Singleton.PokerGame.TableInfo;
		if(info!=null)
		{
 			min=info.BigBlindAmnt*20;
	 		max=User.Singleton.UserData.Chips;
	  		currentValue=min;
			
 			Util.SetLabelValue(lastChip.transform,Util.getLableMoneyK_MModel(max)); 
			Util.SetLabelValue(MinChip.transform,Util.getLableMoneyK_MModel(min));
			Util.SetLabelValue(CurrentChip.transform,Util.getLableMoneyK_MModel(currentValue));
			Util.SetLabelValue(MaxChip.transform,Util.getLableMoneyK_MModel(max));
	 		 
	        AudioSource=gameObject.AddComponent<AudioSource>();
		}
		
	}
	
 	
	// Update is called once per frame
	void Update () {
	
	}
	
	void btnChips()
	{
	   	ActorInforController.Singleton.showBuyChips();
	}
	void BtnClose()
	{
		
	}
	
	void RoleSitDown(GameObject Btn)
	{
 
     	TableInfo info=Room.Singleton.PokerGame.TableInfo;
		if(info!=null)
		{
 			if(currentValue>=info.BigBlindAmnt*20)
			{ 
	  			for(int i=1;i<=9;i++)
				{
	                if(transform.name=="RoleInfo_"+i)
					{
 						if(info.GetPlayer((TableState.Singleton.CurrentNoSeat+(i-1))%9)==null)
						{
							User.Singleton.Sit((byte)((TableState.Singleton.CurrentNoSeat+(i-1))%9),currentValue);
 	 						break;
						}
	   				}
				}
			}
			else
			{
				GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new BuyIndDialog());
				ActorInforController.Singleton.PopUpTips();
	 		}
		}
 	}
	 
	
 	
	void btnBlueAction()
	{
 		
 		UISlider slider=Slider.GetComponent<UISlider>();
  		float slidervalue=	(slider.sliderValue*100.0f);
	 
		if(slidervalue>0)
		{
        	slidervalue--;
		    slider.sliderValue=slidervalue/100.0f;
	 
		  	long vu=(long)(slidervalue*((max-min)/100.0f)+min);
			if(vu>max)
				vu=max;
			 currentValue=vu;
 			Util.SetLabelValue(CurrentChip.transform,Util.getLableMoneyK_MModel(currentValue));
			  
		}
		else
		{
			Util.SetLabelValue(CurrentChip.transform,Util.getLableMoneyK_MModel(min));

		}
 
 	}
	
	void OnDestroy()
	{
		 
		if(CallDestory_target!=null)
		{
			CallDestory_target.SendMessage(CallDestory_Function, gameObject, SendMessageOptions.DontRequireReceiver);
		}
	}
	
	void btnButton_GreenAction()
	{
 		//User.Singleton.Sit(noseat); 
	}
	
	void btnButton_redAction()
	{
 
		UISlider slider=Slider.GetComponent<UISlider>();
		
 		float slidervalue=(slider.sliderValue*100);
		
		Debug.Log((max-min)/100.0f);
		Debug.Log(slidervalue);
 	  if(slidervalue<100)
	  {
        	slidervalue+=1;
  		    slider.sliderValue=slidervalue/100.0f;
 
			long vu=(long)(slidervalue*((max-min)/100.0f)+min);
			if(vu>max)
				vu=max;
		 	currentValue=vu;
 			Util.SetLabelValue(CurrentChip.transform,Util.getLableMoneyK_MModel(currentValue));
		}
 		else
		{
 			Util.SetLabelValue(CurrentChip.transform,Util.getLableMoneyK_MModel(max));
			currentValue=max;
		}
 
	}
	
	float GetSliderValue(UISlider slider)
	{
		
		//Debug.Log(slider.sliderValue);
		long slidervalue=(long)(slider.sliderValue*100);
		Debug.Log(slidervalue);
		float tempvalue=0;
		
	 
		SoundHelper.PlaySound("Music/Other/slider",AudioSource,0);
	 
		if(slidervalue==0)
		{
			tempvalue=min;
		}
		else
		{
			
			tempvalue=((max-min)/100.0f)*slidervalue+min;
			if(tempvalue<min)
				tempvalue=min;
		}
	
		if(slidervalue==100)
   			tempvalue=max;
		
		return tempvalue;
	}
	
	void OnSliderChange()
	{
 		UISlider slider=Slider.GetComponent<UISlider>();
		
		long currentChipsValue = (long)GetSliderValue(slider);
		
		string str=Util.getLableMoneyK_MModel(currentChipsValue);
		
 		Util.SetLabelValue(CurrentChip.transform,str); 	
 		currentValue=currentChipsValue;
 	 
		if(slider.sliderValue<1)
		{
			Util.SetLabelValue(lastChip.transform,Util.getLableMoneyK_MModel(max-currentChipsValue));
		}
		else 
			Util.SetLabelValue(lastChip.transform,Util.getLableMoneyK_MModel(0));

   	}
  	
	void btnClose()
	{
		Destroy(gameObject);
	}
	
	
}
public class BuyIndDialog : DialogInfo {

	public BuyIndDialog()
	{
		this.Title = LocalizeHelper.Translate("GUEST_LOGIN_INFO_TITLE");
		this.Description = LocalizeHelper.Translate("SEND_CHIP_MONEY_NOT_ENOUGH");
		this.Buttons = 1;
	}
}

public class NoFriendDialog : DialogInfo {

	public NoFriendDialog()
	{
		this.Title = LocalizeHelper.Translate("GUEST_LOGIN_INFO_TITLE");
		this.Description = LocalizeHelper.Translate("NOFRIENDTIPS");
		this.Buttons = 1;
	}
}

public class KickErrorLevel : DialogInfo {

	public KickErrorLevel()
	{
		this.Description = LocalizeHelper.Translate("KICKERRORLEVEL");
		this.Title = LocalizeHelper.Translate("GUEST_LOGIN_INFO_TITLE");
		this.Buttons = 1;
	}
}

public class KickErrorLimit : DialogInfo {

	public KickErrorLimit()
	{
		this.Description = LocalizeHelper.Translate("KICKERRORLIMAT");
		this.Title = LocalizeHelper.Translate("GUEST_LOGIN_INFO_TITLE");
		this.Buttons = 1;
	}
}

public class KickErrorOwner : DialogInfo {

	public KickErrorOwner()
	{
		this.Description = LocalizeHelper.Translate("KICKERRORONER");
		this.Title = LocalizeHelper.Translate("GUEST_LOGIN_INFO_TITLE");
		this.Buttons = 1;
	}
}

public class KickErrorPlaying : DialogInfo {

	public KickErrorPlaying()
	{
		this.Description =  LocalizeHelper.Translate("KICKERRORPLAYING");
		this.Title = LocalizeHelper.Translate("GUEST_LOGIN_INFO_TITLE");
		this.Buttons = 1;
	}
}