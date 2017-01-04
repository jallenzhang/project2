using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using DataPersist;
using System.Collections.Generic;
using LilyHeart;
public class SpeakTip : MonoBehaviour {
	
	public string message=null;
	public static string mes=string.Empty;
  	public UITextList textlist;
	
	
	
	// Use this for initialization
	void Start () {
	    
		if(!string.IsNullOrEmpty(mes))
		{
			textlist.Add(mes);
		}
		
//	 	CTUIinput input =  transform.FindChild("Speak_Gambling_input").GetComponent<CTUIinput>();
//	 	input.selected = true;
		
		PhotonClient.BroadcastMessageInTableEvent+=DidBroadcastMessageInTableEvent;
 		
	}
	void OnDestroy()
	{
		//Debug.LogWarning("OnDes  "+index);
		PhotonClient.BroadcastMessageInTableEvent-=DidBroadcastMessageInTableEvent;
	}
	void DidBroadcastMessageInTableEvent(int nosear,string message)
	{	
		if(nosear<0)
			return;
		
	 
		TableInfo info=Room.Singleton.PokerGame.TableInfo;
		if(info!=null)
		{
			PlayerInfo iteminfo=info.GetPlayer(nosear);
			if(iteminfo!=null)
			{
 				publishMessage();	
			}
		}
 		
	}
	// Update is called once per frame
	void Update () {
	
	}
	
	void btnClose()
	{
		emptyInput();
		if(Util.isMatch())
		{
			MatchInforController.Singleton.DisAbleAllButtons(true);
		}
		else
		{
			ActorInforController.Singleton.DisAbleAllButtons(true);

		}
		Destroy(gameObject);
	}
	
	void setLabelText(string name,string text)
	{
		Transform label1=transform.FindChild(name);
        UILabel uilabel=label1.GetComponent<UILabel>();
		uilabel.text=text;
	}
	
	string getLabelText(string name)
	{
		Transform label1=transform.FindChild(name);
        UILabel uilabel=label1.GetComponent<UILabel>();
		return uilabel.text ;
 	}
	void emptyInput()
	{
		Transform label1=transform.FindChild("Speak_Gambling_input/Label");
		UILabel uil= label1.GetComponent<UILabel>();
		uil.text="Input your message";
	}
	
	void OnSubmit()
	{
	  
		string str=getLabelText("Speak_Gambling_input/Label");
	
 		//Debug.Log(str);
		if(str!="Input your message" && str.Length>0)
		{
			//Debug.Log(str);
			User.Singleton.BroadcastMessageInTable(str);
			StatisticsHelper.LogCallInTableEvent();
		 	//str="[38f8ff]"+User.Singleton.UserData.Name+"[-]:"+str;
  			//addMessage(str);
			//textlist.Add(str);
			Invoke("emptyInput",0.2f);
			
 		}
		 
	}
	
  	
	void publishMessage()
	{
		//Debug.Log(mes);
		 textlist.Clear();
		textlist.Add(mes);
		//Debug.Log(mes);
 		
	}
	
	void btnMessage(GameObject btn)
	{
		//uilabel.text="[38f8ff]"+"diyihai"+"[-]:";
		 message=getLabelText("WindowRoot/UIPanel-ViewUI/UIGrid/"+btn.name+"/Label");
		
		User.Singleton.BroadcastMessageInTable(message);

		StatisticsHelper.LogCallInTableEvent(message);
		//message="[38f8ff]"+User.Singleton.UserData.Name+"[-]:"+message;
  		//addMessage(message);
 	}
}
