
using System;
using UnityEngine;
using LilyHeart;

public class ChampionBar:MonoBehaviour
{
	public UILabel labFirstBounds; 
	public UILabel labSenondBounds; 
	public UILabel labJoinSpend; 
	public UILabel labServiceSpend; 
	public GameObject  btnJoinChampion;
	public GameObject  objectBlackMask;
	
	public ChaBarData barsChaBarData{set;get;}
	
	public int nLevel =  -1;
	// Use this for initialization
	void Start () {
	}
	// Update is called once per frame
	void Update () {
	}
	public void UpdateFace(ChaBarData data )
	{
		if(data == null)
			return;
		barsChaBarData = data;
 		nLevel = data.nLevelIndex;
		SetLableText(labFirstBounds,data.fFirstBonus);
		SetLableText(labSenondBounds,data.fSecondBonus);
		SetLableText(labJoinSpend,data.fJoinSpend);
		SetLableText(labServiceSpend,data.fServiceSpend);
		
		if(objectBlackMask!=null)
		{
			if(User.Singleton.UserData.Chips<data.fJoinSpend+data.fServiceSpend)
			{
				if(btnJoinChampion!=null)
				{
					UISlicedSprite sprite = btnJoinChampion.GetComponentInChildren<UISlicedSprite>();
					if(sprite!=null)
						sprite.color = Color.gray;
					BoxCollider box = btnJoinChampion.GetComponent<BoxCollider>();
					if(box!=null)
						box.enabled = false;
				}
				objectBlackMask.SetActiveRecursively(true);
			}
			else
			{
				objectBlackMask.SetActiveRecursively(false);
			}
		}
	}
	private void SetLableText(UILabel lab,float fNum)
	{
		string text = fNum >= 100 ? string.Format("{0:0,00}",fNum) : fNum.ToString();
		if(lab!=null)
		{
			lab.text = "$"+text;
		}
	}
}