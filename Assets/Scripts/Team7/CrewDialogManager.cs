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

	public DialogueRunner dialogRunner;
	public GameObject diaglogScreen;
	public AudioSource testsound;
	public GameObject water;
	public Material mat_gat;
	void Start()
    {
		currentMeter = 50;
		meterBar.SetMeter(currentMeter);
		//diaglogRunner.AddCommandHandler("end_dialog", TurnOffScene);
		dialogRunner.AddCommandHandler("increase", IncreaseMorale);
		dialogRunner.AddCommandHandler("decrease", DecreaseMorale);
		dialogRunner.AddCommandHandler("gatorade", GatoradeMode);

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

	public void GatoradeMode(string[] parameter) 
	{

		water.GetComponent<MeshRenderer>().material = mat_gat;

	}

	// Update is called once per frame
	void Update()
    {
		
	}
}
