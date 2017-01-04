using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp.Helper;
using AssemblyCSharp;
using DataPersist;
using LilyHeart;

public class UIHGrid : MonoBehaviour {

	public enum Arrangement
	{
		Horizontal,
		Vertical,
	}

	public Arrangement arrangement = Arrangement.Horizontal;
	public int maxPerLine = 0;
	public float cellWidth = 200f;
	public float cellHeight = 200f;
	public bool repositionNow = false;
	public bool sorted = false;
	public bool hideInactive = true;
	
	
	public GameObject item0;
	public string type;
	
	// Use this for initialization
    void Start ()
	{
 	    addItems();
		Reposition();
	}
	
	private string GetString(HonorType i, int spirteKind)
	{
		string result = string.Empty;
		string upperString = i.ToString().ToUpper();
		string translateString = string.Empty;
		switch(spirteKind)
		{
		case 1:
			translateString = string.Format("HONOR_{0}_DESCRIPTION", upperString);
			result = LocalizeHelper.Translate(translateString);
			break;
		case 2:
			if (i == HonorType.Citizen)
				return result;
			translateString = string.Format("HONOR_{0}_DESCRIPTION_OTHER", upperString);
			result = LocalizeHelper.Translate(translateString);
			break;
		case 3:
			if(i == HonorType.Infante)
			{
				if(!HonorHelper.IsHonorMaleSex((int)User.Singleton.UserData.Avator))
					upperString = "PRINCESS";
			}
			if(i == HonorType.King)
			{
				if(!HonorHelper.IsHonorMaleSex((int)User.Singleton.UserData.Avator))
					upperString = "QUEUE";
			}
			translateString = string.Format("HONOR_{0}", upperString);
			result = LocalizeHelper.Translate(translateString);
			break;
		}
		return result;
	}
	
	private void DestroyAllItems()
	{
		Transform myTrans = transform;
		List<Transform> list = new List<Transform>();
		
		for (int i = 0; i < myTrans.childCount; ++i) 
			list.Add(myTrans.GetChild(i));
		
		for (int i = (myTrans.childCount - 1); i > -1; i--)
		{
			Destroy(list[i].gameObject);
		}
		//repositionNow = true;
		Reposition ();
	}
	
	private void DestroyItemById(int id)
	{
		Transform myTrans = transform;
		List<Transform> list = new List<Transform>();
		
		for (int i = 0; i < myTrans.childCount; ++i) 
			list.Add(myTrans.GetChild(i));
		
		for (int i = (myTrans.childCount - 1); i > -1; i--)
		{
			if (i == id)
				Destroy(list[i].gameObject);
		}
		repositionNow = true;
	}
	
	private void FillItem(int i, GameObject item)
	{
		Transform[] alltrans=item.GetComponentsInChildren<Transform>();
		
		UserData userData = null;
		
		if(User.Singleton.CurrentPlayInfo == 2)
			userData = Room.Singleton.RoomData.Owner;
		else
			userData = User.Singleton.UserData;
		
		foreach(Transform trs in alltrans)
		{
			UILabel label = (UILabel)trs.gameObject.GetComponent<UILabel>();
			UISprite slicedSprite = (UISprite)trs.gameObject.GetComponent<UISprite>();
			switch(trs.gameObject.name)
			{
			case "ChipsFGLabel":
				label.text = GetString((HonorType)i, 1);
				break;
			case "GameNumberLabel":
				label.text = GetString((HonorType)i, 2);
				break;
			case "HonorNameLabel":
				label.text = GetString((HonorType)i, 3);
				break;
			case "HonorSprite":
				slicedSprite.spriteName = HonorHelper.GetHonorPicString((HonorType)i, userData.Avator);
				break;
			case "Sprite (TITLable)":
				HonorType type = HonorHelper.GetHonorRecuise(userData, HonorType.Citizen);
				if ((int)type == i)
					trs.gameObject.SetActiveRecursively(true);
				else
					trs.gameObject.SetActiveRecursively(false);
				break;
			case "CurrentHonorLabel":
				break;
			}
		}
		
	}
	
	/// <summary>
	/// Adds the items.
	/// </summary>
	/// 
	/// 
	public void addItems()
	{
		for(int i = 1; i < 12; i++)
		{
			//GameObject prefab=Resources.Load("prefab/Long_Prefab") as GameObject;
			GameObject item = Instantiate(item0,new Vector3(0,0,0),Quaternion.identity) as GameObject;
			
			FillItem(i, item);
			
			item.transform.parent=transform;
 		    item.transform.localScale=new Vector3(1f,1f,1f);
		    item.transform.localPosition=new Vector3(0,0,0);
		}
		
		DestroyItemById(0);
		
 		Reposition ();
	}
	 
	 
	
	void Update ()
	{
		if (repositionNow)
		{
			repositionNow = false;
			Reposition();
		}
	}
	static public int SortByName (Transform a, Transform b) { return string.Compare(a.name, b.name); }

	public  void Reposition ()
	{
		Transform myTrans = transform;

		int x = 0;
		int y = 0;

		if (sorted)
		{
			List<Transform> list = new List<Transform>();

			for (int i = 0; i < myTrans.childCount; ++i) list.Add(myTrans.GetChild(i));
			list.Sort(SortByName);

			foreach (Transform t in list)
			{
				if (!t.gameObject.active && hideInactive) continue;

				t.localPosition = (arrangement == Arrangement.Horizontal) ?
					new Vector3(cellWidth * x, -cellHeight * y, 0f) :
					new Vector3(cellWidth * y, -cellHeight * x, 0f);

				if (++x >= maxPerLine && maxPerLine > 0)
				{
					x = 0;
					++y;
				}
			}
		}
		else
		{
			for (int i = 0; i < myTrans.childCount; ++i)
			{
				Transform t = myTrans.GetChild(i);

				if (!t.gameObject.active && hideInactive) continue;

				t.localPosition = (arrangement == Arrangement.Horizontal) ?
					new Vector3(cellWidth * x, -cellHeight * y, 0f) :
					new Vector3(cellWidth * y, -cellHeight * x, 0f);

				if (++x >= maxPerLine && maxPerLine > 0)
				{
					x = 0;
					++y;
				}
			}
		}
	}
	
}
