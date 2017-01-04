using UnityEngine;
using System.Collections;

//[RequireComponent(typeof(Animation))]
public class TF_ButtonAnimationFromPrefab : MonoBehaviour {
	public enum Trigger
	{
		OnClick,
		OnMouseOver,
		OnMouseOut,
		OnPress,
		OnRelease,
		OnDoubleClick,
	}
	
	public AnimationClip animationToPlay;
	public GameObject buttonTarget;
	public GameObject prefabTarget;
	public bool closePrefab = false;
	
	public Trigger trigger = Trigger.OnClick;
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnClick()
	{
		PlayAnimation();
	}
	
	void PlayAnimation()
	{
		if (enabled && trigger == Trigger.OnClick)
		{
			if (prefabTarget != null)
			{
				Animation animation = prefabTarget.GetComponent<Animation>();
				if (animation != null)
				{
					animation.Play(animationToPlay.name);
					
					if (closePrefab)
					{
						fadePanel panel = prefabTarget.GetComponent<fadePanel>();
						panel.fadeOut(null);
					}
				}
			}
		}
	}
}
