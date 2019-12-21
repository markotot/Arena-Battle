using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState {
	Intro = 0,
	Playing = 1,
	Paused = 2,
	GameOver = 3
}

public class BattleLogic : MonoBehaviour {

	private static BattleLogic _instance;
	public static BattleLogic Instance{
		get { return _instance;}
	}

	private GameState _gameState;
	public GameState GameState {
		get { return _gameState; }
		set { _gameState = value; }
	}

	private PlayerController _playerController;
	public PlayerController PlayerController{
		get { return _playerController; }
	}

	private List<AllyController> FriendlyControllers;
	private List<AllyController> OpponentControllers;

	public BulletContainer BulletContainer;
	public GameObject CharacterContainer;
	public BattleField BattleField;
	public MapManager MapManager;
	public DynamicSorting DynamicSorting;

	private Vector2 _mouseWorldPosition;
	public Vector2 MouseWorldPosition{
		get { return _mouseWorldPosition;}
	}
	private float _playerToMouseDistance;
	public float PlayerToMouseDistance{
		get { return _playerToMouseDistance;}
	}


	void Awake(){

		_instance = this;
		_gameState = GameState.Intro;

	}
	// Use this for initialization
	void Start () {

		Time.timeScale = 1f;

		FriendlyControllers = new List<AllyController>();
		OpponentControllers = new List<AllyController>();

		_gameState = GameState.Intro;
		UIManager.Instance.ShowFadeImage();

		if (GameLogic.Instance.IsMultiplayer){
			GameSparksLogic.Instance.SendBattleGroundSceneLoaded();
		} else {
			AllPlayersLoaded();
		}

	}

	// Update is called once per frame
	void Update () {

		if (PlayerController != null){
			_mouseWorldPosition = mouseScreenToWorldPoint(Input.mousePosition);
			_playerToMouseDistance = playerToMouseDistance();
		}

		if (GameState != GameState.Intro){

			// PAUSE GAME
			if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.F10)){
				if (GameLogic.Instance.IsMultiplayer){
					if (_gameState == GameState.Playing){
						GameSparksLogic.Instance.SendPauseGameMessage();
					} else if (_gameState == GameState.Paused){
						GameSparksLogic.Instance.SendResumeGameMessage();
					}
				} else {
					if (_gameState == GameState.Playing){
						PauseGame();
					} else {
						ResumeGame();
					}
				}
			}

			// QUIT GAME
			if (Input.GetKeyDown(KeyCode.Z)){
				if (GameLogic.Instance.IsMultiplayer){
					GameSparksLogic.Instance.DisconnectFromRTSession();
				} else {
					EndBattle();
				}
			}
		}
		
	}

	public void PauseGame(){

		_gameState = GameState.Paused;
		Time.timeScale = 0;
		UIManager.Instance.HUD.ShowInGameMenu();
		UIManager.Instance.SetFadeImageTransparency(0.5f);
	}

	public void ResumeGame(){

		_gameState = GameState.Playing;
		Time.timeScale = 1;
		UIManager.Instance.HUD.HideInGameMenu();
		UIManager.Instance.SetFadeImageTransparency(0f);
	}

	public void GameOver(Team winner){

		_gameState = GameState.GameOver;
		Time.timeScale = 0;
		string text = null;
		if (winner == Team.None){
			text = "Draw!";
		}else if (winner == GameSparksLogic.Instance.SessionInfo.HostPlayer.Team){
			text = "Victory!";
		} else {
			text = "Defeat!";
		}
		UIManager.Instance.HUD.VictoryScreenUI.SetText(text);
		UIManager.Instance.HUD.ShowVictoryScreen();
	}

	public void StartTwitchEvent(List<CharacterEffects> effects, int duration = 30, int delay = 0){
		GameLogic.Instance.TwitchLogic.StartEvent(effects, duration, delay);
	}

	public void EndTwitchEvent(){
		GameLogic.Instance.TwitchLogic.EndEvent();
	}

	public void EndBattle(){
		
		if (GameLogic.Instance.IsMultiplayer){
			GameLogic.Instance.LoadLobbyScene();
		} else {
			GameLogic.Instance.LoadMainMenuScene();
		}

	}

	public void PlayerDisconnected(int peerId){
		RTPlayer rtp = GameSparksLogic.Instance.SessionInfo.GetRTPlayer(peerId);
		Destroy(rtp.CharacterController.CharacterRigidbody.gameObject);
	}

	public void AllPlayersLoaded(){

		if (GameLogic.Instance.IsMultiplayer){
			SpawnAllCharacters();
		} else {
			SpawnCharactersSinglePlayer();
		}

		StartCoroutine(UIManager.Instance.FadeInLevel(3));
	}

	public void SpawnAllCharacters(){
		foreach(RTPlayer rtp in GameSparksLogic.Instance.SessionInfo.PlayerList){
			SpawnCharacter(rtp);
		}
	}

	private void SpawnCharacter(RTPlayer rtp){

		Vector3 spawnPosition = Vector3.zero;
		if (rtp.Team == Team.Red){
			spawnPosition = BattleField.RedSpawnPositions[rtp.TeamPosition - 1];
		} else if (rtp.Team == Team.Blue){
			spawnPosition = BattleField.BlueSpawnPositions[rtp.TeamPosition - 1];
		}

		if (rtp == GameSparksLogic.Instance.SessionInfo.HostPlayer){
			CreatePlayer(rtp,spawnPosition);
		} else {
			CreateAlly(rtp,spawnPosition);
		}
	}

	private void CreatePlayer(RTPlayer rtp, Vector3 spawnPosition){

		GameObject player = (GameObject) Instantiate (GameLogic.Instance.Player, spawnPosition, transform.rotation);
		player.transform.parent = CharacterContainer.transform;
		_playerController = player.GetComponentInChildren<PlayerController>();
		_playerController.SetupAbilities(rtp);
		rtp.CharacterController = _playerController;
	}

	private void CreateAlly(RTPlayer rtp, Vector3 spawnPosition){

		GameObject character = null;
		if (rtp.Team == GameSparksLogic.Instance.SessionInfo.HostPlayer.Team){
			character = (GameObject) Instantiate (GameLogic.Instance.Ally, spawnPosition, transform.rotation);
		} else {
			character = (GameObject) Instantiate (GameLogic.Instance.Opponent, spawnPosition, transform.rotation);
		}

		character.transform.parent = CharacterContainer.transform;
		AllyController allyController = character.GetComponentInChildren<AllyController>();
		allyController.SetupAbilities(rtp);
		rtp.CharacterController = allyController;
		if (rtp.Team == GameSparksLogic.Instance.SessionInfo.HostPlayer.Team){
			FriendlyControllers.Add(allyController);
		} else {
			OpponentControllers.Add(allyController);
		}
	}



	private void SpawnCharactersSinglePlayer(){
		SpawnPlayer(gameObject.transform.position);
	}


	private void SpawnPlayer(Vector3 spawnPosition){
		GameObject player;
		player = (GameObject)Instantiate (GameLogic.Instance.Player, spawnPosition, transform.rotation);
		player.transform.parent = gameObject.transform;
		_playerController = player.GetComponentInChildren<PlayerController>();
		
	}

	public Vector2 playerToMouseVector(){

		Vector2 playerPosition = PlayerController.CharacterRigidbody.transform.position;
		return (_mouseWorldPosition - playerPosition).normalized;
	}

	public Vector2 positionRelativeToPlayer(Vector3 position){
		return PlayerController.CharacterRigidbody.transform.position + position;
	}

	private float playerToMouseDistance(){

		Vector2 playerPosition = PlayerController.CharacterRigidbody.transform.position;
		return Vector2.Distance(playerPosition,_mouseWorldPosition);
	}

	private Vector3 mouseScreenToWorldPoint(Vector3 mousePosition){
		return Camera.main.ScreenToWorldPoint(Input.mousePosition);
	}

	public float playerToMouseAngle(){
		Vector2 vector = playerToMouseVector();
		return Vector2.SignedAngle(Vector2.right, vector);

	}

	public Quaternion addAngleToRotation(float angle, Quaternion quaternion){
		return Quaternion.Euler(quaternion.eulerAngles.x,quaternion.eulerAngles.y,quaternion.eulerAngles.z + angle);
	}

}
