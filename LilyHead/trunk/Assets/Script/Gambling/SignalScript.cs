using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using LilyHeart;

public class SignalScript : MonoBehaviour {
	
	public int signalValue = 0;
	// Use this for initialization
	void Start () {
		gameObject.transform.FindChild("noSignal").gameObject.SetActiveRecursively(false);
	}
	
	// Update is called once per frame
	void Update () {
		if (PhotonClient.Singleton != null)
		{
			int temp= (int)PhotonClient.Singleton.signalValue;
			if(signalValue !=temp)
			{
				signalValue = temp;
				UpdateSignal();
			}
		}
	}
	
	void UpdateSignal()
	{
		if (signalValue > 0)
		{
			for (int i = 1; i < 6; i++)
			{
				if (i < signalValue)
				{
					gameObject.transform.FindChild(i.ToString()).gameObject.SetActiveRecursively(false);
				}
				else
				{
					gameObject.transform.FindChild(i.ToString()).gameObject.SetActiveRecursively(true);
				}
			}
			
			if (signalValue > 5)
					gameObject.transform.FindChild("noSignal").gameObject.SetActiveRecursively(true);
				else
					gameObject.transform.FindChild("noSignal").gameObject.SetActiveRecursively(false);
		}
	}
}
