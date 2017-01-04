using UnityEngine;
using System.Collections;
using AssemblyCSharp.Helper;
using AssemblyCSharp;
using LilyHeart;

public class NewVesionDialog : DialogInfo
{
	private string internalVersion = string.Empty;
	private string upgradeUrl = string.Empty;
	private DataPersist.DeviceType deviceType;
	
	public NewVesionDialog (string version)
	{
		internalVersion = version;
		this.Title = LocalizeHelper.Translate("TIPS_NEW_VERSION_NEED_TO_DOWN_TITLE");
		this.Description = string.Format(LocalizeHelper.Translate("TIPS_NEW_VERSION_NEED_TO_DOWN_DESCRIPTION"), version);
		this.Buttons = 2;
		PhotonClient.UpgradUrlEvent += UpgradeUrlCallback;
		
		deviceType = GetDeviceType();
		
		PhotonClient.Singleton.UpgradeUrl(deviceType);
	}
	
	private DataPersist.DeviceType GetDeviceType()
	{
#if UNITY_IPHONE
		return DataPersist.DeviceType.IOS;
#endif
		
#if UNITY_ANDROID
		return DataPersist.DeviceType.ANDROID;
#endif
		return DataPersist.DeviceType.IOS;
	}
	
	private void UpgradeUrlCallback(string url)
	{
		if (!string.IsNullOrEmpty(url))
		{
			PhotonClient.UpgradUrlEvent -= UpgradeUrlCallback;
			upgradeUrl = url;
		}
	}
	
	public override void Process ()
	{
		if (!string.IsNullOrEmpty(upgradeUrl))
		{
			Application.OpenURL(upgradeUrl);
		}
		
		QuitAtMajorChange();
	}
	
	public override void CancelProcess()
	{
		//Debug.Log("todo, need to check the master version! if not equel, exit the app");
		QuitAtMajorChange();
	}
	
	private void QuitAtMajorChange()
	{
		string[] internalStrings = internalVersion.Split('.');
		string[] currentStrings = MyVersion.CurrentVersion.Split('.');
		if (internalStrings[0] != currentStrings[0] || internalStrings[1] != currentStrings[1])
		{
			Debug.Log("Quit");
			Application.Quit();
		}
		else
		{
			if (PlatformHelper.CanAutoLogin())
			{
				PlatformHelper.AutoLogin();
			}
			else
			{
				if (GlobalManager.Singleton.version == KindOfVersion.Ultimate)
					Application.LoadLevelAsync("LaunchTable");
				else
				{
					if(GlobalManager.Singleton.ApplicationType == AppType.NinetyOne)
					{
						Application.LoadLevelAsync("LaunchTable_91");
					}
					else
						Application.LoadLevelAsync("LaunchTable_simple");
				}
			}
		}
	}
	
}
