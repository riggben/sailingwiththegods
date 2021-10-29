//turns a gameobject/3d mesh into a button

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MeshButton : MonoBehaviour
{

	public GameObject button;
	public Camera mainCamera;
	public UnityEvent OnClick = new UnityEvent();
	

	void Start() {
		button = this.gameObject;
	}

	void Update() {
		var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
		RaycastHit Hit;

		if (Input.GetMouseButtonDown(0)) {
			if (Physics.Raycast(ray, out Hit) && Hit.collider.gameObject == gameObject) {
				Debug.Log("Button Clicked");
				OnClick.Invoke();
			}
		}
	}
}