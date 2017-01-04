using System;
using UnityEngine;
using AssemblyCSharp;
using DataPersist;
using System.Collections;
using AssemblyCSharp.Helper;
using System.Collections.Generic;

/// <summary>  
/// Daily sign in for gamer get the reward.
/// </summary>  
/// <Author>John wu</Author>
/// <Email>wuxiaoming1733@sina.com</Email>
/// <Date>2012.08.13</Date>
/// <LastModify>John wu</LastModify>
/// <LastModifyDate>2012.08.15</LastModifyDate>
/// <Version>1.0</Version>
/// <value>Age of the claimant.</value>  
/// 
public class DailySignIn:MonoBehaviour
{
	private DailySignInData m_SignInData = null;
	public const int NDayNumber = 7;
	
	public GameObject  prefab = null;
	public AudioClip getAwardSound = null;
	public GameObject BtnKaiTongKangXi = null;
	public GameObject BtnLingQu = null;
	public Vector3 btnLingQulocalPosition;
	public Vector3 btnLingQuOpenKangXiPosition;
	public GameObject PromtLable = null;
	public Vector3 PromtLableAfterOpenKangXiPosition;
	
	public  List<Animation> listDayGameObject = new List<Animation> (NDayNumber);
	public List<GameObject>  UILableObjectList = new List<GameObject> (NDayNumber);
	
	
	private bool bIsClick = false;
	private GameObject item = null;
	private int nPlayIndex = 0;
	private  Animation [] aryAnimation = new Animation [NDayNumber];
	private string callWhenFinished = "PlayNextAnimaInSignIn";
	private bool [] bIsAnimaPlayedFinish =  new bool [NDayNumber];
	
	// Use this for initialization
	public void Start () {
		if(m_SignInData == null)
			m_SignInData = new DailySignInData ();
		m_SignInData.DataInit();
		Debug.Log("string:"+m_SignInData.DailyStatus);
		Debug.Log("bIsOpenKangXi:"+m_SignInData.bIsOpenKangXi);
		Debug.Log("Last Day:"+m_SignInData.nLastDay.ToString()+"start Day:"+m_SignInData.nStartDay.ToString());
		SetFaceBeforePop();
		for(int i=0;i<NDayNumber;i++)
		{
			bIsAnimaPlayedFinish[i] = false;
			aryAnimation[i] = null;
		}
		PlayAnimAfterGetReward();//Play the Get Reward Animation
	}
	// Update is called once per frame
	public void Update () {
	}
	public void btnGetRewardInfo()
	{
		if(!bIsClick)
		{
			if(m_SignInData.bIsCanGetReward)
			{
				m_SignInData.GetReward();//Get Reward to server through photonClient
				m_SignInData.GetDailyStatus();//Get the new Daily Status Set to sign in data 
			}
			StartCloseDailySignIn();
			if(this.transform.parent!=null)
				this.transform.parent.SendMessage("CloseBoxAfterPop",SendMessageOptions.DontRequireReceiver);
		}
		bIsClick = true;
	}
	public void btnOpenKangXi()
	{
		//Open Kang Xi Start
		Debug.Log("btnOpenKangXi");
	}
	private void PlayAnimAfterGetReward()
	{
		int iIndex = 0;
		if(listDayGameObject.Count>0)
		{
			for(int i = 0;i<NDayNumber&&listDayGameObject[i]!=null;i++)
			{
				Transform trs = listDayGameObject[i].transform;
				Animation dayAnimation = trs.gameObject.GetComponent<Animation>();
				aryAnimation[i] = dayAnimation;
			}
		}
		PlayFirstAnimaInSignIn();
	}
	private void PlayNextAnimaInSignIn()
	{
		bIsAnimaPlayedFinish[nPlayIndex] = true;
		nPlayIndex ++;
		if(nPlayIndex == m_SignInData.nLastDay||nPlayIndex<0||nPlayIndex>=NDayNumber)
			return;
		if(nPlayIndex>=0&&!bIsAnimaPlayedFinish[nPlayIndex]&&aryAnimation[nPlayIndex]!=null)
		{
			PlayAnimaByDayName("Day_"+nPlayIndex.ToString(),aryAnimation[nPlayIndex]);
		}
	}
	private void PlayFirstAnimaInSignIn()
	{
		nPlayIndex =  m_SignInData.nStartDay-1;
		if(!m_SignInData.bIsCanGetReward||nPlayIndex>=NDayNumber)
			return;
		if(nPlayIndex>=0&&!bIsAnimaPlayedFinish[nPlayIndex]&&aryAnimation[nPlayIndex]!=null)
		{
			PlayAnimaByDayName("Day_"+nPlayIndex.ToString(),aryAnimation[nPlayIndex]);
		}
	}
	private void PlayAnimaByDayName(string strName,Animation dayAnimation)
	{
		int nCurrentDay = Convert.ToInt32(strName.Substring(4));
		if(nCurrentDay >=0&&nCurrentDay<NDayNumber)
		{
			if(m_SignInData.nAwardTypeAry[nCurrentDay]!=0 &&dayAnimation!=null&&m_SignInData.nLastDay>=nCurrentDay-1)
				PlayAnimation(dayAnimation,"dayanim");
		}
	}
	private void  SetDailyAwardFace()
	{
		if(listDayGameObject.Count>0)
		{
			for(int i = 0;i<NDayNumber&&i<listDayGameObject.Count;i++)
			{
				if(listDayGameObject[i]!=null)
				{
					Transform trs = listDayGameObject[i].transform;
					if(i+1<m_SignInData.nStartDay)
					{
						HideDailyFlip(trs);
					}
				}
			}
		}
	}
	private void HideDailyFlip(Transform trs)
	{
		Transform[] alltranss = trs.transform.GetComponentsInChildren<Transform>();
		foreach(Transform trss in alltranss)
			{
				switch(trss.gameObject.name)
				{
				case "Sprite (Pic_1)":
					   trss.localRotation = new Quaternion (trss.localRotation.x,0,trss.localRotation.z,trss.localRotation.w);
					break;
				case "Sprite (Pic_2)":
					trss.rotation = new Quaternion (trss.localRotation.x,0,trss.localRotation.z,trss.localRotation.w);
					break;
				case "Sprite (Pic_3)":
					trss.rotation = new Quaternion (trss.localRotation.x,0,trss.localRotation.z,trss.localRotation.w);
					break;
				case "Sprite (Pic_card)":
					trss.gameObject.SetActiveRecursively(false);
					break;
				default:
					break;
				}
			}
	}
	private void StartCloseDailySignIn()
	{
		//StartCoroutine("IEnumCloseDailySignIn");
		CloseDailySignIn();
	}
	
	private IEnumerator IEnumCloseDailySignIn()
	{
		//if(CloseDelayTime<0f) CloseDelayTime = 0.5f;
		yield return new  WaitForSeconds( 0 );
		CloseDailySignIn();
		StopCoroutine("IEnumCloseDailySignIn");
	}
	public void CloseDailySignIn()
	{
			if(prefab!=null)
			{
				fadePanel iFadePanle =  prefab.transform.GetComponent<fadePanel>();
				if(iFadePanle!=null)
					iFadePanle.fadeOut(null);
			}
	}
	private void PlayAnimation (Animation target,string strClipName)
	{
	    if(getAwardSound != null)
			NGUITools.PlaySound(getAwardSound);//play audio <publiccardsopen>
		if(strClipName == null) strClipName = "dayanim";
		if (target != null)
		{
			ActiveAnimation anim = ActiveAnimation.Play(target,strClipName,AnimationOrTween.Direction.Forward,AnimationOrTween.EnableCondition.DoNothing,AnimationOrTween.DisableCondition.DoNotDisable);
			// Set the delegate
			//anim.onFinished = onFinished;
			// Copy the event receiver
			if (prefab != null && !string.IsNullOrEmpty(callWhenFinished))
			{
				anim.eventReceiver = prefab;
				anim.callWhenFinished = callWhenFinished;
			}
			else anim.eventReceiver = null;
		}
	}
	private void  SetFaceBeforePop()
	{
		SetDailyAwardFace();//Set the poker flip by the sign in reward Status [Already Get] 
		SetKangXiLableShow();//Display KangXI and Hide  by isKangXiHide or m_SignInData.bIsOpenKangXi
		SetRewardData();//Display Reward data in sign in.
		SetLingQuBtnPosition();//Set LingQu Btn position
		m_SignInData.DataInit();//Data Init
	}
	private void SetRewardData()
	{
		int nIndex = 0;
		foreach(GameObject trs in UILableObjectList)
		{
			UILabel UIRewardLable = trs.transform.GetComponent<UILabel>();
			
			if(UIRewardLable!=null)
			{
				Debug.Log(m_SignInData.GetDisplayNumber(nIndex).ToString());
				UIRewardLable.text = "+"+m_SignInData.GetDisplayNumber(nIndex).ToString();
			}
			nIndex++;
	 	 }
	}
	private void  SetKangXiLableShow()
	{
		if(PromtLable!=null)
		{
			Transform[] alltrans=PromtLable.GetComponentsInChildren<Transform>();
			foreach(Transform trs in alltrans)
			{
				if(trs.gameObject.name == "Sprite (TableBtn_dragon)")
				{
					if(m_SignInData.bIsOpenKangXi)
					{
						trs.localPosition = new Vector3 (trs.localPosition.x+0.4f,trs.localPosition.y,trs.localPosition.z);
						break;
					}
				 }
		 	 }
		}
		if(BtnKaiTongKangXi!=null)
		{
			BtnKaiTongKangXi.SetActiveRecursively(!m_SignInData.bIsOpenKangXi);
		}
		if(PromtLable!=null)
		{
			UILabel promtLable = PromtLable.GetComponent<UILabel>();
			if(promtLable!=null)
			{
				promtLable.text = "";
				string  strKangXi = LocalizeHelper.Translate("SIGNIN_DIALOG_KANGXI_MESSAGE");
				promtLable.transform.localPosition = new Vector3 (-61,-53,promtLable.transform.position.z);
				if(m_SignInData.bIsOpenKangXi)
				{
					strKangXi = LocalizeHelper.Translate("SIGNIN_DIALOG_OPEN_KANGXI_MESSAGE");
					promtLable.transform.localPosition = new Vector3 (0,-53,promtLable.transform.position.z);
				}
				if(PromtLableAfterOpenKangXiPosition!=Vector3.zero)
					promtLable.transform.localPosition = PromtLableAfterOpenKangXiPosition;
				promtLable.text = strKangXi;
			}
		}
	}
	private  void SetLingQuBtnPosition()
	{
		if(BtnLingQu!=null)
		{
			if(btnLingQulocalPosition != Vector3.zero)
			{
				BtnLingQu.transform.localPosition = btnLingQulocalPosition;
				if(m_SignInData.bIsOpenKangXi&&btnLingQuOpenKangXiPosition != Vector3.zero )
					BtnLingQu.transform.localPosition = btnLingQuOpenKangXiPosition;
			}
		}
	}
}

