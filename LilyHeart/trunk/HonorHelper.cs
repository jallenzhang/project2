using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.IO;
using System;
using DataPersist;

namespace LilyHeart
{
	public enum HonorType
	{
		Citizen = 1,
		Knight,
		Baron,
		Vicomte,
		Comte,
		Marquis,
		Duke,
		Archduke,
		Infante,
		Prince,
		King
	}
	
	public class HonorHelper {
		
		private static Dictionary<int, Honor> honorDictionary = new Dictionary<int, Honor>();
		
		public static int GetChipLevel(long data, int level)
		{
			int result = level;
			
			if (level >=6)
				return result;
			
			long nextData = data / 10;
			
			if (nextData > 0)
			{
				result = GetChipLevel(nextData, level + 1);
			}
			
			return result;
		}
		
		public static string GetChipString(int level)
		{
			string result = string.Empty;
			
			if (level == 1 || level == 2)
			{
				result = "Chipyellow";
			}
			else if (level == 3)
			{
				result = "Chipgreen";
			}
			else if (level == 4)
			{
				result = "Chipred";
			}
			else if (level == 5)
			{
				result = "Chipblue";
			}
			else
			{
				result = "Chipblack";
			}
			
			return result;
		}
		
		public static string GetHonorPicString(HonorType honor, byte avator)
		{
			string result = string.Empty;
			
			switch(honor)
			{
			case HonorType.Citizen:
				result = honor.ToString();
				break;
			case HonorType.Knight:
				result = honor.ToString();
				break;
			case HonorType.Baron:
				result = honor.ToString();
				break;
			case HonorType.Vicomte:
				result = "Viscount";
				break;
			case HonorType.Comte:
				result = "Count";
				break;
			case HonorType.Marquis:
				result = honor.ToString();
				break;
			case HonorType.Duke:
				result = honor.ToString();
				break;
			case HonorType.Archduke:
				result = "GrandDuke";
				break;
			case HonorType.Infante:
				if ((int)avator <= (int)PlayerAvator.European || (int)avator == (int)PlayerAvator.AGe || (int)avator == (int)PlayerAvator.General)
					result = "age";
				else
					result = "Crownprince";
				break;
			case HonorType.Prince:
				result = honor.ToString();
				break;
			case HonorType.King:
				if ((int)avator <= (int)PlayerAvator.European || (int)avator == (int)PlayerAvator.AGe || (int)avator == (int)PlayerAvator.General)
					result = honor.ToString();
				else
					result = "Queen";
				break;
			default:
				break;
			}
			
			return result;
		}
	
		public static void LoadXmlFile(TextAsset resource)
		{
			byte[] encodedString = Encoding.UTF8.GetBytes(resource.text);
	
		    MemoryStream ms = new MemoryStream(encodedString);
		    ms.Flush();
		    ms.Position = 0;
			
			XmlDocument doc = new XmlDocument();
	        doc.Load(ms);
	
	        XmlNode root = doc.SelectSingleNode("honors");
	        XmlNodeList nodeList = root.ChildNodes;
			
			foreach (XmlNode node in  nodeList)
			{
				Honor honor = new Honor();
				
				foreach (XmlNode xn in node.ChildNodes)
				{
					XmlElement xe = (XmlElement)xn;
					if (xe.Name == "id")
					{
						honor.Id = Convert.ToInt32(xe.InnerText); 
					}
					if (xe.Name == "low_bouns")
					{
						honor.Low_bound = Convert.ToInt32(xe.InnerText); 
					}
					if (xe.Name == "high_bouns")
					{
						honor.High_bound = Convert.ToInt32(xe.InnerText); 
					}
					if (xe.Name == "game_number")
					{
						honor.Game_number = Convert.ToInt32(xe.InnerText); 
					}
					if (xe.Name == "game_win_number")
					{
						honor.Game_win_number = Convert.ToInt32(xe.InnerText); 
					}
					if (xe.Name == "level")
					{
						honor.Level = Convert.ToInt32(xe.InnerText); 
					}
					if (xe.Name == "match_number")
					{
						honor.Match_number = Convert.ToInt32(xe.InnerText); 
					}
				}
				honorDictionary.Add(honor.Id, honor);
				
			}
			
			Debug.Log("Honor count is: " + honorDictionary.Count.ToString());
		}
		
		public static HonorType GetHonorRecuise(UserData user, HonorType baseHonor)
		{
			HonorType result = baseHonor;
			int nextHonor = (int)baseHonor + 1;
			
			if (nextHonor == 12)
				return HonorType.King;
			
			Honor honor = honorDictionary[nextHonor];
			
			if (user.Chips > honor.Low_bound 
				&& user.HandsPlayed > honor.Game_number 
				&& user.HandsWon > honor.Game_win_number
				&& user.Level > honor.Level
				&& user.CareerWins >= honor.Match_number)
			{
				result = GetHonorRecuise(user, (HonorType)nextHonor);
			}
			
			return result;
		}
		
//		public static string GetHonor(UserData user)
//		{
//			string result = string.Empty;
//			
//			HonorType currentHonor = GetHonorRecuise(user, HonorType.Citizen);
//			
//			switch(currentHonor)
//			{
//			case HonorType.Citizen:
//				result = LocalizeHelper.Translate("HONOR_CITIZEN");
//				break;
//			case HonorType.Knight:
//				result = LocalizeHelper.Translate("HONOR_KNIGHT");
//				break;
//			case HonorType.Baron:
//				result = LocalizeHelper.Translate("HONOR_BARON");
//				break;
//			case HonorType.Vicomte:
//				result = LocalizeHelper.Translate("HONOR_VICOMTE");
//				break;
//			case HonorType.Comte:
//				result = LocalizeHelper.Translate("HONOR_COMTE");
//				break;
//			case HonorType.Marquis:
//				result = LocalizeHelper.Translate("HONOR_MARQUIS");
//				break;
//			case HonorType.Duke:
//				result = LocalizeHelper.Translate("HONOR_DUKE");
//				break;
//			case HonorType.Archduke:
//				result = LocalizeHelper.Translate("HONOR_ARCHDUKE");
//				break;
//			case HonorType.Infante:
//				if(IsHonorMaleSex((int)user.Avator))
//					result = LocalizeHelper.Translate("HONOR_INFANTE");
//				else
//					result = LocalizeHelper.Translate("HONOR_PRINCESS");
//				break;
//			case HonorType.Prince:
//				result = LocalizeHelper.Translate("HONOR_PRINCE");
//				break;
//			case HonorType.King:
//				if(IsHonorMaleSex((int)user.Avator))
//					result = LocalizeHelper.Translate("HONOR_KING");
//				else
//					result = LocalizeHelper.Translate("HONOR_QUEUE");
//				break;
//			default:
//				break;
//			}
//			
//			return result;
//		}
	public static bool IsHonorMaleSex(int nAvator)
	{
		bool bResult = false;
		if (nAvator <= (int)PlayerAvator.European||nAvator == (int)PlayerAvator.AGe ||nAvator == (int)PlayerAvator.General)
			bResult = true;
		return bResult;
	}
	}
	public class Honor
	{
		public int Id
		{
			get;
			set;
		}
		
		public int Low_bound
		{
			get;
			set;
		}
		
		public int High_bound
		{
			get;
			set;
		}
		
		public int Game_number
		{
			get;
			set;
		}
		
		public int Game_win_number
		{
			get;
			set;
		}
		
		public int Level
		{
			get;
			set;
		}
		
		public int Match_number
		{
			get;
			set;
		}
	}
}
