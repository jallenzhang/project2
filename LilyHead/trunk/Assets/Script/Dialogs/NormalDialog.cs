using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using LilyHeart;

public class NormalDialog : MonoBehaviour {

	private DialogInfo info = null;
	void Start()
	{
		if(GlobalScript.ScriptSingleton.CurrentInfos.Count>0)
		{
			info = GlobalScript.ScriptSingleton.CurrentInfos.Dequeue();
			if (info != null)
			{
				Transform trs_description = gameObject.transform.FindChild("Label");
				UILabel label = trs_description.gameObject.GetComponent<UILabel>();
				label.text = info.Description;
				
				Transform trs_title = gameObject.transform.FindChild("Label_title");
				UILabel label_title = trs_title.gameObject.GetComponent<UILabel>();
				label_title.text = info.Title;
				
				if (info.Buttons == 2)
				{
					Transform trs_ok = gameObject.transform.FindChild("Button_ok");
					trs_ok.gameObject.SetActiveRecursively(false);
				}
				else if(info.Buttons == 1)
				{
					Transform trs_blue = gameObject.transform.FindChild("Button_blue");
					if(trs_blue!=null)
					{
						trs_blue.gameObject.SetActiveRecursively(false);
					}
					
					Transform trs_red = gameObject.transform.FindChild("Button_red");
					if(trs_red!=null)
					{
						trs_red.gameObject.SetActiveRecursively(false);
					}
				}
				else
				{
					Transform trs_blue = gameObject.transform.FindChild("Button_blue");
					if(trs_blue!=null)
					{
						trs_blue.gameObject.SetActiveRecursively(false);
					}
					
					Transform trs_red = gameObject.transform.FindChild("Button_red");
					if(trs_red!=null)
					{
						trs_red.gameObject.SetActiveRecursively(false);
					}
					
					Transform Button_ok = gameObject.transform.FindChild("Button_ok");
					if(Button_ok!=null)
					{
						Button_ok.gameObject.SetActiveRecursively(false);
					}
				}
			}
		}
	}
	
	void onCancel()
	{
		info.CancelProcess();
		onClose();
	}
	
	void onClose()
	{
		fadePanel panel = gameObject.GetComponent<fadePanel>();
		panel.fadeOut(null);
		
		//((Player)User.Singleton).CurrentInfos = null;
		User.Singleton.MessageOperating = false;
	}
	
	void onOK()
	{
		info.Process();
		onClose();
	}
	
	void Update()
	{
	}
}

public class DialogInfo
{
	public DialogInfo()
	{
	}
	
	public string Title {get;protected set;}
	public string Description{get;protected set;}
	public int Buttons{get;set;}
	
	public virtual void Process()
	{
	}
	
	public virtual void CancelProcess()
	{
	}
	
}
