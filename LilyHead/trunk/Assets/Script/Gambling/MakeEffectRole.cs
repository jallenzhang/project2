using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using DataPersist;
using LilyHeart;
public class MakeEffectRole : MonoBehaviour {
	
		public int index=-1;
		public Vector3  localPosition1=new Vector3(1,-160.2f,0);
		public Vector3  localPosition2=new Vector3(-237,-160.2f,0);
		public Vector3  localPosition3=new Vector3(-406,-73,0);
		public Vector3  localPosition4=new Vector3(-406,138,0);
		public Vector3  localPosition5=new Vector3(-140,220,0);
		public Vector3  localPosition6=new Vector3(147,220,0);
		public Vector3  localPosition7=new Vector3(408,138,0);
		public Vector3  localPosition8=new Vector3(408,-73,0);
		public Vector3  localPosition9=new Vector3(242,-160,0);
 
	 
		public Vector3  matchlocalPosition1=new Vector3(1,-160.2f,0);
		public Vector3  matchlocalPosition2=new Vector3(-406,-42,0);
		public Vector3  matchlocalPosition3=new Vector3(-180,214,0);
		public Vector3  matchlocalPosition4=new Vector3(187,214,0);
		public Vector3  matchlocalPosition5=new Vector3(406,-42,0);

	// Use this for initialization
	void Start () {
 			
		Vector3 locateposition=(Vector3)this.GetType().GetField((Util.isMatch()?"matchlocalPosition":"localPosition")+index).GetValue(this);
		transform.localPosition=locateposition;
		transform.localScale=new Vector3(163,208,1);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnDestroy()
	{
//	    TableInfo infor= Room.Singleton.PokerGame.TableInfo;
//		if(infor!=null)
//		{
//			if(index==1 && User.Singleton.UserData.NoSeat != -1)
//			{
//				
//			}
//		}
	}
}
