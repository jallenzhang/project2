using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;
using DataPersist;
using LilyHeart;

public class AchievementGrid : MonoBehaviour {

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
	public GameObject Panel;

    private List<GameObject> achivementBarList = new List<GameObject>();
    private List<byte> mainAchivement = new List<byte>();
    private string mainAchivementFormat = string.Empty;
    private Vector3 initPos;
	private int previousLeftBtn = -1;
	private bool isLeftBtnChanged = false;
	private int currentIndex = -1;
	private UserData userData = null;

    static public int SortByName(Transform a, Transform b) { return string.Compare(a.name, b.name); }

	void Start ()
	{
		if(User.Singleton.CurrentPlayInfo == 2)
			userData = Room.Singleton.RoomData.Owner;
		else
			userData = User.Singleton.UserData;
		
        SetMainAchivement();
		addItems(1);
        initPos = transform.localPosition;        
	}

	void Update ()
	{
		if (repositionNow)
		{
			repositionNow = false;
			Reposition();
		}
		
		if(isLeftBtnChanged){
			if(currentIndex != 1){ 
				StartCoroutine("WaitForSecondsToDo");
			} else {
				isLeftBtnChanged = false;
			}	
		}
	}
	
	IEnumerator WaitForSecondsToDo(){
		yield return new WaitForSeconds(2);
		StopCoroutine("WaitForSecondsToDo");
		ResetGridView();
		isLeftBtnChanged = false;
	}
	
    #region Btn Action

    void btnTotalAchievement()
    {		
        if(setbtnState(0)){
	        addItems(1);
	        ResetGridView();
		}
    }

    void btnGamblingAchivement()
    {
        if(setbtnState(1)){
	        addItems(2);
	        ResetGridView();
		}
    }

    void btnFriendAchivement()
    {
        if(setbtnState(2)){
	        addItems(3);
	        ResetGridView();
		}
    }

    void btnBack()
    {
        iTween.MoveTo(Panel, iTween.Hash("y", -2, "loopType", "none", "time", 0.2f, "easetype", "easeInOutQuad", "oncomplete", "destroyitem", "oncompletetarget", gameObject));
    }

    bool setbtnState(int btnIndex)
    {		
		currentIndex = btnIndex;
		if(previousLeftBtn == btnIndex) return false;
		previousLeftBtn = btnIndex;
		isLeftBtnChanged = true;
		
        Transform leftbtntrs = Panel.transform.FindChild("LeftBtn");
        string[] leftBtnsStr = new string[] { "Button_Comprehensive", "Button_friend", "Button_Gambling" };

        for (int i = 0; i < leftBtnsStr.Length; i++)
        {
            if (i != btnIndex)
            {
                UILabel uiOtherLabel = FindLeftBtnUILabel(leftbtntrs, leftBtnsStr[i]);
                uiOtherLabel.color = new Color(0.88f, 0.654f, 0.365f, 1);
            }
        }

        UILabel uiLabel = FindLeftBtnUILabel(leftbtntrs, leftBtnsStr[btnIndex]);
        uiLabel.color = Color.white;
		
		return true;
    }

    void destroyitem()
    {
        enableAllbtns();
        Destroy(Panel);
    }

    void enableAllbtns()
    {
        BoxCollider[] boxcoolides = Panel.transform.parent.gameObject.GetComponentsInChildren<BoxCollider>();
        foreach (BoxCollider one in boxcoolides)
        {
            one.enabled = true;
        }
    }

    #endregion

    #region Public Method

    public void addItems(int typeID)
    {
        ClearCurrentAchiements(typeID);

        List<Achievement> list = this.GetAchievementByTypeID(typeID);        
        for (int i = 0; i < list.Count; i++)
        {
            if (mainAchivement.Contains(list[i].Number))
            {
                setATCurrentlyPrefabValue(list[i]);
            }
            else
            {
                if (i == 0)
                {
                    setATOtherBarPrefabValue(item0.transform, list[i]);
                }
                else
                {
                    GameObject item = Instantiate(item0, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                    item.transform.parent = transform;
                    item.transform.localScale = new Vector3(1f, 1f, 1f);
                    item.transform.localPosition = new Vector3(0, 0, 0);
                    setATOtherBarPrefabValue(item.transform, list[i]);
                    achivementBarList.Add(item);
                }
            }
        }

        Reposition();
    }

    public void Reposition()
    {
        Transform myTrans = transform;

        int x = 0;
        int y = 0;

        if (sorted)
        {
            List<Transform> list = new List<Transform>();

            for (int i = 0; i < myTrans.childCount; ++i)
            {
                list.Add(myTrans.GetChild(i));
            }
            list.Sort(SortByName);

            foreach (Transform t in list)
            {
                RepositionTrans(t, ref x, ref y);
            }
        }
        else
        {
            for (int i = 0; i < myTrans.childCount; ++i)
            {
                Transform t = myTrans.GetChild(i);
                RepositionTrans(t, ref x, ref y);
            }
        }
    }

    public void RepositionTrans(Transform t, ref int x, ref int y)
    {
        if (!t.gameObject.active && hideInactive) return;

        t.localPosition = (arrangement == Arrangement.Horizontal) ?
            new Vector3(cellWidth * x, -cellHeight * y, 0f) :
            new Vector3(cellWidth * y, -cellHeight * x, 0f);

        if (++x >= maxPerLine && maxPerLine > 0)
        {
            x = 0;
            ++y;
        }
    }

    public void ResetGridView() {    
        transform.localPosition = initPos;
    }

    #endregion

    #region Private Method

    List<Achievement> GetAchievementByTypeID(int typeId) {
        Achievement[] list = GlobalManager.Singleton.Achievements;
        List<Achievement> res = new List<Achievement>();
        if (list != null) {
            for (int i = 0; i < list.Length; i++) {
                if (list[i].Type == typeId) {
                    res.Add(list[i]);
                }
            }
        }        
        
        return res;
    }

    void SetMainAchivement() {
        mainAchivement.Add(6);
        mainAchivement.Add(11);
        mainAchivement.Add(32);
    }

    UILabel FindLeftBtnUILabel(Transform leftbtntrs, string name)
    {
        Transform transBtn = leftbtntrs.FindChild(name);
        Transform transLabel = transBtn.FindChild("Label_a");
        UILabel uiLabel = transLabel.GetComponent<UILabel>();

        return uiLabel;
    }

    void setATCurrentlyPrefabValue(Achievement achiObj)
    {
        Transform atCurrently = transform.parent.parent.FindChild("Window").FindChild("ATCurrentlyBar");
        SetLabelName(atCurrently.FindChild("Label_1"), achiObj.Name, achiObj.Number);
        SetLabelName(atCurrently.FindChild("Label_2"), achiObj.Condition, achiObj.Number);
		// set label 3
        UILabel label3 = atCurrently.FindChild("Label_3").GetComponent<UILabel>();
        if(string.IsNullOrEmpty(mainAchivementFormat))
            mainAchivementFormat = label3.text;
        label3.text = string.Format(mainAchivementFormat, achiObj.Exp, achiObj.Chip);
		// set label 4
		UILabel label4 = atCurrently.FindChild("Label_4").GetComponent<UILabel>();
		label4.text = CaculateAchivePercent(achiObj.Number, achiObj.Type);
		// set icon
        SetIcon(atCurrently, "Sprite (ct_cj_011)", achiObj);		
	}

    void setATOtherBarPrefabValue(Transform trans, Achievement achiObj)
    {
        SetLabelName(trans.FindChild("Label_5"), achiObj.Name, achiObj.Number);
        SetLabelName(trans.FindChild("Label_6"), achiObj.Condition, achiObj.Number);
        SetIcon(trans, "Sprite (ct_cj_011)", achiObj);
		// set ATOther bar be gray
		UISprite sprite_ATOtherbar = trans.FindChild("Sprite (ATOtherBar)").GetComponent<UISprite>();
		GrayATOtherBar(achiObj.Number, sprite_ATOtherbar);
    }

    void SetLabelName(Transform trans, string pvalue, byte number)
    {
        UILabel label = trans.GetComponent<UILabel>();
        label.text = pvalue;
		HighLightAchivement(number, label);
    }

    void SetIcon(Transform trans, string childname, Achievement achiveObj)
    {
        UISprite sprite1 = trans.FindChild(childname).GetComponent<UISprite>();
        string sprintname = string.Format("ct_cj_{0}", GetAchievementNumber(achiveObj.Number));
        sprite1.spriteName = sprintname;
		HighLightAchivement(achiveObj.Number, sprite1);
    }

    void HighLightAchivement(byte pvalue, UISprite sprite1)
    {
		if(IsCompleteAchivement(pvalue))
			sprite1.alpha = 1;		
		else 
			sprite1.alpha = 0.393f;           
    }
	
	void HighLightAchivement(byte pvalue, UILabel label)
    {
		if(IsCompleteAchivement(pvalue))
			label.alpha = 1;			
		else 
			label.alpha = 0.393f;           
    }
	
	void GrayATOtherBar(byte pvalue, UISprite sprite)
    {
		if(!IsCompleteAchivement(pvalue)){
			sprite.color = new Color(0.76f, 0.76f, 0.76f);
		} else {
			sprite.color = new Color(1f, 1f, 1f);			
		}
    }
	
	string CaculateAchivePercent(byte achiveNumber, int type){
		Achievement[] list = GlobalManager.Singleton.Achievements;
		int total = 0;
		int acnt = 0;		
		for(int i=0; i<list.Length; i++){
			if(list[i].Type == type){
				total++;
				if(IsCompleteAchivement(list[i].Number)){
					acnt++;						
				}
			}
		}
		string res = acnt + "/" + total;
		
		return res;
	}
	
	private bool IsCompleteAchivement(byte number)
    {
		if(null == userData){
			return false;				
		}

		return (userData.Achievements & (1L << (number - 1))) == 1L << (number - 1);		
    }

    void ClearCurrentAchiements(int type) {
        for(int i=0; i<achivementBarList.Count; i++)
        {
            achivementBarList[i].transform.parent = null;
            Destroy(achivementBarList[i]);            
        }
        
        achivementBarList.Clear();
    }

    private string GetAchievementNumber(byte number)
    {
        int cnt = 3;
        int lenth = number.ToString().Length;
        string res = number.ToString();
        if (lenth < cnt)
        {
            int dif = cnt - lenth;
            for (int i = 0; i < dif; i++)
            {
                res = "0" + res;
            }
        }
        return res;
    }

    #endregion    
}
