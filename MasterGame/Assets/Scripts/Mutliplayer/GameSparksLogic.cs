using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSparks.RT;
using GameSparks.Core;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using GameSparks.Api.Messages;
using System;


public class GameSparksLogic : MonoBehaviour {

	public delegate void AuthCallback(AuthenticationResponse authRespone);
	public delegate void RegCallback(RegistrationResponse regResponse);

	private static GameSparksLogic _instance;
	public static GameSparksLogic Instance {
		get { return _instance; }
	}

	private RTSessionInfo _sessionInfo;
	public RTSessionInfo SessionInfo{
		get { return _sessionInfo;}
		set { _sessionInfo = value; }
	}

	private GameSparksRTUnity _session;
	public GameSparksRTUnity Session {
		get { return _session;}
	}

	private string _userDisplayName;
	public string UserDisplayName{
		get{ return _userDisplayName;}
	}

	private string _userId;
	public string UserId{
		get { return _userId;}
	}

	private float _updateRate = 0.015f;
	public float UpdateRate{
		get { return _updateRate; }
	}

	void Awake(){

		if (Instance == null){
			_instance = this;
			DontDestroyOnLoad(gameObject);
		} else {
			Destroy(gameObject);
		}

	}

	void Start(){

		LogoutUser();

		GameSparks.Api.Messages.MatchNotFoundMessage.Listener = MatchNotFound;
		GameSparks.Api.Messages.MatchFoundMessage.Listener = MatchFound; 

		StartCoroutine(SendTimeStampMessage());

	}



	#region GameSparks API

	public void RegisterPlayer(string displayName, string username, string password,RegCallback regCallback){
		
		new GameSparks.Api.Requests.RegistrationRequest()
			.SetDisplayName(displayName)
			.SetUserName(username)
			.SetPassword(password)
			.Send((response) => {
				if (!response.HasErrors) {
					Debug.Log("Player Registered");
					regCallback(response);
				} else {
					Debug.Log("Error Registering Player " + response.Errors.JSON);
				}
			});

	}

	public void AuthenticatePlayerByUsernameAndPass(string username, string password, AuthCallback authCallback){

		new GameSparks.Api.Requests.AuthenticationRequest()
			.SetUserName(username)
			.SetPassword(password)
			.Send((response) => {
				if (!response.HasErrors) {
					Debug.Log("Player Authenticated ");
					_userId = response.UserId;
					_userDisplayName = response.DisplayName;
					authCallback(response);
				} else {
					Debug.Log("Error Authenticating Player " + response.Errors.JSON);
				}
			});
		
	}

	public void AuthenticatePlayerByDevice(AuthCallback authCallback){

		new GameSparks.Api.Requests.DeviceAuthenticationRequest()
			.SetDisplayName("Randy")
			.Send((response) => {
				if (!response.HasErrors) {
					Debug.Log("Device Authenticated...");
					authCallback(response);
				} else {
					Debug.Log("Error Authenticating Device " + response.Errors.JSON);
				}
			});

	}


	public void LogoutUser(){
		GameSparks.Core.GS.Reset();
	}

	public bool IsLoggedIn(){
		return GameSparks.Core.GS.Authenticated;
	}

	public void SetAccountDetails(){

		if (GameSparks.Core.GS.Authenticated){

			new GameSparks.Api.Requests.AccountDetailsRequest()
				.Send((response) => {
					if (!response.HasErrors){
						_userDisplayName = response.DisplayName;
						_userId = response.UserId;
					} else {
						Debug.Log("Error Getting Account Details " +  response.Errors.JSON);
					}
				});
		}
	}

	public void FindMatch(GameType matchType){ // MatchFound listened MatchFound && MatchNotFound

		string matchShortCode = GetMatchShortCode(matchType);

		new GameSparks.Api.Requests.MatchmakingRequest ()
			.SetMatchShortCode (matchShortCode)
			.SetSkill (0) // in this case we assume all players have skill level zero and we want anyone to be able to join so the skill level for the request is set to zero
			.Send ((response) => {
				if(response.HasErrors){ // check for errors
					Debug.LogError("GSM| MatchMaking Error \n"+response.Errors.JSON);
				}
			});

	}

	public void CancelFindMatch(GameType matchType){

		string matchShortCode = GetMatchShortCode(matchType);

		new GameSparks.Api.Requests.MatchmakingRequest()
			.SetAction("cancel")
			.SetMatchShortCode(matchShortCode)
			.Send((response) => {
				if (!response.HasErrors) {
					Debug.Log ("Match canceled..."); 
					LobbyManager.Instance.GameLobbyManager.MatchNotFound();
				} else {
					Debug.LogError("Error Canceling Find Match :" + response.Errors.JSON);
				}
		});
	}

	public void MatchFound(MatchFoundMessage message){
		LobbyManager.Instance.GameLobbyManager.MatchFound();

		GameType gameType = GetGameType(message.MatchShortCode);
		RTSessionInfo sessionInfo = new RTSessionInfo(message,gameType);
		StartNewRTSession(sessionInfo);
	}

	public void MatchNotFound(MatchNotFoundMessage message){
		LobbyManager.Instance.GameLobbyManager.MatchNotFound();
	}

	public void StartNewRTSession(RTSessionInfo info){

		Debug.Log("GSM| Creating New RT Session Instance...");
		_sessionInfo = info;
		_sessionInfo.SetHostPlayer(_userId);
		_session = gameObject.GetComponent<GameSparksRTUnity>();

		GSRequestData mockedResponse = new GSRequestData()
			.AddNumber("port", (double)_sessionInfo.PortID)
			.AddString("host", _sessionInfo.HostURL)
			.AddString("accessToken", _sessionInfo.AccessToken);


		FindMatchResponse response = new FindMatchResponse(mockedResponse);

		_session.Configure(response,
		    (peerId) =>  { OnPlayerConnectedToGame(peerId);},
			(peerId) => { OnPlayerDisconnected(peerId);},
			(ready) => { OnRTReady(ready);},
			(packet) => { OnPacketReceived(packet);}
		);
		_session.Connect(); // when the config is set, connect the game
	}

	public void DisconnectFromRTSession(){
		Session.Disconnect();
		BattleLogic.Instance.EndBattle();
	}

	private void OnPlayerConnectedToGame(int peerId){
		Debug.Log ("GSM| Player Connected, " + peerId);
	}

	private void OnPlayerDisconnected(int peerId){
		Debug.Log ("GSM| Player Disconnected, " + peerId);
		BattleLogic.Instance.PlayerDisconnected(peerId);
	}

	private void OnRTReady(bool _isReady){
		if (_isReady) {
			GameLogic.Instance.SessionStarted();
		}
		
	}

	#endregion

	private void OnPacketReceived(RTPacket _packet){

		// Ability Selection
		switch (_packet.OpCode){
		case 9: {
			int peerId = _packet.Sender;
			AbilityType abilityType = (AbilityType) _packet.Data.GetInt(1);
			int abilityIndex = (int) _packet.Data.GetInt(2);

			RTPlayer rtPlayer = SessionInfo.GetRTPlayer(peerId);
			CharacterSelectionManager.Instance.SetPlayerAbility(rtPlayer.Team,rtPlayer.TeamPosition,abilityType,abilityIndex);

			break;
		}
		//Player Selection Ready
		case 10: {
			int peerId = (int)_packet.Data.GetInt(1);
			RTPlayer rtPlayer = SessionInfo.GetRTPlayer(peerId);
			CharacterSelectionManager.Instance.SetPlayerReady(rtPlayer.Team,rtPlayer.TeamPosition,true);
			break;
		}
		//Player Selection Not Ready
		case 11: { 
			int peerId = (int)_packet.Data.GetInt(1);
			RTPlayer rtPlayer = SessionInfo.GetRTPlayer(peerId);
			CharacterSelectionManager.Instance.SetPlayerReady(rtPlayer.Team,rtPlayer.TeamPosition,false);
			break;
		}
		// All players are ready in selection screen
		case 12: {
			CharacterSelectionManager.Instance.AllPlayersReady();
			break;
		}
		// All players have loaded the BattleGround scene
		case 13: {
			BattleLogic.Instance.AllPlayersLoaded();
			break;
		}
		// Pause Game
		case 14: {
			BattleLogic.Instance.PauseGame();
			break;
		}
		// Resume Game
		case 15: {
			BattleLogic.Instance.ResumeGame();
			break;
		}
		// GameOver 
		case 16: {
			int winner = (int) _packet.Data.GetInt(1);
			BattleLogic.Instance.GameOver((Team)winner);
			break;
		}
		// Recieve character movement info
		case 20: {
			int peerId = (int) _packet.Data.GetInt(1);
			Vector4 movement = (Vector4) _packet.Data.GetVector4(2);
			float rotationZ = (float) _packet.Data.GetFloat(3);

			RTPlayer rtPlayer = SessionInfo.GetRTPlayer(peerId);
			AllyController allyController = (AllyController) rtPlayer.CharacterController;
			if (allyController != null){
				allyController.MovementUpdate(new Vector2(movement.x, movement.y), new Vector2(movement.z,movement.w),rotationZ);
			}
			break;
		}
		// Ability Used Recieved
		case 21: {

			int peerId = _packet.Sender;
			RTPlayer rtPlayer = SessionInfo.GetRTPlayer(peerId);
			AllyController ac = (AllyController) rtPlayer.CharacterController;

			Vector4 positionInfo = (Vector4) _packet.Data.GetVector4(1);
			AbilityType abilityType = (AbilityType)((int)_packet.Data.GetInt(2));
			int targetPeerId = (int) _packet.Data.GetInt(3);
			int bulletId = (int) _packet.Data.GetInt(4);

			switch (abilityType){
			case AbilityType.AbilityQ:
				ac.AbilityQ.CreateProjectile(new Vector2(positionInfo.x,positionInfo.y), new Vector2(positionInfo.z,positionInfo.w),targetPeerId,bulletId, peerId);
				break;
			case AbilityType.AbilityE:
				ac.AbilityE.CreateProjectile(new Vector2(positionInfo.x,positionInfo.y), new Vector2(positionInfo.z,positionInfo.w),targetPeerId,bulletId, peerId);
				break;
			case AbilityType.AbilitySpace:
				ac.AbilitySpace.CreateProjectile(new Vector2(positionInfo.x,positionInfo.y), new Vector2(positionInfo.z,positionInfo.w),targetPeerId,bulletId, peerId);
				break;
			default:
				Debug.Log ("On packet recieved ability type is default: " + abilityType);
				break;
			}
			break;
		}
		// Ability Bullet Hit Player 
		case 22: {

			int peerId = _packet.Sender;

			int bulletId = (int) _packet.Data.GetInt(1);
			int targetId = (int) _packet.Data.GetInt(2);

			RTPlayer rTPlayer = SessionInfo.GetRTPlayer(targetId);
			CharController cc = rTPlayer.CharacterController;
			cc.TakeDamage(5f);
			BattleLogic.Instance.BulletContainer.DestroyAbilityBullet(bulletId,peerId);
			break;
		}
		// TwitchEvent Started
		case 23: {

			int numberOfChoices = (int) _packet.Data.GetInt(1);
			List<CharacterEffects> twitchEffects = new List<CharacterEffects>();

			for (int i = 0; i < numberOfChoices; i++){
				CharacterEffects ce =(CharacterEffects)((int)_packet.Data.GetInt((uint)(i+2)));
				twitchEffects.Add(ce);
			}

			BattleLogic.Instance.StartTwitchEvent(twitchEffects);
			break;
		}
		// TwitchEventEnded
		case 24: {

			BattleLogic.Instance.EndTwitchEvent();
			break;
		}
		// Twitch Vote Count
		case 25: {

			int[] votes = new int[GameLogic.Instance.TwitchLogic.CurrentEventOptionCount];
			for (int i = 0; i < votes.Length; i++){
				votes[i] = (int) _packet.Data.GetInt((uint)(i + 1));
			}
			GameLogic.Instance.TwitchLogic.SetOtherPlayerPoints(_packet.Sender,votes);
			break;
		}
		// Character Died
		case 26: {
			int peerId = _packet.Sender;
			RTPlayer rTPlayer = SessionInfo.GetRTPlayer(peerId);
			CharController cc = rTPlayer.CharacterController;
			cc.Die();
			break;
		}
		// Test message
		case 99: {
			Debug.Log ("Stigla test poruka");
			break;
		}
		// Timestamp message
		case 101: {
			long clientTime = (long) _packet.Data.GetLong(1);
			long serverTime = (long) _packet.Data.GetLong(2);
			ServerUI.Instance.CalculateTimeDelta(clientTime,serverTime);
			break;
		}
		default: 
			Debug.Log ("Packet Recieved Unknown code:" + _packet.OpCode);
			break;
		}
	}



	#region GameSparks Latency && ServerTime

	public IEnumerator SendTimeStampMessage(){

		if ( _session != null){
			using (RTData data = RTData.Get ()) {
				data.SetLong (1, (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds); // get the current time as unix timestamp, send at index 2
				GameSparksRTUnity.Instance.SendData(101, GameSparksRT.DeliveryIntent.UNRELIABLE, data, new int[]{ 0 }); // code 101 timestamp
			}
		}

		yield return new WaitForSeconds(1f);
		StartCoroutine(SendTimeStampMessage());

	}
	
	#endregion
	
	#region GameSparks Messages

	public void SendTestMessage(){

		using (RTData data = RTData.Get()){
			data.SetInt(1,10);
			GameSparksRTUnity.Instance.SendData(99, GameSparksRT.DeliveryIntent.RELIABLE, data); // code 99 for test
		}
	}

	public void SendPlayerAbilitySelected(Team team, int teamPosition, AbilityType abilityType, int abilityIndex){
		
		using (RTData data = RTData.Get()){
			data.SetInt(1, (int)abilityType);
			data.SetInt(2, abilityIndex);
			GameSparksRTUnity.Instance.SendData(9,GameSparksRT.DeliveryIntent.RELIABLE, data); // code 9 for player ability selected
		}
	}

	public void SendPlayerSelectionReady(Team team, int teamPosition){

		int peerId = SessionInfo.PeerId(team,teamPosition);
		using (RTData data = RTData.Get()) {	
			data.SetInt (1, peerId); // add int at index 1
			GameSparksRTUnity.Instance.SendData(10, GameSparksRT.DeliveryIntent.RELIABLE, data); // code 10 for player ready
		}
	}


	public void SendPlayerSelectionNotReady(Team team, int teamPosition){

		int peerId = SessionInfo.PeerId(team,teamPosition);
		using (RTData data = RTData.Get()) {	
			data.SetInt (1, peerId);
			GameSparksRTUnity.Instance.SendData(11, GameSparksRT.DeliveryIntent.RELIABLE, data); // code 11 for player not ready
		}
	}

	public void SendBattleGroundSceneLoaded(){

		int peerId = SessionInfo.HostPlayer.PeerId;
		using (RTData data = RTData.Get()) {
			data.SetInt(1,peerId);
			GameSparksRTUnity.Instance.SendData(13, GameSparksRT.DeliveryIntent.RELIABLE, data, new int[]{0}); // code 13 for battle scene loaded
		}
	}

	public void SendPlayerMovement(Vector4 movement,float rotationZ){

		int peerId = SessionInfo.HostPlayer.PeerId;
		using (RTData data = RTData.Get ()) {  // we put a using statement here so that we can dispose of the RTData objects once the packet is sent
			data.SetInt(1,peerId);
			data.SetVector4 (2,movement); // add the position at key 1
			data.SetFloat (3, rotationZ); // add the rotation at key 2
			GameSparksRTUnity.Instance.SendData(20, GameSparks.RT.GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);// code 20 for movement message
		}
	}

	public void SendAbilityUsedMessage(Vector2 myPosition, Vector2 targetPosition, AbilityType abilityType, int targetPeerId, int bulletId){

		using (RTData data = RTData.Get()) {
			Vector4 positionVector = new Vector4(myPosition.x,myPosition.y,targetPosition.x,targetPosition.y);
			data.SetVector4(1, positionVector);
			data.SetInt(2, (int) abilityType);
			data.SetInt(3, targetPeerId);
			data.SetInt(4, bulletId);
			GameSparksRTUnity.Instance.SendData(21, GameSparks.RT.GameSparksRT.DeliveryIntent.UNRELIABLE, data); // code 21 for ability used message
		}

	}

	public void SendBulletHitPlayerMessage(int bulletId, int targetPeerId){
		using (RTData data = RTData.Get()) {
			data.SetInt(1, bulletId);
			data.SetInt(2, targetPeerId);
			GameSparksRTUnity.Instance.SendData(22, GameSparks.RT.GameSparksRT.DeliveryIntent.UNRELIABLE, data); // code 22 for bullet hit player message
		}
	}

	public void SendPauseGameMessage(){
		using (RTData data = RTData.Get()) {
			GameSparksRTUnity.Instance.SendData(14, GameSparks.RT.GameSparksRT.DeliveryIntent.RELIABLE, data, new int[]{ 0 }); // code 14 for PAUSE game
		}
	}

	public void SendResumeGameMessage(){
		using (RTData data = RTData.Get()) {
			GameSparksRTUnity.Instance.SendData(15, GameSparks.RT.GameSparksRT.DeliveryIntent.RELIABLE, data, new int[]{ 0 }); // code 15 for RESUME game
		}
	}

	public void SendPlayerDeadMessage(){ // only sent by Player - Host
		using (RTData data = RTData.Get()) {
			RTPlayer rtp = SessionInfo.HostPlayer;
			data.SetInt(1, (int)rtp.Team);
			data.SetInt(2, (int)rtp.TeamPosition);
			GameSparksRTUnity.Instance.SendData(26, GameSparks.RT.GameSparksRT.DeliveryIntent.RELIABLE, data); // code 26 for player dead
		}
	}

	public void SendTwitchVotesMessage(int[] voteCount){
		using (RTData data = RTData.Get()){

			for (int i = 1; i <= GameLogic.Instance.TwitchLogic.CurrentEventOptionCount; i++){
				data.SetInt((uint)i, voteCount[i-1]);
			}
			GameSparksRTUnity.Instance.SendData(25, GameSparks.RT.GameSparksRT.DeliveryIntent.RELIABLE, data); // code 25 for send twitch votes
		}
	}

	#endregion

	public GameType GetGameType(string matchShortCode){
		
		GameType retVal = GameType.None;

		switch(matchShortCode){
		case "unranked1v1" : retVal = GameType.Unranked1v1; break;
		case "unranked2v2" : retVal = GameType.Unranked2v2; break;
		case "unranked4v4" : retVal = GameType.Unranked4v4; break;
		case "ranked1v1" : retVal = GameType.Ranked1v1; break;
		case "ranked2v2" : retVal = GameType.Ranked2v2; break;
		case "ranked4v4" : retVal = GameType.Ranked4v4; break;
		default : retVal = GameType.None; break;
		}
		
		return retVal;
	}

	public string GetMatchShortCode(GameType gameType){

		string retVal = "";

		switch(gameType){
		case GameType.Ranked1v1: retVal = "ranked1v1"; break;
		case GameType.Ranked2v2: retVal = "ranked2v2"; break;
		case GameType.Ranked4v4: retVal = "ranked4v4"; break;
		case GameType.Unranked1v1: retVal = "unranked1v1"; break;
		case GameType.Unranked2v2: retVal = "unranked2v2"; break;
		case GameType.Unranked4v4: retVal = "unranked4v4"; break;
		default: Debug.Log ("Wrong game type for multiplayer"); break;
		}

		return retVal;
	}

}
