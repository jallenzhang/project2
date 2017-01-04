using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using LilyHeart;

public class OnlinePeopleNum : MonoBehaviour {
	
	public string labelInitStr;
    public UILabel onlineUILable;
	public UILabel statusUILable;
	
	private string storeLabelStr;

	// Use this for initialization
	void Start () {
		storeLabelStr = statusUILable.text;
		statusUILable.text = labelInitStr;
		
        PhotonClient.Singleton.AskOnlinePeopleNumber();
        PhotonClient.Singleton.SetOnlinePeopleNumberAction += SetLableValue;
	}
	
	// Update is called once per frame
	void Update () {
	}

    void SetLableValue(int num) {
		statusUILable.text = storeLabelStr;
		
        onlineUILable.text = string.Format("{0:N0}", num);    
    }
}
