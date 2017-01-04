using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

namespace LilyHeart
{
	
	public enum FileType
	{
		Account = 1,
		DropCache,
		Duration,
		SerializeError,
		SuspendError
	}
	
	public class FileIOHelper
	{
		private const string FILE_DROPCACHE="DropCache.dat";
		private const string FILE_DURATION="Duration.dat";
		private const string FILE_FUCK="SerializeError.dat";
		private const string FILE_SUSPEND="Suspend.dat";
		
		public static string ReadFile(FileType fileType)
		{
			switch(fileType)
			{
			case FileType.Account:
				return GetAccountContent();
			case FileType.DropCache:
				return GetDropCache();
			case FileType.Duration:
				return GetDuration();
			default:
				return string.Empty;
			}
		}
		
		public static void WriteFile(FileType fileType, string content)
		{
			switch(fileType)
			{
			case FileType.Account:
				SetAccountContent(content);
				break;
			case FileType.DropCache:
				SetDropCache(content);
				break;
			case FileType.Duration:
				SetDuration(content);
				break;
			case FileType.SerializeError:
				SetSerializeError(content);
				break;
			case FileType.SuspendError:
				SetSuspendError(content);
				break;
			default:
				break;
			}
		}
		
		public static void DeleteFile(FileType fileType)
		{
			switch(fileType)
			{
				case FileType.Account:
					DeleteAccountFileOnApplieDevice();
					break;
				default:
					break;
			}
		}
		
		public static Dictionary<string,Duration> ReadDurationFile()
		{
			string path=Application.persistentDataPath+"/"+FILE_DURATION;
			try
			{
				if(File.Exists(path))
				{
					BinaryFormatter binaryFormatter=new BinaryFormatter();
					FileStream fileStream=new FileStream(path,FileMode.Open);
					Dictionary<string,Duration> durations= binaryFormatter.Deserialize(fileStream) as Dictionary<string,Duration>;
					fileStream.Close();
					return durations;
				}
				else
				{
					return new Dictionary<string, Duration>();
				}
			}
			catch
			{
				File.Delete(path);
				return new Dictionary<string, Duration>();
			}
		}
		
		public static void SaveDurationFile(Dictionary<string,Duration> durations)
		{
			string path=Application.persistentDataPath+"/"+FILE_DURATION;
			if(File.Exists(path))
			{
				File.Delete(path);
			}
			BinaryFormatter binaryFormatter=new BinaryFormatter();
			FileStream fileStream=new FileStream(path,FileMode.Create);
			binaryFormatter.Serialize(fileStream,durations);
			fileStream.Close();
		}
		
		private static void SetAccountContent(string content)
		{
			//if (Application.platform == RuntimePlatform.IPhonePlayer)
			//{
				//_SetAccountContent(content);
				PutAccountContentToAppleDevice(content);
			//}
		}
		
		private static string GetAccountContent()
		{
			//if (Application.platform == RuntimePlatform.IPhonePlayer)
			//{
				//return _GetAccountContent();
				return GetAccountContentFromAppleDevice();
			//}
			
			
			
			return null;
		}
		
		private static void DeleteAccountFileOnApplieDevice()
		{
			string accountFilePath = GetAccountFilePath();
			Debug.Log("@@@@ DeleteAccountFileOnApplieDevice");
			if (System.IO.File.Exists(accountFilePath))
			{
				Debug.Log("@@@ begin to delete file!");
				File.Delete(accountFilePath);
			}
		}
		
		private static void PutAccountContentToAppleDevice(string content)
		{
			FileStream fs = null;
			string accountFilePath = GetAccountFilePath();
			
			if (System.IO.File.Exists(accountFilePath))
			{
				File.Delete(accountFilePath);
			}
			
			fs = new FileStream(accountFilePath, FileMode.Create);
			
			Byte[] info = new UTF8Encoding(true).GetBytes(content);
			fs.Write(info, 0, info.Length);
			fs.Close();
		}
		
		private static string GetAccountFilePath()
		{
			string retValue = string.Empty;
			switch(Application.platform)
			{
			case RuntimePlatform.IPhonePlayer:
			case RuntimePlatform.Android:
				string accountFolderPath = Application.persistentDataPath;
				retValue = System.IO.Path.Combine(accountFolderPath, "account.dat");
				break;
			default:
				retValue = System.IO.Path.Combine(Environment.CurrentDirectory, "account.dat");
				break;
			}
			
			return retValue;
		}
		
		private static string GetAccountContentFromAppleDevice()
		{
			string retValue = string.Empty;
			
			string accountFilePath = GetAccountFilePath();
			
			if (!System.IO.File.Exists(accountFilePath))
			{
				return retValue;
			}
			
			FileStream fs = new FileStream(accountFilePath, FileMode.OpenOrCreate, FileAccess.Read);
			BinaryReader read = new BinaryReader(fs);
			int count = (int)fs.Length;
			byte[] buffer = new byte[count];
            read.Read(buffer, 0, buffer.Length);
			
            retValue = Encoding.Default.GetString(buffer);
			fs.Close();
			return retValue;
		}
		
		#region DropCache
		private static void SetDropCache(string content)
		{
			string path=Application.persistentDataPath+"/"+FILE_DROPCACHE;
			if(File.Exists(path))
			{
				File.Delete(path);
			}
			FileStream fileStream=new FileStream(path,FileMode.OpenOrCreate);
			byte[] bytes=Encoding.UTF8.GetBytes(content);
			fileStream.Write(bytes,0,bytes.Length);
			fileStream.Close();
		}
		
		private static string GetDropCache()
		{
			string content=string.Empty;
			string path=Application.persistentDataPath+"/"+FILE_DROPCACHE;
			if(File.Exists(path))
			{
				FileStream fileStream=new FileStream(path,FileMode.Open);
				byte[] bytes=new byte[fileStream.Length];
				fileStream.Read (bytes,0,bytes.Length);
				content=Encoding.UTF8.GetString(bytes);
				fileStream.Close();
			}
			return content;
		}
		#endregion
		
		#region Duration
		private static string GetDuration()
		{
			string content=string.Empty;
			string path=Application.persistentDataPath+"/"+FILE_DURATION;
			if(File.Exists(path))
			{
				FileStream fileStream=new FileStream(path,FileMode.Open);
				byte[] bytes=new byte[fileStream.Length];
				fileStream.Read(bytes,0,bytes.Length);
				content=Encoding.UTF8.GetString(bytes);
				fileStream.Close();
			}
			if(content.Split(',').Length<2)
			{
				content=string.Empty;
			}
			return content;
		}
		
		public static void SetDuration(string content)
		{
			string path=Application.persistentDataPath+"/"+FILE_DURATION;
			if(File.Exists(path))
			{
				File.Delete(path);
			}
			FileStream fileStream=new FileStream(path,FileMode.OpenOrCreate);
			byte[] bytes=Encoding.UTF8.GetBytes(content);
			fileStream.Write(bytes,0,bytes.Length);
			fileStream.Close();
		}
		#endregion
		
		
		private static void SetSerializeError(string content)
		{
			string path=Application.persistentDataPath+"/"+FILE_FUCK;
			if(File.Exists(path))
			{
				File.Delete(path);
			}
			FileStream fileStream=new FileStream(path,FileMode.OpenOrCreate);
			byte[] bytes=Encoding.UTF8.GetBytes(content);
			fileStream.Write(bytes,0,bytes.Length);
			fileStream.Close();
		}
			
		private static void SetSuspendError(string content)
		{
			string path=Application.persistentDataPath+"/"+FILE_SUSPEND;
			if(File.Exists(path))
			{
				File.Delete(path);
			}
			FileStream fileStream=new FileStream(path,FileMode.OpenOrCreate);
			byte[] bytes=Encoding.UTF8.GetBytes(content);
			fileStream.Write(bytes,0,bytes.Length);
			fileStream.Close();
		}
	}
}