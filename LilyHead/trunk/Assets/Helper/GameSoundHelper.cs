using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public enum PlayerBehavior
{
	AllIn,
	Fold,
	Pass,
	Follow,
	Speak_01,
	Speak_02,
	Speak_03,
	Speak_04
}

public class GameSoundHelper {
	
	private static string GetSoundPath(PlayerBehavior behavior, bool sex)//true: male, false:female
	{
		string result = string.Empty;
		
		switch(behavior)
		{
		case PlayerBehavior.AllIn:
			if (sex)
			{
				result = "Music/Other/male/male_08";
			}
			else
			{
				result = "Music/Other/female/female_08";
			}
			break;
		case PlayerBehavior.Fold:
			if (sex)
			{
				result = "Music/Other/male/male_02";
			}
			else
			{
				result = "Music/Other/female/female_02";
			}
			break;
		case PlayerBehavior.Pass:
			if (sex)
			{
				result = "Music/Other/male/male_03";
			}
			else
			{
				result = "Music/Other/female/female_03";
			}
			break;
		case PlayerBehavior.Follow:
			if (sex)
			{
				result = "Music/Other/male/male_04";
			}
			else
			{
				result = "Music/Other/female/female_04";
			}
			break;
		case PlayerBehavior.Speak_01:
			if (sex)
			{
				result = "Music/Other/male/male_05";
			}
			else
			{
				result = "Music/Other/female/female_05";
			}
			break;
		case PlayerBehavior.Speak_02:
			if (sex)
			{
				result = "Music/Other/male/male_06";
			}
			else
			{
				result = "Music/Other/female/female_06";
			}
			break;
		case PlayerBehavior.Speak_03:
			if (sex)
			{
				result = "Music/Other/male/male_07";
			}
			else
			{
				result = "Music/Other/female/female_07";
			}
			break;
		case PlayerBehavior.Speak_04:
			if (sex)
			{
				result = "Music/Other/male/male_01";
			}
			else
			{
				result = "Music/Other/female/female_01";
			}
			break;
		}
		
		return result;
	}
	
	public static void PlaySound(PlayerBehavior behavior, AudioSource audioSource, bool sex)
	{
		string soundPath = GetSoundPath(behavior, sex);
		Debug.Log("soundPath " + soundPath + " sex " + sex);
		SoundHelper.PlaySound(soundPath,audioSource,0);
	}
}
