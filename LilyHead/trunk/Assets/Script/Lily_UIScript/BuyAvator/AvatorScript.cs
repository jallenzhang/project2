using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AvatorScript : MonoBehaviour {
	
	public enum AvatorFeeType
	{
		Free,
		Chip,
		Money,
		Using,
		ToUse
	}
	
	public enum AvatorPrice
	{
		price70K = 70000,
		price100K = 100000,
		price130K = 130000,
		price150K = 150000,
		Price160K = 160000,
		Price200K = 200000,
		Price240K = 240000,
		Price300K = 300000,
		Price350K = 350000,
		Price370K = 370000,
		Price400K = 400000,
		Price450K = 450000,
		price2,
	}
	
	public enum AvatorType:byte
	{
		Guest,
		DaHeng = 1,
		Songer,
		Captain,
		European,
		Qianjing,
		Dalaopo,
		Yitaitai,
		Luoli,
		AGe,
		NianGenYao,//10
		GeGe,
		NiangNiang
	}
	
	public enum AvatorIdInPurchse
	{
		DaHeng = 301,
		Songer ,
		Captain,
		European,
		Qianjing,
		Dalaopo,
		Yitaitai,
		Luoli,
		AGe,
		NianGenYao,//10
		GeGe,
		NiangNiang
	}
	
	public AvatorFeeType feeType;
	public AvatorPrice price;
	public AvatorType  avatorType;
	public AvatorIdInPurchse avatorIdInPurchase;
	public string avatorPicName;
	public string avatorName;
	public List<GameObject> freeGameObjects = new List<GameObject>();
	public List<GameObject> chipGameObjects = new List<GameObject>();
	public List<GameObject> moneyGameObjects = new List<GameObject>();
	public List<GameObject> UsingGameObjects = new List<GameObject>();
	public List<GameObject> ToUseGameObjects = new List<GameObject>();
	
	// Use this for initialization
	void Start () {
		//UpdateAvator();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void UpdateAvator()
	{
		if ((int)feeType == -1)
		{
			Debug.LogWarning("feetype is -1");
			return;
		}
		
		
		foreach(GameObject obj in freeGameObjects)
		{
			if (feeType == AvatorFeeType.Free)
				obj.SetActiveRecursively(true);
			else
				obj.SetActiveRecursively(false);
		}
		
		foreach(GameObject obj in chipGameObjects)
		{
			if (feeType == AvatorFeeType.Chip)
				obj.SetActiveRecursively(true);
			else
				obj.SetActiveRecursively(false);
		}
		
		foreach(GameObject obj in moneyGameObjects)
		{
			if (feeType == AvatorFeeType.Money)
				obj.SetActiveRecursively(true);
			else
				obj.SetActiveRecursively(false);
		}
		
		foreach(GameObject obj in UsingGameObjects)
		{
			if (feeType == AvatorFeeType.Using)
			{
				obj.SetActiveRecursively(true);
			}
			else
				obj.SetActiveRecursively(false);
		}
		
		foreach(GameObject obj in ToUseGameObjects)
		{
			if (feeType == AvatorFeeType.ToUse)
				obj.SetActiveRecursively(true);
			else
				obj.SetActiveRecursively(false);
		}
	}
}
