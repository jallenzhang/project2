using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public static class SoundHelper
	{
		private const ulong DEFAULT_HZ=44100;
		
		public static void PlayMusic(string assetName,AudioSource audioSource)
		{
			if(SettingManager.Singleton.Music)
			{
				AudioClip audioClip=Resources.Load(assetName) as AudioClip;
				audioSource.clip=audioClip;
				audioSource.loop=true;
				audioSource.Play();
			}
		}
		public static void PlaySound(string assetName,AudioSource audioSource){
			
			PlaySound(assetName,audioSource,0);
		}
		
		public static void PlaySound(string assetName,AudioSource audioSource,uint delay)
		{
			if(audioSource==null)
				return;
			
			if(SettingManager.Singleton.Sound)
			{
				AudioClip audioClip=Resources.Load(assetName) as AudioClip;
				
				audioSource.clip=audioClip;
				audioSource.loop=false;
				ulong hz=DEFAULT_HZ*delay/1000;
				audioSource.Play(hz);
			}
		}
	}
}

