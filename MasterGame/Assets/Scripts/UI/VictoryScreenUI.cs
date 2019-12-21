using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryScreenUI : MonoBehaviour {

	public Button Menu;
	public Text VictoryText;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void MenuAction(){
		GameLogic.Instance.LoadMainMenuScene();
	}

	public void SetText(string text){
		VictoryText.text = text;
	}
}
