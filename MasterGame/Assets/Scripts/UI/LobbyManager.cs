using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour {

	private static LobbyManager _instance;
	public static LobbyManager Instance {
		get { return _instance;}
	}

	public LoginManager LoginManager;
	public RegisterManager RegisterManager;
	public GameLobbyManager GameLobbyManager;
	// Use this for initialization
	void Awake () {

		if (Instance == null){
			_instance = this;
		} else {
			Destroy(gameObject);
		}
		
	}

	void Start(){
		if(GameSparksLogic.Instance.IsLoggedIn()){
			GameSparksLogic.Instance.SetAccountDetails();
			HideLoginManager();
			HideRegisterManager();
			ShowGameLobbyManager();
		} else {
			HideGameLobbyManager();
			HideRegisterManager();
			ShowLoginManager();
		}

	}
	
	// Update is called once per frame
	void Update () {

	}

	public void HideLoginManager(){
		LoginManager.gameObject.SetActive(false);
	}

	public void ShowLoginManager(){
		LoginManager.gameObject.SetActive(true);
		LoginManager.FocusFirst();
	}

	public void HideRegisterManager(){
		RegisterManager.gameObject.SetActive(false);
	}

	public void ShowRegisterManager(){
		RegisterManager.gameObject.SetActive(true);
		RegisterManager.FocusFirst();
	}

	public void ShowGameLobbyManager(){
		GameLobbyManager.Setup();
		GameLobbyManager.gameObject.SetActive(true);
	}

	public void HideGameLobbyManager(){
		GameLobbyManager.gameObject.SetActive(false);
	}

}
