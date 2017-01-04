// (c) copyright Hutong Games, LLC 2010-2011. All rights reserved.

using HutongGames.PlayMaker;
using UnityEngine;

/// <summary>
/// Put this component on the GameObject with the Collider used by NGUI.
/// Choose an FSM to send events to (leave blank to target an FSM on the same GameObject).
/// You can rename the events to match descriptive event names in your FSM. E.g., "OK Button Pressed"
/// NOTE: Use the Get Event Info action in PlayMaker to get event arguments.
/// See also: http://www.tasharen.com/?page_id=160
/// </summary>
public class UIEventsToPlaymakerFSM : MonoBehaviour
{
	public PlayMakerFSM targetFSM;
	public string onClickEvent = "OnClick";
	public string onHoverEvent = "OnHover";
	public string onPressEvent = "OnPress";
	public string OnSelectEvent = "OnSelect";
	public string OnDragEvent = "OnDrag";
	public string OnDropEvent = "OnDrop";
	public string OnTooltipEvent = "OnTooltip";

	void OnEnable()
	{
		if (targetFSM == null)
		{
			targetFSM = GetComponent<PlayMakerFSM>();
		}

		if (targetFSM == null)
		{
			enabled = false;
		}
	}

	void OnClick()
	{
		if (!enabled || targetFSM == null) return;
		targetFSM.SendEvent(onClickEvent);
	}

	void OnHover(bool isOver)
	{
		if (!enabled || targetFSM == null) return;
		Fsm.EventData.BoolData = isOver;
		targetFSM.SendEvent(onHoverEvent);
	}

	void OnPress(bool pressed)
	{
		if (!enabled || targetFSM == null) return;
		Fsm.EventData.BoolData = pressed;
		targetFSM.SendEvent(onPressEvent);
	}

	void OnSelect(bool selected)
	{
		if (!enabled || targetFSM == null) return;
		Fsm.EventData.BoolData = selected;
		targetFSM.SendEvent(OnSelectEvent);
	}

	void OnDrag(Vector2 delta)
	{
		if (targetFSM == null) return;
		Fsm.EventData.Vector3Data = new Vector3(delta.x, delta.y);
		targetFSM.SendEvent(OnDragEvent);
	}

	void OnTooltip(bool show)
	{
		if (!enabled || targetFSM == null) return;
		Fsm.EventData.BoolData = show;
		targetFSM.SendEvent(OnTooltipEvent);
	}

}
