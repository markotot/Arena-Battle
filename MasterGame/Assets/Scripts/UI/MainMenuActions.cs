using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuActions : MonoBehaviour {
	


	public void ClickSinglePlayer(){

		GameLogic.Instance.IsMultiplayer = false;
		GameLogic.Instance.LoadBattleGroundScene();
	}

	public void ClickMultiPlayer(){
		GameLogic.Instance.IsMultiplayer = true;
		GameLogic.Instance.LoadLobbyScene();
	}

	public void ClickSettings(){

	}

	public void ClickTwitch(){

	}

	public void ClickQuit(){
		GameLogic.Instance.QuitGame();
	}
}
