using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameLogic : MonoBehaviour {


	public int MaxFPS;
	private static GameLogic _instance = null;
	public static GameLogic Instance {
		get { return _instance;}
	}

	private bool _isMultiplayer;
	public bool IsMultiplayer{
		get { return _isMultiplayer;}
		set { _isMultiplayer = value;}
	}

	private bool _smartCast;
	public bool SmartCast {
		get { return _smartCast;}
		set { _smartCast = value;}
	}

	// -- umesto ovoga kad dodji klase ce biti ispisane klase
	public GameObject Player;
	public GameObject Ally;
	public GameObject Opponent;

	public GameObject HealthBar;

	public GameObject FireBallBullet;
	public GameObject IceWallBullet;
	// -- end

	// Twitch API
	public TwitchLogic TwitchLogic;
	// -- end

	void Awake () {

		if (Instance == null){
			_instance = this;
			DontDestroyOnLoad(gameObject);
		} else {
			Destroy (gameObject);
		}

		Application.targetFrameRate = MaxFPS;
		_smartCast = false;

	}

	void Update(){
		if (Input.GetKeyDown(KeyCode.Y)){
			QuitGame();
		}
	}

	public void SessionStarted(){
		LoadCharacterSelectionScreen();
	}


	public void QuitGame(){
		if (!Application.isEditor)
		{
			System.Diagnostics.Process.GetCurrentProcess().Kill();
		}
	}

	public void LoadMainMenuScene(){
		if (UIManager.Instance != null){
			UIManager.Instance.SetFadeImageTransparency(0f);
			UIManager.Instance.ShowMainMenu();
			UIManager.Instance.HideFadeImage();
			UIManager.Instance.HideHUD();
			UIManager.Instance.HUD.HideAll();
		}
		ResetTimeScale();
		SceneManager.LoadScene(1);
	}

	public void LoadLobbyScene(){

		UIManager.Instance.HideMainMenu();
		SceneManager.LoadScene(2);
		ResetTimeScale();
	}

	public void LoadCharacterSelectionScreen(){

		UIManager.Instance.HideMainMenu();
		SceneManager.LoadScene(3);
		ResetTimeScale();
	}

	public void LoadBattleGroundScene(){

		UIManager.Instance.HideMainMenu();
		TwitchLogic.TwitchIRC.ConnectToTwitch();

		SceneManager.LoadScene(4);
		ResetTimeScale();
	}

	public void ResetTimeScale(){
		Time.timeScale = 1;
	}





}
