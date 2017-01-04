using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using AssemblyCSharp.Helper;
using LilyHeart;

public class SpeakWorld : MonoBehaviour {
	
	public GameObject textList;
	// Use this for initialization
	void Start () {
	//	PhotonClient.BroadcastMessageEvent += BroadcastMessageFromWorld;
		StartCoroutine(NetworkUpdate());
		UITextList list = textList.GetComponent<UITextList>();
		list.Clear();
		
		string messageTitle = string.Empty;
		string lastSystemMessage = string.Empty;
		
		foreach(LilyBoardMessages message in GlobalManager.Singleton.BoardMessages)
		{
			if(message.kmMessagesType == WorldMessageType.SystemNotice)
			{
				messageTitle = message.Messages.Substring(9,2);
				Debug.Log("messageTitle:"+messageTitle);
				//judge if system notice
				if(!string.IsNullOrEmpty(messageTitle)&&messageTitle == LocalizeHelper.Translate("WORLD_SPEAK_SYSTEM_NOTICE_TITLE"))
				{
					if(!string.IsNullOrEmpty(lastSystemMessage))
					{
						if(lastSystemMessage == message.Messages)
							continue;
					}
					lastSystemMessage = message.Messages;
				}
			}
			list.Add(message.Messages);
		}
		
		UILabel label = textList.transform.FindChild("Label").GetComponent<UILabel>();
		label.lineWidth = 4;
	}
	
	IEnumerator NetworkUpdate()
	{
		PhotonClient.Singleton.Update();
		yield return null;
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	string getLabelText(string name)
	{
		Transform label1=transform.FindChild(name);
        UILabel uilabel=label1.GetComponent<UILabel>();
		return uilabel.text ;
 	}
	
	void ClearLabelString()
	{
		UILabel label = transform.FindChild("Label").GetComponent<UILabel>();
		label.text = string.Empty;
		UIInput input = transform.GetComponent<UIInput>();
		input.text = " ";
	}
	
	void OnSend()
	{
		string str = getLabelText("Label").Trim();
		if(str!=LocalizeHelper.Translate("PLEASE_INPUT_WHAT_YOU_WANT_TO_SAY") && str.Length>0 && str.Length < 60)
		{
			User.Singleton.BroadcastMessage(str);
			StatisticsHelper.LogCallInWorldEvent();
			UITextList list = textList.GetComponent<UITextList>();
			string boardMessage = string.Format(LocalizeHelper.Translate("SPEAK_WORLD"), User.Singleton.UserData.NickName, str);
			if(!string.IsNullOrEmpty(boardMessage))
				list.Add(boardMessage);
 		}
		Invoke("ClearLabelString", 0.2f);
	}
	public void BroadcastMessageFromWorld()
	{
		UITextList list = textList.GetComponent<UITextList>();
		string message = GlobalManager.Singleton.CurrentBoardMessage.Messages;
		if(!string.IsNullOrEmpty(message))
			list.Add(message);
	}
	void OnDestroy()
	{
		//PhotonClient.BroadcastMessageEvent -= BroadcastMessageFromWorld;
	}
}
