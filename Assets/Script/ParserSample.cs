﻿using UnityEngine;
using System.Collections;

public class ParserSample : MonoBehaviour {
	public GUIText txtTime;

	// Use this for initialization
	void Start () {
		txtTime.text = "Time = 00:00:00.0";
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI()
	{
		float fPosX = 20, fPosY = 50, fYInterval = 40;
		int nYPosCount = 0;

		System.DateTime timeStart = System.DateTime.Now;
		System.TimeSpan timeToSpen;


		// CSV...
		nYPosCount = 0;
		if (GUI.Button(new Rect(fPosX, fPosY + (fYInterval * nYPosCount), 150, 30), "CSV Start"))
		{
			CSV csv = new CSV("TestCSV.txt");

			// Load

			// Paesing
			{
				timeStart = System.DateTime.Now;

				TextAsset txt = Resources.Load<TextAsset>("Tables/Sample_CSV");
				csv.Parse(txt.text);

				timeToSpen = System.DateTime.Now - timeStart;
			}

			// Show result
			txtTime.text = "Time = " + timeToSpen.ToString();
		}

		// JSON...
		nYPosCount = 2;
		if (GUI.Button(new Rect(fPosX, fPosY + (fYInterval * nYPosCount), 150, 30), "JSON Start"))
		{
			SimpleJSON.JSONNode json = null;
			
			// Load
			
			// Paesing
			{
				timeStart = System.DateTime.Now;
				
				TextAsset txt = Resources.Load<TextAsset>("Tables/Sample_JSON");
				json = SimpleJSON.JSONNode.Parse(txt.text);

				timeToSpen = System.DateTime.Now - timeStart;

				Debug.Log(json.ToString());
			}
			
			// Show result
			txtTime.text = "Time = " + timeToSpen.ToString();
		}

	}
}