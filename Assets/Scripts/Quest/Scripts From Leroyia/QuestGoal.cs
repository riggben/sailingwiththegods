using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class QuestGoal 
{
	//if quest.city has a statue then give the qust.moraleReward and quest.goldReward
	//if there's a amount of statues/items needed
	//change needed amount to statue amount and attach to statue 

	public GoalType goalType;

	public int neededAmount;
	public int currentAmount;

	public bool isDone()
	{
		if (currentAmount >= neededAmount)
		{
			return true;
		}
		return false;
	}

	public void StatueAdded()
	{
		//check if a statue has been placed in said city

		if (goalType == GoalType.ItemPlacing)
		{
			currentAmount++;
		}


	}
}

public enum GoalType
{
	ItemPlacing,
	Escort
}
