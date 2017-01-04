using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.IO;
using System;

public class GiftGrid : MonoBehaviour {
	
	public GameObject clone;
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
	
	public static Dictionary<int, Gift> GiftDictionary = new Dictionary<int, Gift>();

	bool mStarted = false;
	
  	public int Id=-1;
	
	void Start ()
	{
		mStarted = true;
		
		addItems();
		Reposition();
	}
	void Awake()
	{
		if(GiftDictionary.Count==0)
				LoadGift();
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

	/// <summary>
	/// Recalculate the position of all elements within the grid, sorting them alphabetically if necessary.
	/// </summary>

	public void Reposition ()
	{
		if (!mStarted)
		{
			repositionNow = true;
			return;
		}

		Transform myTrans = transform;

		int x = 0;
		int y = 0;

		if (sorted)
		{
			List<Transform> list = new List<Transform>();

			for (int i = 0; i < myTrans.childCount; ++i)
			{
				Transform t = myTrans.GetChild(i);
				if (t) list.Add(t);
			}
			list.Sort(SortByName);

			for (int i = 0, imax = list.Count; i < imax; ++i)
			{
				Transform t = list[i];
				if (!t.gameObject.active && hideInactive) continue;

				float depth = t.localPosition.z;
				t.localPosition = (arrangement == Arrangement.Horizontal) ?
					new Vector3(cellWidth * x, -cellHeight * y, depth) :
					new Vector3(cellWidth * y, -cellHeight * x, depth);

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

				float depth = t.localPosition.z;
				t.localPosition = (arrangement == Arrangement.Horizontal) ?
					new Vector3(cellWidth * x, -cellHeight * y, depth) :
					new Vector3(cellWidth * y, -cellHeight * x, depth);

				if (++x >= maxPerLine && maxPerLine > 0)
				{
					x = 0;
					++y;
				}
			}
		}

		UIDraggablePanel drag = NGUITools.FindInParents<UIDraggablePanel>(gameObject);
		if (drag != null) drag.UpdateScrollbars(true);
	}
	
	void chooseGift(GameObject btn)
	{
		//Debug.Log(btn.name);
		Transform[] trs=transform.GetComponentsInChildren<Transform>();
		foreach(Transform tr in trs)
		{
			if(tr.name=="choose")
			{
				tr.gameObject.active=false;
			}
		}
		
		Transform choose =btn.transform.FindChild("choose");
		choose.gameObject.active=true;
		
		
		Transform sprite=btn.transform.FindChild("Sprite (Gift_1)");
		UISprite uisprite=sprite.GetComponent<UISprite>();
 		
		foreach(int key in GiftDictionary.Keys)
		{
			Gift gift=GiftDictionary[key];
			if(gift.icon_name==uisprite.spriteName)
			{
				Id=key;
				break;
			}
			//Debug.Log(gift.icon_name);
			//Debug.Log(gift.name);
 		}
 
 	}
	
	void SetBtnValue(Transform btn,Gift git)
	{
		Transform giticon=btn.FindChild("Sprite (Gift_1)");
		UISprite giftsprite=giticon.GetComponent<UISprite>();
		giftsprite.spriteName=git.icon_name;
		giticon.transform.localScale=new Vector3(giftsprite.sprite.outer.width,giftsprite.sprite.outer.height,1);
		
		Transform Label_nametr=btn.FindChild("Label_name");
		UILabel Label_name=Label_nametr.GetComponent<UILabel>();
		Label_name.text=git.name;
  		
		Transform Labeltr=btn.FindChild("Label");
		UILabel Label=Labeltr.GetComponent<UILabel>();
		if(git.price>=1000)
			Label.text=(git.price/1000).ToString()+"K";
		else 
		  	Label.text=git.price.ToString();

		Transform choose=btn.FindChild("choose");
		choose.gameObject.active=false;
	}
	
	
	void fillItem(Gift git1,Gift git2,Transform parent)
	{
		Transform btn1=parent.FindChild("Button_1");
		Transform btn2=parent.FindChild("Button_2");
		SetBtnValue(btn1,git1);
		SetBtnValue(btn2,git2);
		
		 
		
	}

	public void addItems()
	{
		{
			Gift git1=GiftDictionary[1];
			Gift git2=GiftDictionary[2];
			fillItem(git1,git2,clone.transform);
		}
		
		for(int i = 2; i < 17; i++)
		{
 			GameObject item = Instantiate(clone,new Vector3(0,0,0),Quaternion.identity) as GameObject;
			
			Gift git1=GiftDictionary[2*i-1];
			Gift git2=GiftDictionary[2*i];
  			item.name="item";
			item.transform.parent=transform;
 		    item.transform.localScale=new Vector3(1f,1f,1f);
		    item.transform.localPosition=new Vector3(0,0,0);
			
			
			fillItem(git1,git2,item.transform);
		}
	 
 		Reposition ();
	}
	
	public static void LoadGift()
	{
		TextAsset GiftAsset = (TextAsset)Resources.Load("GiftTable/Gifts");
		
		if (GiftAsset == null)
		{
			Debug.Log("Load honor.xml failed");
			return;
		}
		
		GiftGrid.LoadXmlFile(GiftAsset);
	}
	
	public static void LoadXmlFile(TextAsset resource)
	{
		byte[] encodedString = Encoding.UTF8.GetBytes(resource.text);

	    MemoryStream ms = new MemoryStream(encodedString);
	    ms.Flush();
	    ms.Position = 0;
		
		XmlDocument doc = new XmlDocument();
        doc.Load(ms);

        XmlNode root = doc.SelectSingleNode("gifts");
        XmlNodeList nodeList = root.ChildNodes;
 		foreach (XmlNode node in  nodeList)
		{
			Gift git = new Gift();
			
			foreach (XmlNode xn in node.ChildNodes)
			{
 				XmlElement xe = (XmlElement)xn;
				if (xe.Name == "id")
				{
					git.Id = Convert.ToInt32(xe.InnerText); 
				}
				if (xe.Name == "name")
				{
					git.name = (xe.InnerText); 
				}
				if (xe.Name == "icon_name")
				{
					git.icon_name =  xe.InnerText; 
				}
				if (xe.Name == "price")
				{
					git.price = Convert.ToInt32(xe.InnerText); 
				}
				if (xe.Name == "unity")
				{
					git.unity  = xe.InnerText; 
				}
 			}
 			GiftDictionary.Add(git.Id,git);
  		}
  	}
}

public class Gift
{
	public int Id
	{
		get;
		set;
	}
	
	public string name
	{
		get;
		set;
	}
	
	public string icon_name
	{
		get;
		set;
	}
	
	public int price
	{
		get;
		set;
	}
	
	public string unity
	{
		get;
		set;
	}
	
 }

