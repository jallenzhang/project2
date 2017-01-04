using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class PockerHanHua : MonoBehaviour {

	// Use this for initialization
	public int index=-1;
	public Vector3 Localposition1=new Vector3(0,0,0);
	public Vector3 Localposition2=new Vector3(0,0,0);
	public Vector3 Localposition3=new Vector3(0,0,0);
	public Vector3 Localposition4=new Vector3(0,0,0);
	public Vector3 Localposition5=new Vector3(0,0,0);
	public Vector3 Localposition6=new Vector3(0,0,0);
	public Vector3 Localposition7=new Vector3(0,0,0);
	public Vector3 Localposition8=new Vector3(0,0,0);
	public Vector3 Localposition9=new Vector3(0,0,0);
	public Vector3 LocalScale1=new Vector3(1,1,1);
	public Vector3 LocalScale2=new Vector3(1,1,1);
	public Vector3 LocalScale3=new Vector3(1,1,1);
	public Vector3 LocalScale4=new Vector3(1,1,1);
	public Vector3 LocalScale5=new Vector3(1,1,1);
	public Vector3 LocalScale6=new Vector3(1,1,1);
	public Vector3 LocalScale7=new Vector3(1,1,1);
	public Vector3 LocalScale8=new Vector3(1,1,1);
	public Vector3 LocalScale9=new Vector3(1,1,1);
	public Quaternion LocaleRotate1=Quaternion.Euler(0,0,0);
	public Quaternion LocaleRotate2=Quaternion.Euler(0,0,0);
	public Quaternion LocaleRotate3=Quaternion.Euler(0,0,0);
	public Quaternion LocaleRotate4=Quaternion.Euler(0,0,0);
	public Quaternion LocaleRotate5=Quaternion.Euler(0,0,0);
	public Quaternion LocaleRotate6=Quaternion.Euler(0,0,0);
	public Quaternion LocaleRotate7=Quaternion.Euler(0,0,0);
	public Quaternion LocaleRotate8=Quaternion.Euler(0,0,0);
	public Quaternion LocaleRotate9=Quaternion.Euler(0,0,0);
	
	
	public Vector3 Match_Localposition1=new Vector3(0,0,0);
	public Vector3 Match_Localposition2=new Vector3(0,0,0);
	public Vector3 Match_Localposition3=new Vector3(0,0,0);
	public Vector3 Match_Localposition4=new Vector3(0,0,0);
	public Vector3 Match_Localposition5=new Vector3(0,0,0);
 
	public Vector3 Match_LocalScale1=new Vector3(1,1,1);
	public Vector3 Match_LocalScale2=new Vector3(1,1,1);
	public Vector3 Match_LocalScale3=new Vector3(1,1,1);
	public Vector3 Match_LocalScale4=new Vector3(1,1,1);
	public Vector3 Match_LocalScale5=new Vector3(1,1,1);
 
	public Quaternion Match_LocaleRotate1=Quaternion.Euler(0,0,0);
	public Quaternion Match_LocaleRotate2=Quaternion.Euler(0,0,0);
	public Quaternion Match_LocaleRotate3=Quaternion.Euler(0,0,0);
	public Quaternion Match_LocaleRotate4=Quaternion.Euler(0,0,0);
	public Quaternion Match_LocaleRotate5=Quaternion.Euler(0,0,0);
 
	
	public string text;
	public GameObject hanhuabg=null;
	public GameObject label;
		
	void Start () {
			Vector3 Localposition=(Vector3)this.GetType().GetField((Util.isMatch()?"Match_Localposition":"Localposition")+index).GetValue(this);
		    Vector3 LocalScale=(Vector3)this.GetType().GetField((Util.isMatch()?"LocalScale":"LocalScale")+index).GetValue(this);
		    Quaternion LocaleRotate=(Quaternion)this.GetType().GetField((Util.isMatch()?"Match_LocaleRotate":"LocaleRotate")+index).GetValue(this);

		transform.localScale=LocalScale;
		transform.localPosition=Localposition;
		transform.localRotation=LocaleRotate;
		
  		int length=UtilityHelper.CalculationMessageLeghtIncludeChinese(text);
		
		if(Util.isMatch())
		{
			if(index==1||index==4||index==5)
			{
	   			hanhuabg.transform.localRotation= Quaternion.Euler(0,180,0);
	 		}
			else
			{
				hanhuabg.transform.localRotation= Quaternion.Euler(0,0,0);
	
			}
		}
		else
		{
 			if(index==1||index==6||index==7||index==8||index==9)
			{
	   			hanhuabg.transform.localRotation= Quaternion.Euler(0,180,0);
	 		}
			else
			{
				hanhuabg.transform.localRotation= Quaternion.Euler(0,0,0);
	
			}
		}
  		if(length>18 && length<=36 )
		{
 			//Debug.Log(" 2 line :"+text+" text.Length:"+text.Length);
			 hanhuabg.transform.localScale = new Vector3(168,49,1);
 		}
		else if(length<=18)
		{
			//hanhua.transform.localScale = new Vector3(168,35,1);
			 Debug.Log(" 1 line :"+text+" text.Length:"+text.Length);
		} 
		else
		{
			 hanhuabg.transform.localScale = new Vector3(168,70,1);
			 Debug.Log(" 3 line :"+text+" text.Length:"+text.Length);
		} 
 		UILabel uitext=label.GetComponent<UILabel>();
		uitext.text=text;
	
	}
	// Update is called once per frame
	void Update () {
	
	}
}
