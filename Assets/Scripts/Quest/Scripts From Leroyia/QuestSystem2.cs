using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest 
{
	public bool isActive;
	public CrewDialogManager Manager;
	public MeterBar meterBar;

	public string title;
	public string descritpion;
	public int moraleReward;
	public int goldReward;

	public QuestGoal goal;

	public void Complete()
	{
		isActive = false;
		//Manager.currentMeter += moraleReward;
		Manager.currentMeter = moraleReward + Manager.currentMeter;
		meterBar.SetMeter(Manager.currentMeter);
		Debug.Log(Manager.currentMeter + " Is how much the meter went up.");
		Debug.Log(title + " was completed!");

	}
}
