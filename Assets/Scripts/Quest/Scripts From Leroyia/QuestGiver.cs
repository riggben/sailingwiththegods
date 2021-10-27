using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestGiver : MonoBehaviour
{
	//inside edu
	public Quest quest;

	public Player player;

	public QuestGoal goal;

	public GameObject questMenu;
	public Text titleText;
	public Text decriptionText;
	public Text moraleText;
	public Text goldText;

	public void OpenQuestMenu()
	{
		questMenu.SetActive(true);
		titleText.text = quest.title;
		decriptionText.text = quest.descritpion;
		moraleText.text = quest.moraleReward.ToString();
		goldText.text = quest.goldReward.ToString();
	}
	public void AcceptQuest()
	{
		questMenu.SetActive(false);
		quest.isActive = true;
		player.quest = quest;


	}
}
