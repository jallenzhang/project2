using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;
using AssemblyCSharp.Helper;
public class ActorsChooseTable : MonoBehaviour {

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
	public string CurrentAvator = string.Empty;
	
	public GameObject Item;

	void Start ()
	{
		addItems();
		Reposition();
	}

	void Update ()
	{
		if (repositionNow)
		{
			repositionNow = false;
			Reposition();
		}
		
		//PhotonClient.Singleton.Update();
	}
	// string [] str={"ggg","fsf"};
	/// <summary>
	/// Shows the actor infor.
	/// </summary>
	void showActorInfor(GameObject actor,int flag)
	{
		Transform[]  trs=actor.GetComponentsInChildren<Transform>();
		foreach(Transform t in trs)
		{
			if(t.name=="Sprite (STMissy)")
			{
				UISprite sprite=t.gameObject.GetComponent<UISprite>();
				switch(flag)
				{
					case 1:
						sprite.spriteName="STOiltycoon";
 						actor.tag = "1";
				 		 break;
					case 2:
						sprite.spriteName="STPirate";
 						//t.localPosition  =new Vector3(-67.49998f,57.9119f,0.0f);
						actor.tag = "3";
						break;
					case 3:
						sprite.spriteName="STPrince";
 						actor.tag = "4";
						break;
					case 5:
						sprite.spriteName="STSongDynastyWomen";
 						actor.tag = "7";
						break;
					case 4:
						sprite.spriteName="STWealthyfirstwife";
 						actor.tag = "6";
						break;
					case 6:
						sprite.spriteName="STBlack";
 						actor.tag = "2";
						break;
					case 7:
						sprite.spriteName="STloli";
 						actor.tag = "8";
						break;			
				}
				t.transform.localScale = new Vector3(226, 340, 1);
			}
			else if (t.name == "FGLabel" || t.name == "BGLabel")
			{
				UILabel label = (UILabel)t.gameObject.GetComponent<UILabel>();
				switch(flag)
				{
					case 1:
						label.text = LocalizeHelper.Translate("NICKNAME_OIL");
				 		 break;
					case 2:
						label.text = LocalizeHelper.Translate("NICKNAME_PIRATE");
						break;
					case 3:
						label.text = LocalizeHelper.Translate("NICKNAME_PRINCE");
						break;
					case 5:
						label.text = LocalizeHelper.Translate("NICKNAME_SONG_WOMEN");
						break;
					case 4:
						label.text = LocalizeHelper.Translate("NICKNAME_WEALTHYWIFE");
						break;
					case 6:
						label.text = LocalizeHelper.Translate("NICKNAME_BLACK");
						break;
					case 7:
						label.text = LocalizeHelper.Translate("NICKNAME_LOLI");
						break;
				}
			}
		}
 		
	}
	
	/// <summary>
	/// Adds the items.
	/// </summary>/
	void addItems()
	{
		//UIAtlas atlas=Resources.Load();
		for(int i=0;i<7;i++)
		{
 		   GameObject item=Instantiate(Item) as GameObject;
		   item.name="actor_role";
  		   item.transform.parent=transform;
 		   item.transform.localScale=new Vector3(1f,1f,1f);
		   showActorInfor(item,i+1);
			
   		}
		
		Reposition ();
	}
	/// <summary>
	/// Clear Other actors State when press One Actor.
	/// </summary>
	/// <param name='obejct'>
	/// Obejct.
	
	void clearBtnState()
	{
		Transform[] allTrans = gameObject.GetComponentsInChildren<Transform>();
		foreach(Transform t in allTrans)
		{
			
			if(t.name=="actor_role" || t.name == "item")
			{
		       Transform[]  trs=t.GetComponentsInChildren<Transform>();
		       foreach(Transform t1 in trs)
		       {
			        showMaskPic(t1.gameObject,false);
		        }
			}
		}
		  
	}
	/// <summary>
	/// Shows the mask pic.
	/// </summary>
	/// <param name='ob'>
	/// Ob.
	/// </param>
	/// <param name='flag'>
	/// Flag.
	/// </param>
	void showMaskPic(GameObject ob,bool flag)
	{
		
		if(ob.name == "SlicedSprite (CRPic_3)" )
			{
			    UISprite sprite = ob.GetComponent<UISprite>();
				sprite.enabled=flag;
			}
			
			if(ob.name == "Normal" )
			{
 				UISprite sprite = ob.GetComponent<UISprite>();
		       sprite.spriteName=flag?"CRChooseBtn":"CRBtnRed";
 			}
	}
	
	/// <summary>
	/// Presseds btn Show Mask picture.
	/// </summary>
	/// <param name='obejct'>
	/// Obejct.
	/// </param>
 
	void PressedBtn(GameObject obejct)
	{
 	 	clearBtnState();
		CurrentAvator = obejct.tag;
		Debug.Log("aaaaaa CurrentAvator: " + CurrentAvator);
		Transform[]  trs=obejct.GetComponentsInChildren<Transform>();
		foreach(Transform t in trs)
		{
			showMaskPic(t.gameObject,true);
		}
	}
	
	static public int SortByName (Transform a, Transform b) { return string.Compare(a.name, b.name); }

	public void Reposition ()
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
