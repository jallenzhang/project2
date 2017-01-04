using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class PaySwitchScript : MonoBehaviour {
		
	public static string PaySubject;
	public static string PayBody;
	public static string PayPrice;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {		
	}
	
	public static void OnShowPaySwitchView(string gameObjectName, string subject, string body, string price){
#if UNITY_ANDROID
		PaySubject = subject;
		PayBody = body;
		PayPrice = price;
		WebBindingHelper.ShowPaySwitchWebView(gameObjectName, "PaySwitchFunction");
#endif		
	}
	
	void PaySwitchFunction(string payWay){
		if(payWay.Equals("Alipay")){
			GlobalScript.ScriptSingleton.UnityPayWay = DataPersist.PayWay.Alipay;				
		} else if(payWay.Equals("Yeepay")){
			GlobalScript.ScriptSingleton.UnityPayWay = DataPersist.PayWay.Yeepay;
		}
		
#if UNITY_ANDROID
			EtceteraAndroid.StartBuy(PaySubject, PayBody, PayPrice);
#endif
	}
}
