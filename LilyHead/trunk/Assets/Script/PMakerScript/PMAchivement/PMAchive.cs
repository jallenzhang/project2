using UnityEngine;
using System.Collections;
using DataPersist;
using AssemblyCSharp;
using System;
using LilyHeart;

public class PMAchive : MonoBehaviour
{
	public static int CntPmAchiveQueue = 0;
	
	protected Achievement achievement;
	
    public byte achievementNumber;
    public GameObject item;
    public GameObject prefab;
	public Action popDialogBehavior;
	public float waitTime = 1;
	public Vector3 localPosition;
	
	private float playTime = 5;	
	private string longAchivementStr = string.Empty;        

    // Use this for initialization
    void Start()
    {		
		SetAchivementMsg();
		SetWaitTime();
		
		longAchivementStr = Convert.ToString(User.Singleton.UserData.Achievements, 2);
        SetPrefabValue();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void GiveAward()
    {
		DeEnqueueMsg();
		
		if(PMAchive.CntPmAchiveQueue == 0){
			if(popDialogBehavior != null){
				popDialogBehavior();
			}
		}
		
        // so here we do nothing.
        if (item != null)
        {
            item.transform.parent = null;
            Destroy(item);			
        }
    }
	
	public void SetFullColor(){
		SetColor(1);
	}
	
	public void Set25Color(){
		SetColor(0.3f);
	}
	
	public void ShowPosition(){
        this.item.transform.localPosition = localPosition;
        this.item.transform.localScale = new Vector3(1, 1, 1);
	}
	
	public static void ResetAchivement(){
		CntPmAchiveQueue = 0;
	}
	
    #region Private Method

	void SetAchivementMsg(){	
		AchievementMessage msg = (AchievementMessage)User.Singleton.CurrentMessage;		
		this.achievementNumber = msg.AchievementNumber;
		msg.ProcessMessage();
		
		CntPmAchiveQueue++;	
	}
	
	void SetWaitTime(){
		if(CntPmAchiveQueue > 1){
			waitTime = (CntPmAchiveQueue - 1) * playTime;			
		}
		// Debug.LogError("lee test wait Time:" + waitTime);
	}
	
	void DeEnqueueMsg(){
		CntPmAchiveQueue--;
	}
	
    void SetPrefabValue()
    {
        if (achievementNumber <= GlobalManager.Singleton.Achievements.Length)
        {
            achievement = GlobalManager.Singleton.Achievements[achievementNumber - 1];

            UISprite sprite1 = transform.FindChild("Sprite1").GetComponent<UISprite>();
            string sprintname = string.Format("ct_cj_{0}", GetAchievementNumber(this.achievement.Number));
            sprite1.spriteName = sprintname;
			
            UILabel label = transform.FindChild("Label").GetComponent<UILabel>();
            label.text = this.achievement.Name;
			SetColor(0.6f);
        }
    }
	
	private void SetColor(float value){
		UISprite sprite1 = transform.FindChild("Sprite1").GetComponent<UISprite>();
		UISprite sprite2 = transform.FindChild("Sprite (GetAchievement)").GetComponent<UISprite>();
		sprite1.alpha = value;
		sprite2.alpha = value;
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
