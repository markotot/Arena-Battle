using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour {


	public Button Resume;
	public Button SettingsButton;
	public Button QuitButton;

	public void ResumeButtonAction(){
		BattleLogic.Instance.ResumeGame();
	}

	public void SettingsButtonAction(){

	}

	public void QuitButtonAction(){
		UIManager.Instance.HUD.HideInGameMenu();
		BattleLogic.Instance.EndBattle();
	}

}
