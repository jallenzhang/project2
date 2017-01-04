using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class ChangeBackGroundSence : MonoBehaviour {
	
	 /// <summary>
	 /// Sets the back ground.
	 /// </summary>
	 /// <param name='tile'>
	 /// Tile.
	 /// </param>
	 /// 
	 /// 
	void setBackGround(string tile,UIAtlas atlas)
	{
 		GameObject roombg=GameObject.Find("RoomBg/Camera/Anchor/Panel/RoomBackGround");
		if(roombg)
		{
			UISprite spritebg=roombg.GetComponent<UISprite>();
	   		spritebg.atlas=atlas; 
			spritebg.spriteName=tile+"Bg";
		}
 		
		 
 	}
	/// <summary>
	/// Sets the back ground mask.
	/// </summary>
	/// <param name='tile'>
	/// Tile.
	/// </param>
	/// <param name='atlas'>
	/// Atlas.
	/// </param>
	void setBackGroundMask(string tile,UIAtlas atlas)
	{
		GameObject RoomBgMask=GameObject.Find("RoomBgMask/Camera/Anchor/Panel/RoomBackGroundMask");
		if(RoomBgMask)
		{
			UISprite spritebg=RoomBgMask.GetComponent<UISprite>();
	   		spritebg.atlas=atlas; 
			spritebg.spriteName=tile+"BgMask";
			
			RoomBgMask.transform.localScale = new Vector3(spritebg.sprite.outer.width, spritebg.sprite.outer.height, 1);
//			if(tile=="Egypt")
//			{
//				RoomBgMask.transform.localScale =new Vector3(960,640,1);
//		    }
 		}
	}
	/// <summary>
	/// Sets the room mask.
	/// </summary>
	/// <param name='tile'>
	/// Tile.
	/// </param>
	/// <param name='atlas'>
	/// Atlas.
	/// </param>
	void setRoomMask(string tile,UIAtlas atlas)
	{
		GameObject roommask=GameObject.Find("RoomMask/Camera/Anchor/Panel/SpriteRoomMask");
		if(roommask)
		{
			UISprite spritebg=roommask.GetComponent<UISprite>();
	   		spritebg.atlas=atlas; 
			spritebg.spriteName=tile+"RoomMask";
 
			roommask.transform.localScale = new Vector3(spritebg.sprite.outer.width, spritebg.sprite.outer.height, 1);
//			if(tile=="Egypt")
//			{
//				roommask.transform.localScale =new Vector3(585,331,1);
//		    }
		}
 
	}
	
	void setRoomChairsAndTable(string tile,UIAtlas atlas)
	{
		GameObject SeatbackPanel=GameObject.Find("Seatback/Camera/Anchor/Panel");
		if(SeatbackPanel)
		{
			Transform[] childs=SeatbackPanel.GetComponentsInChildren<Transform>();
 			foreach(Transform child in childs)
			{
				if(child != SeatbackPanel.transform)
				{
 					UISprite spritebg=child.GetComponent<UISprite>();
		   		    spritebg.atlas=atlas; 
				    spritebg.spriteName=tile+"SeatBack";
 					//spritebg.sprite.outer.width 
					child.localScale = new Vector3(spritebg.sprite.outer.width, spritebg.sprite.outer.height, 1);
//					if(tile=="Egypt")
//					{
//						child.localScale =new Vector3(79,103,1);
//					}
					 
				}
			}
 		}
		
		GameObject SeatfrontPanel=GameObject.Find("Seatfront/Camera/Anchor/Panel");
		if(SeatbackPanel)
		{
			Transform[] childs=SeatfrontPanel.GetComponentsInChildren<Transform>();
 			foreach(Transform child in childs)
			{
				if(child != SeatfrontPanel.transform)
				{
 					UISprite spritebg=child.GetComponent<UISprite>();
		   		    spritebg.atlas=atlas; 
				    spritebg.spriteName=tile+"SeatFront";
     				child.localScale = new Vector3(spritebg.sprite.outer.width, spritebg.sprite.outer.height, 1);
//					if(tile=="Egypt")
//					{
//						child.localScale =new Vector3(80,112,1);
//					}
					
				}
			}
 		}
		
		GameObject Table=GameObject.Find("Table/Camera/Anchor/Panel");
		if(SeatbackPanel)
		{
			Transform[] childs=Table.GetComponentsInChildren<Transform>();
 			foreach(Transform child in childs)
			{
				if(child != Table.transform)
				{
 					UISprite spritebg=child.GetComponent<UISprite>();
		   		    spritebg.atlas=atlas; 
				    spritebg.spriteName=tile+"Table";
					child.localScale = new Vector3(spritebg.sprite.outer.width, spritebg.sprite.outer.height, 1);
				 
//					if(tile=="Egypt")
//					{
//						child.localScale =new Vector3(360,233,1);
//					}
 				}
			}
 		}
	}
	
	UIAtlas getAtlas(string tile)
	{
		string path=string.Format("Game Lobby/{0}Atlas",tile);
 		GameObject loadaltalsObject=Resources.Load(path) as GameObject;
		return loadaltalsObject.GetComponent<UIAtlas>();
	}
	
	public void changeRoomBg(string result)
	{	
		if (result == "Common")
			result = "Community";
		UIAtlas atlas=getAtlas(result);
 	    setBackGround(result,atlas);
		setBackGroundMask(result,atlas);
		setRoomMask(result,atlas);
		setRoomChairsAndTable(result,atlas);
		
		MusicManager.Singleton.PlayBgMusic();
	}
	
	void Awake()
	{
	}
	
	// Use this for initialization
	void Start () {	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
