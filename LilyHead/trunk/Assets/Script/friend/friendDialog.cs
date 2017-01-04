using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using AssemblyCSharp.Helper;
using LilyHeart;

public class friendDialog : MonoBehaviour {
	
	public enum AvatorType:byte
	{
		Guest,
		DaHeng = 1,
		Songer,
		Captain,
		European,
		Qianjing,
		Dalaopo,
		Yitaitai,
		Luoli,
		AGe,
		General,//10
		Princess,
		Queen
	}
	
	public GameObject acceptBtn;
	public bool acceptBtnActived = true;
	public GameObject acceptBtnBackground;
	
	// Use this for initialization
	void Start () {
		
		if (!acceptBtnActived)
		{
			if (acceptBtn != null && acceptBtnBackground != null)
			{
				BoxCollider box = acceptBtn.GetComponent<BoxCollider>();
				box.enabled = false;
				UIButtonColor btnColor = acceptBtn.GetComponent<UIButtonColor>();
				btnColor.defaultColor = Color.gray;
				UISlicedSprite slicedSprite = acceptBtnBackground.GetComponent<UISlicedSprite>();
				slicedSprite.color = Color.gray;
			}
		}
		
		Transform[] trs = gameObject.GetComponentsInChildren<Transform>();
		
		foreach(Transform tr in trs)
		{
			UILabel label = tr.gameObject.GetComponent<UILabel>();
			UISprite sprite = tr.gameObject.GetComponent<UISprite>();
			if(User.Singleton.CurrentMessage is PlayerMessage)
			{
				PlayerMessage playerMessage=User.Singleton.CurrentMessage as PlayerMessage;
				switch(tr.gameObject.name)
				{
				case "Label_name":
					label.text = playerMessage.NickName;
					break;
				case "Level_Chip_Label":
					label.text = string.Format(LocalizeHelper.Translate("LEVEL_CHIP_PROMPT_MESSAGE"), 
						playerMessage.Level, FormatString(playerMessage.Chips));
					break;
				case "BehaviorLabel":
					label.text = LocalizeHelper.Translate(playerMessage.Title);
					break;
				case "HonorSprite":
					HonorType type = playerMessage.Honor;
					sprite.spriteName = HonorHelper.GetHonorPicString(type, playerMessage.Avator);
					break;
				case "SayLabel":
					label.text = GetTalkContent(playerMessage.Avator);			
					break;
				case "AvatorSprite":
					Debug.Log("fucking " + ((AvatorType)playerMessage.Avator).ToString());
					sprite.spriteName = ((AvatorType)playerMessage.Avator).ToString();
					break;
				}
			}
		}
	}
	
	string GetTalkContent(byte avator)
	{
		string result = string.Empty;
		switch(avator)
		{ 
		case (byte)PlayerAvator.Captain:
			result = LocalizeHelper.Translate("HAIDAO_TALK_PROMPT_MESSAGE");
			break;
		case (byte)PlayerAvator.DaHeng:
			result = LocalizeHelper.Translate("OIL_TALK_PROMPT_MESSAGE");
			break;
		case (byte)PlayerAvator.Dalaopo:
			result = LocalizeHelper.Translate("RUSSIA_TALK_PROMPT_MESSAGE");
			break;
		case (byte)PlayerAvator.European:
			result = LocalizeHelper.Translate("EURO_TALK_PROMPT_MESSAGE");
			break;
		case (byte)PlayerAvator.Luoli:
			result = LocalizeHelper.Translate("LOLI_TALK_PROMPT_MESSAGE");
			break;
		case (byte)PlayerAvator.Qianjing:
			result = LocalizeHelper.Translate("QIANJING_TALK_PROMPT_MESSAGE");
			break;
		case (byte)PlayerAvator.Songer:
			result = LocalizeHelper.Translate("BLACK_TALK_PROMPT_MESSAGE");
			break;
		case (byte)PlayerAvator.Yitaitai:
			result = LocalizeHelper.Translate("SONG_TALK_PROMPT_MESSAGE");
			break;
		case (byte)PlayerAvator.AGe:
			result = LocalizeHelper.Translate("AGE_SPEAK");
			break;
		case (byte)PlayerAvator.General:
			result = LocalizeHelper.Translate("NIANGENYAO_SPEAK");
			break;
		case (byte)PlayerAvator.Princess:
			result = LocalizeHelper.Translate("GEGE_SPEAK");
			break;
		case (byte)PlayerAvator.Queen:
			result = LocalizeHelper.Translate("NIANGNIANG_SPEAK");
			break;
		}
		
		return result;
	}
	
	IEnumerator NetworkUpdate()
	{
		PhotonClient.Singleton.Update();
		yield return null;
	}
	
	// Update is called once per frame
	void Update () {
		StartCoroutine(NetworkUpdate());
	}
	
	void OnClose()
	{
		fadePanel panel = gameObject.GetComponent<fadePanel>();
		panel.fadeOut(null);
		User.Singleton.MessageOperating = false;
	}
	
	void OnAccept()
	{
		if (User.Singleton.CurrentMessage is InviteFriendMessage && ((InviteFriendMessage)User.Singleton.CurrentMessage).DestinationType == InviteFriendMessage.DESTINATION_ROOM)
			transform.parent.parent.gameObject.AddComponent<ShowLoadingTable>();
		User.Singleton.CurrentMessage.ProcessMessage();
		OnClose();
	}
	
	void OnIgnore()
	{
		OnClose();
	}
	public string FormatString(long lvalue)
	{
		if (lvalue < 100)
			return lvalue.ToString();
		
		return string.Format("{0:0,00}",lvalue);
	}
}
