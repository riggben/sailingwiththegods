using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class CrewDialogManager : MonoBehaviour
{
	public int maxMeter = 100;
	public int currentMeter;
	public MeterBar meterBar;

	public DialogueRunner diaglogRunner;
	public GameObject diaglogScreen;
	public AudioSource testsound;
	void Start()
    {
		currentMeter = 50;
		meterBar.SetMeter(currentMeter);
		//diaglogRunner.AddCommandHandler("end_dialog", TurnOffScene);
		diaglogRunner.AddCommandHandler("increase", IncreaseMorale);
		diaglogRunner.AddCommandHandler("decrease", DecreaseMorale);

	}

	public void TurnOffScene(string [] parameter)
	{
		//diaglogScreen.SetActive(false);
		testsound.Play();
		Debug.Log("PLAYSOUND");
	}

	public void IncreaseMorale(string[] parameter) 
	{
		currentMeter += 10;
		meterBar.SetMeter(currentMeter);
		Debug.Log("Increase");
	}
	public void DecreaseMorale(string[] parameter) 
	{
		currentMeter -= 10;
		meterBar.SetMeter(currentMeter);
		Debug.Log("Decrease");
	}

	// Update is called once per frame
	void Update()
    {
		
	}
}
