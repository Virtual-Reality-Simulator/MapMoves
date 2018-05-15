﻿using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FacebookApiResponse {
	public class CategoryList {
		public string id;
		public string name;
	}
	public CategoryList[] category_list;
	public string id;
}

[Serializable]
public class SpecialIcons {
	public string name;
	public int iconId;
}

public class FacebookPlaces : MonoBehaviour {
	public static FacebookPlaces instance;

	public SpecialIcons[] icons;
	public Sprite[] iconsImages;
	Dictionary<string, int> placesMemory = new Dictionary<string, int>();
	Dictionary<string, int> customIcons = new Dictionary<string, int>();

	// In future versions Facebook API login dialog will be implemented
	public string fbAccessToken;

	void Awake() {
		instance = this;
	}

	public void GetPlaceCategory(string placeId, Action<int> action) {
		StartCoroutine(GetText(placeId, action));
	}

	IEnumerator GetText(string placeId, Action<int> action) {
		int number = 0;
		if (placesMemory.TryGetValue(placeId, out number)) {
			action.Invoke(icons[number].iconId);
		} else {
			string address = "https://graph.facebook.com/v2.12/" + placeId + "?fields=category_list&access_token=" + fbAccessToken;
			using (UnityWebRequest www = UnityWebRequest.Get(address)) {
				yield return www.Send();

				if (www.isNetworkError || www.isHttpError) {
					Debug.Log(www.error);
				} else {
					FacebookApiResponse m = JsonConvert.DeserializeObject<FacebookApiResponse>(www.downloadHandler.text);
					if (!FindIcon(placeId, m, action))
						Debug.Log(m.category_list[0].name);
				}
			}
		}
	}

	bool FindIcon(string placeId, FacebookApiResponse m, Action<int> action) {
		for (int i = 0; i < icons.Length; i++) {
			if (icons[i].name == m.category_list[0].name) {
				action.Invoke(icons[i].iconId);
				if (!placesMemory.ContainsKey(placeId))
					placesMemory.Add(placeId, i);
				return true;
			}
		}
		return false;
	}
}