﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonTabs : MonoBehaviour {
	public static ButtonTabs instance;

	public TabsButton[] buttons;
	public GameObject[] tabs;
	int? currentlyOpened;
	public Text saveButtonText;

	void Awake() {
		instance = this;
	}

	void Start () {
		foreach (var item in tabs) {
			item.SetActive(false);
		}
		Open(1);
	}

	public void Open(int id) {
		if (id == 3 && ReadJson.instance.uploadedFiles.Count == 0)
			return;
		if (currentlyOpened == 3) {
			Calendar.instance.ResetActivityListPadding();
		}

		if (currentlyOpened == id) {
			buttons[id].Disable();
			tabs[id].SetActive(false);
			currentlyOpened = null;
		} else {
			if (id == 3) {
				Calendar.instance.OnCalendarEnable();
			}
			if (currentlyOpened.HasValue) {
				buttons[currentlyOpened.Value].Disable();
				tabs[currentlyOpened.Value].SetActive(false);
			}
			buttons[id].Enable();
			tabs[id].SetActive(true);
			currentlyOpened = id;
		}
	}


	public void Save() {
		saveButtonText.gameObject.SetActive(true);
		saveButtonText.text = "Saving...";
		StartCoroutine(RevertAfterTime());
	}
	IEnumerator RevertAfterTime() {
		yield return new WaitForEndOfFrame();
		yield return new WaitForSeconds(1f);
		SaveSystem.Save();
		saveButtonText.text = "Saved!";
		yield return new WaitForSeconds(1.5f);
		saveButtonText.gameObject.SetActive(false);
	}

	public void ClearCalendarView() {
		if (currentlyOpened == 3)
			Open(3);
	}

	public void OpenURL(string url) {
		Application.OpenURL(url);
	}

}
