using System;
using AssemblyCSharp;
using DataPersist;
using LilyHeart;

public class DailySignInData
{
	public int [] nAwardGuest = {181,362,604,786,967,1209,1935};
	public int [] nAwardNormal = {362,725,1209,1572,1935,2419,3870};
	public int [] nAwardPay = {544,1088,1814,2358,2903,3628,5806};
	public string DailyStatus = "0|1|1|0|1|1|2|3|3";//Guest 1//None 0//Normal 2//Pay 3
	public int [] nAwardTypeAry = new int [7]{-1,-1,-1,-1,-1,-1,-1};
	public float [] fProportion = new float [7]{-1f,-1f,-1f,-1f,-1f,-1f,-1f};
	
	public int nLastDay = 0;
	public int nStartDay = 1;
	public bool bIsCanGetReward = false;
	public  bool bIsOpenKangXi = false;
	private float fLastProp =  -2f;
	private int nLastUserType = -2;
	public const int NDayNumber = 7;
	
	public DailySignInData()
	{
	}
	public void DataInit()
	{
		nStartDay = 1;
		nLastDay = 0;
		GetDailyStatus();
		fLastProp = -2f;
		nLastUserType = -2;
	    //SetDilyStatus("10,0.0|10,0.0|20,0.0|30,0.0|30,0.0|30,0.0");
	}
	public  void GetDailyStatus()
	{
		if(User.Singleton != null)
		{
			if(User.Singleton.UserData!=null&&User.Singleton.UserData.Awards!=null)
				SetDilyStatus(User.Singleton.UserData.Awards);
			bIsOpenKangXi = User.Singleton.UserData.Jade;
		}
	}
	private int GetOriginal(int nUserType,int nDayIndex)
	{
		int nValue = -1;
		if(nUserType == 0) nUserType = 10;
		if(nUserType<0&&nUserType>3) 
			return -2;
		if(nDayIndex<0&&nDayIndex>=NDayNumber)
			return -3;
		switch(nUserType)
		{
		case 1:
		case 10:
			nValue = nAwardGuest[nDayIndex];
			break;
		case 2:
		case 20:
			nValue = nAwardNormal[nDayIndex];
			break;
		case 3:
		case 30:
			nValue = nAwardPay[nDayIndex];
			break;
		default:break;
		}
		return nValue;
	}
	public int GetDisplayNumber(int nIndex)
	{
		int nDisply = 0;
		float fResult = 0;
		int nDayType = 0;
		float fDayProportion = 0f;
		
		if(fProportion[nIndex]!=-1)
			fDayProportion = fProportion[nIndex];
		else
		{
			if(fLastProp!=-2)
			{
				if(bIsOpenKangXi)
					fLastProp = 0.2f;
				else
					fLastProp = 0f;
				fDayProportion = fLastProp;
			}
			else
				fDayProportion = 0f;
		}
		if(fProportion[nIndex]!=-1)
			fLastProp = fProportion[nIndex];

		if(nAwardTypeAry[nIndex]!=-1)
			nDayType = nAwardTypeAry[nIndex];
		else
		{
			if(nLastUserType !=-2)
			{
				nDayType = nLastUserType;
				if(User.Singleton.UserData.UserType != UserType.Guest)
					nDayType = 2;
				if(User.Singleton.UserData.VIP>0)
					nDayType = 3;
			}
			else
				nDayType = 1;
		}
		if(nAwardTypeAry[nIndex]!=-1)
			nLastUserType = nAwardTypeAry[nIndex];
		
		if(fDayProportion >20) fDayProportion = 1;
		
		fResult = GetOriginal(nDayType,nIndex) * (fDayProportion+1f);

		nDisply = (int)fResult;
		return nDisply;
	}
	public void SetDilyStatus(String strDailyStatus)
	{
		this.DailyStatus = strDailyStatus;
		TransfoDailyStatus(this.DailyStatus);
	}
	private void TransfoDailyStatus(string str)
	{
		if(str == null &&str.Length == 0)
			return ;
		if(str.IndexOf('|') == -1)
		{
			if(str.IndexOf(',') == -1) return;
			string [] sStringTmpAry = str.Split(new char[]{','});
			if(sStringTmpAry.Length!=2) return;
			nAwardTypeAry[0] = Convert.ToInt32(sStringTmpAry[0]);
			nLastDay = 1;
			if(Convert.ToInt32(sStringTmpAry[0])<4)
			{
				nStartDay = 1;
				bIsCanGetReward = true;
			}
			else
			{
				nStartDay = 2;
				bIsCanGetReward = false;
			}
			fProportion[0] = Convert.ToSingle(sStringTmpAry[1]);
		}
		else
		{
			string [] myDailyStringStatus = str.Split(new char[]{'|'});
			string s = string.Empty;
			nLastDay = myDailyStringStatus.Length;
			if(nLastDay >NDayNumber || nLastDay ==0) nLastDay =1;
			for(int i = 0;i<myDailyStringStatus.Length&&i<NDayNumber;i++)
			{
				s = myDailyStringStatus[i];
				if(s.IndexOf(',') == -1) return;
				string [] sStringTmpAry = s.Split(new char[]{','});
				if(sStringTmpAry.Length!=2) return;
				if(Convert.ToInt32(sStringTmpAry[0])>0&&Convert.ToInt32(sStringTmpAry[0])<31)
					nAwardTypeAry[i] = Convert.ToInt32(sStringTmpAry[0]);
				if(nAwardTypeAry[i]>9)
					nStartDay ++;
				else
				{
					if(!bIsCanGetReward)
						bIsCanGetReward = true;
				}
				fProportion[i] = Convert.ToSingle(sStringTmpAry[1]);
			}
		}
	}
	public void GetReward()
	{
		PhotonClient.Singleton.GetAward();
	}
}

