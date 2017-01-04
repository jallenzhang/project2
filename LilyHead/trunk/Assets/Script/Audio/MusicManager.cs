using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using DataPersist;
using LilyHeart;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour {
	private const string MUSIC_PATH="Music/Scene/";
    private const float DEFAULT_TIME=0.25f;
	private const string SIMPLE_BG_MUSIC="Bg_simple";
	
	private static GameObject singleton;
	private static MusicManager musicManager;
	public static MusicManager Singleton {get {return musicManager;}}
	
	public bool EnableForeAudio=true;
	public bool EnableBgAudio=true;
	
	public AudioSource BgAudio {get;set;}
	public AudioSource ForeAudio {get;set;}
	
	void Awake()
	{
		if(singleton==null)
		{
			singleton=this.gameObject;
			musicManager=this;
			this.gameObject.audio.playOnAwake=false;
			if(BgAudio==null)
			{
				BgAudio=this.gameObject.audio;
			}
		}
		else if(singleton!=this.gameObject)
		{
			Destroy(this.gameObject);
		}
	}
	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(this.gameObject);
		Debug.Log (GlobalManager.Singleton.version);
		if(GlobalManager.Singleton.version==KindOfVersion.Basic&&singleton==this.gameObject)
		{
			PlayBgMusic();
		}
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	public void PlayBgMusic()
	{
		if(!EnableBgAudio)
		{
			return;
		}
		
		if(ForeAudio!=null&&ForeAudio.isPlaying)
		{
			return;
		}
		string musicName=GlobalManager.Singleton.version==KindOfVersion.Basic?SIMPLE_BG_MUSIC:Room.Singleton.RoomData.Owner.LivingRoomType.ToString();
		if((BgAudio!=null)&&(!BgAudio.isPlaying||BgAudio.name!=musicName))
		{
			string assetName=MUSIC_PATH+musicName;
			BgAudio.name=musicName;
			SoundHelper.PlayMusic(assetName,BgAudio);
		}
	}
	
	private string GetMusicName(string itemName)
	{
		string result = itemName=="Community"?"Common":itemName;
//		switch(itemName)
//		{
//		case "item0":
//			result = "Common";
//			break;
//		case "item1":
//			result = "China";
//			break;
//		case "item2":
//			result = "Egypt";
//			break;
//		case "item3":
//			result = "Hawaii";
//			break;
//		case "item4":
//			result = "Japan";
//			break;
//		case "item5":
//			result = "West";
//			break;
//		}
		
		return result;
	}
	
	public void PlayForeMusic(string btnName)
	{
		if(!EnableForeAudio)
		{
			return;
		}
		
		BgAudio.Stop();
		string assetName=MUSIC_PATH+GetMusicName(btnName);
		SoundHelper.PlayMusic(assetName,ForeAudio);
	}
	
	public void StopForeMusic()
	{
		if(ForeAudio==null)
		{
			return;
		}
		ForeAudio.Stop();
		PlayBgMusic();
	}
	
	public IEnumerator BgFadeOut(float volume)
	{
		if(BgAudio==null||volume>=BgAudio.volume)
		{
			yield break;
		}
		
		float time =DEFAULT_TIME/5;
		float volumeRate=(BgAudio.volume-volume)/5;
		
		while(BgAudio.volume>volume)
		{
			BgAudio.volume-=volumeRate;
			if(BgAudio.volume<volume)
			{
				BgAudio.volume=volume;
			}
			yield return new WaitForSeconds(time);
		}
	}
	
	public IEnumerator BgFadeIn(float volume)
	{
		if(BgAudio==null||volume<=BgAudio.volume)
		{
			yield break;
		}
		
		float time =DEFAULT_TIME/5;
		float volumeRate=(volume-BgAudio.volume)/5;
		
		while(BgAudio.volume<volume)
		{
			BgAudio.volume+=volumeRate;
			if(BgAudio.volume>volume)
			{
				BgAudio.volume=volume;
			}
			yield return new WaitForSeconds(time);
		}
	}
}
