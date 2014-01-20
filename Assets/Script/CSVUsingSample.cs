using UnityEngine;
using System.Collections;

public class CSVUsingSample : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI()
	{
		float fPosX = 20, fPosY = 50, fYInterval = 40;
		int nYPosCount = 0;
		
		nYPosCount = 0;
		if (GUI.Button(new Rect(fPosX, fPosY + (fYInterval * nYPosCount), 150, 30), "Show Datas"))
		{
			CSV csv = new CSV("TestCSV.txt");
			
			// Load & Parsing
			TextAsset txt = Resources.Load<TextAsset>("Tables/Sample_CSV");
			csv.Parse(txt.text);

			{
				// Read
				foreach(uint i in csv.GetArrayUint("index"))
				{
					Debug.Log("Data [" + i +"]\n"
					          + csv.GetValueString("id", i) + "\n"
					          + csv.GetValueString("material", i) + "\n");
				}
			}
		}
	}
}
