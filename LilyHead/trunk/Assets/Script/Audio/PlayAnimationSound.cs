using UnityEngine;
using System.Collections;
using EasyMotion2D;
using AssemblyCSharp;

[RequireComponent(typeof(AudioSource))]
public class PlayAnimationSound : MonoBehaviour {
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	public void Play(SpriteAnimationEvent e)
	{
		if(e.stringParameter!=audio.name)
		{
			SoundHelper.PlaySound(e.stringParameter,audio,(uint)e.intParameter);
			audio.name=e.stringParameter;
		}
		else if(SettingManager.Singleton.Sound)
		{
			audio.Play((uint)e.intParameter);
		}
	}
}
