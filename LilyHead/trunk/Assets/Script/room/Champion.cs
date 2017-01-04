using System;
using UnityEngine;
using LilyHeart;
using DataPersist;
using AssemblyCSharp;

public class Champion:MonoBehaviour
{
	//define data for init
//	private int NGameLevelNumber = 6;
//	private float [] aryfFirstBonus ={8000f,20000f,40000f,200000f,400000f,2000000f};
//	private float [] aryfSecondBonus ={2000f,5000f,10000f,50000f,100000f,500000f};
//	private float [] aryfJoinSpend ={2000f,5000f,10000f,50000f,100000f,500000f};
//	private float [] aryfServiceSpend ={80f,150f,250f,750f,1000f,5000f};
	//end-define
	public GameObject ChampionGrid;
 	public float cellWidth = 200f;
	public float cellHeight = 80f;
	public GameGrade [] myGameGrades;
	
	public static ChaBarData barChaBarData{set;get;}
	
	// Use this for initialization
	void Start () {
		if(GlobalManager.Singleton.GameGrades != null)
		{
			Debug.Log("GlobalManager.Singleton.GameGrades != null");
			myGameGrades = GlobalManager.Singleton.GameGrades;
			InitItem();
		}
		else
		{
			Debug.Log("PhotonClient.Singleton.GetGameGrades()");
			PhotonClient.Singleton.GetGameGrades();
			gameObject.transform.parent.gameObject.AddComponent<MaskingTable>();
		}
	}
	public void DealNotificationGameGrades()
	{
		Debug.Log("DealNotificationGameGrades:");
		if(GlobalManager.Singleton.GameGrades != null)
		{
			Debug.Log("GlobalManager.Singleton.GameGrades != null:"+myGameGrades.Length);
			myGameGrades = GlobalManager.Singleton.GameGrades;
			InitItem();
			UtilityHelper.CloseMaskingTable();
		}
		else
		{
			PhotonClient.Singleton.GetGameGrades();
		}
	}
	
	// Update is called once per frame
	void Update () {
	}
	private void InitItem()
	{
		Debug.Log("InitItem:"+myGameGrades.Length);
		if(ChampionGrid != null&&myGameGrades!=null)
		{
			for(int i=0;i<myGameGrades.Length;i++)
			{
	 			GameObject prefab=Resources.Load("prefab/ChampionBar1") as GameObject;
				GameObject item = Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
				
				item.name = "Item_"+(myGameGrades[i].ID).ToString();
	 			item.transform.parent = ChampionGrid.transform;
				item.layer = ChampionGrid.layer;
				item.transform.localScale = new Vector3 (1,1,1);
				item.transform.localPosition = new Vector3(cellWidth - 940f, -cellHeight * i, 0f);
				
				ChampionBar chaBar = item.GetComponent<ChampionBar>();
				if(chaBar!=null)
				{
					ChaBarData data = new ChaBarData (myGameGrades[i].ID,myGameGrades[i].First,myGameGrades[i].Second,myGameGrades[i].Tickets,myGameGrades[i].Tip);
					chaBar.UpdateFace(data);
				}
				
				UIMyDragObject drag = item.GetComponent<UIMyDragObject>();
				if(drag!=null)
				{
					drag.target = ChampionGrid.transform;
				}
			}
		    foreach(UIButtonMessage  btnme in ChampionGrid.GetComponentsInChildren<UIButtonMessage>())
			{
				if(btnme.functionName=="btnJoinChampionMatch")
				{
					btnme.target=gameObject;
				}
			}
		}
	}
	void btnJoinChampionMatch(GameObject btn)
	{
	  // int i=btn.transform.parent.GetComponent<ChampionBar>().nLevel-1;
	  // barChaBarData = new ChaBarData (i+1,aryfFirstBonus[i],aryfSecondBonus[i],aryfJoinSpend[i],aryfServiceSpend[i]);
		barChaBarData = btn.transform.parent.GetComponent<ChampionBar>().barsChaBarData;
		transform.parent.parent.gameObject.AddComponent<ShowLoadingTable>();
		Application.LoadLevelAsync("GamblingInterface_game");
	}
	void CloseDialog(GameObject btn)
	{
		if (btn!=null)
		{
			BoxCollider box = btn.GetComponent<BoxCollider>();
			if(box!=null)
				box.enabled = false;
			Destroy(btn);
		}
		fadePanel panel = transform.GetComponent<fadePanel>();
		if(panel!=null)
			panel.fadeOut(null);
	}
}

public class ChaBarData{
	public ChaBarData(int nLevel,float FirstBonus,float SecondBonus,float JoinSpend,float ServiceSpend)
	{
		this.nLevelIndex = nLevel;
		this.fFirstBonus = FirstBonus;
		this.fSecondBonus = SecondBonus;
		this.fJoinSpend = JoinSpend;
		this.fServiceSpend = ServiceSpend;
	}
	public int  nLevelIndex;
	public float fFirstBonus;
	public float fSecondBonus;
	public float fJoinSpend;
	public float fServiceSpend;
}