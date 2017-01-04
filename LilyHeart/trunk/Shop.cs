using System;
using DataPersist;

namespace LilyHeart
{
	public class Shop
	{
		private static Shop shop;
		public static Shop Singleton {
			get
			{
				if(shop==null)
				{
					shop=new Shop();
				}
				return shop;
			}
		}
		
		public ItemType TempItemType {get;set;}
		public int TempItemId {get;set;}
		public string CurrentIAP {get;set;}
		
		private Shop ()
		{
		}
		
		public void BuyPropWithChip(int itemType, int itemId,long chip)
		{
			this.TempItemId = itemId;
			this.TempItemType = (ItemType)itemType;
            PhotonClient.Singleton.BuyItemByChips(this.TempItemType, this.TempItemId, chip);
		}
		
		public void BuyProp(int propId, ItemType itemType, int money, string result, PayWay payWay)
		{
			this.TempItemId = propId;
			this.TempItemType = itemType;
			PhotonClient.Singleton.BuyItem(this.TempItemType, propId, money, result, payWay);
		}
		
		public void BuyRoomWithChips(RoomType roomType,long chip)
		{
			this.TempItemId = (int)roomType;
			this.TempItemType = ItemType.Room;
			PhotonClient.Singleton.BuyItemByChips(this.TempItemType, this.TempItemId,chip);
		}
		
		public void BuyRoom(RoomType roomType, int money, string result, PayWay payWay)
		{
			this.TempItemType=ItemType.Room;
			this.TempItemId=(int)roomType;
			PhotonClient.Singleton.BuyItem(ItemType.Room,(int)roomType, money, result, payWay);
		}
		
		public void BuyChip(int chip, int money, string result, PayWay payWay)
		{
			this.TempItemType=ItemType.Chip;
			this.TempItemId=chip;
			PhotonClient.Singleton.BuyItem (ItemType.Chip,chip, money, result, payWay);
		}
		
		public void RemoveCurrentIAP()
		{
#if UNITY_IPHONE
			if (!string.IsNullOrEmpty(Shop.Singleton.CurrentIAP))
			{
				EtceteraBinding.etceteraUpdatePurchaseInfos(Shop.Singleton.CurrentIAP);
				Shop.Singleton.CurrentIAP = string.Empty;
			}
#endif
		}
	}
}

