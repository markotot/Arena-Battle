using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwitchLogic : MonoBehaviour {

	public TwitchIRC TwitchIRC;

	private CharacterEffects[] _optionNames = new CharacterEffects[11];
	private CharacterEffects[] OptionNames {
		get { return _optionNames; }
		set { _optionNames = value; }
	}

	private int[] _voteCount = new int[11];
	public int[] VoteCount{
		get { return _voteCount;}
		set { _voteCount = value;}
	}

	private Dictionary<int,int[]> _otherPlayersVotes = new Dictionary<int,int[]>();
	public Dictionary<int,int[]> OtherPlayerVotes {
		get { return _otherPlayersVotes;}
		set { _otherPlayersVotes = value;}
	}

	private int _currentEventOptionCount;
	public int CurrentEventOptionCount{
		get { return _currentEventOptionCount;}
	}

	private bool _chatEventActive;
	public bool ChatEventActive{
		get { return _chatEventActive; }
	}

	// Use this for initialization
	void Start () {

		_currentEventOptionCount = -1;
		_chatEventActive = false;
		TwitchIRC.messageRecievedEvent.AddListener(OnChatMsgRecieved);		
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void StartEvent(List<CharacterEffects> choices, float duration, float delay){


		_currentEventOptionCount = choices.Count;
		_chatEventActive = true;
		ResetResults();
		SetupOtherPlayers();
		SetOptionNames(choices);
		UIManager.Instance.SetTwitchEffectIcons(choices);
		SendEventMessageToTwitch();
		StartCoroutine(SendResults());
	}

	public void EndEvent(){

		_chatEventActive = false;
		UIManager.Instance.HideTwitchEffectIcons();
		StartCoroutine(ShowWinnerEffect());
	}

	public void SetupOtherPlayers(){

		_otherPlayersVotes = new Dictionary<int, int[]>();
		RTSessionInfo sessionInfo = GameSparksLogic.Instance.SessionInfo;

		foreach(RTPlayer rtp in sessionInfo.PlayerList){
			if (rtp.PeerId != sessionInfo.HostPlayer.PeerId){
				_otherPlayersVotes.Add(rtp.PeerId,new int[_currentEventOptionCount]);
			}
		}

	}

	public void SendEventMessageToTwitch(){
		string msg = "Event started vote for you choice:    ";

		for (int i = 0; i < _currentEventOptionCount; i++){
			msg += (i + 1) + "." + EffectToNames(OptionNames[i]) + " "; 
		}
		SendMsg(msg);
	}

	public IEnumerator SendResults(){
		while (_chatEventActive){
			GameSparksLogic.Instance.SendTwitchVotesMessage(_voteCount);
			yield return new WaitForSeconds(2f);
		}
		GameSparksLogic.Instance.SendTwitchVotesMessage(_voteCount);
	}

	public void ShowAllResults(){
		ShowResults();

		Debug.Log("Other playes:\n");
		foreach(KeyValuePair<int,int[]> votes in OtherPlayerVotes){
			Debug.Log("PeerId:" + votes.Key + " \n");
			for(int i = 0; i < _currentEventOptionCount; i++){
				Debug.Log(OptionNames[i] + ": " + votes.Value[i] + "\n");
			}
		}
	}

	public Team GetWinnerForEffect(int index){

		int redTeam = 0;
		int blueTeam = 0;

		Team retVal = Team.None;

		foreach (KeyValuePair<int, int[]> votes in _otherPlayersVotes){
			RTPlayer player = GameSparksLogic.Instance.SessionInfo.GetRTPlayer(votes.Key);
			if (player.Team == Team.Red){
				redTeam += votes.Value[index];
			} else if (player.Team == Team.Blue){
				blueTeam += votes.Value[index];
			}
		}

		RTPlayer hostPlayer = GameSparksLogic.Instance.SessionInfo.HostPlayer;
		for (int i = 0; i < _currentEventOptionCount; i++){

			if (hostPlayer.Team == Team.Red) {
				redTeam += _voteCount[i];
			} else if (hostPlayer.Team == Team.Blue){
				blueTeam += _voteCount[i];
			}
		}

		if (redTeam > blueTeam){
			retVal = Team.Red;
		} else if (redTeam < blueTeam){
			retVal = Team.Blue;
		} else {
			retVal = Team.None;
		}

		return retVal;
	}

	public int GetVoteCountForEffect(Team t, int index){

		int count = 0;
		foreach (KeyValuePair<int, int[]> votes in _otherPlayersVotes){
			RTPlayer player = GameSparksLogic.Instance.SessionInfo.GetRTPlayer(votes.Key);
			if (player.Team == t){
				count += votes.Value[index];
			}
		}

		RTPlayer hostPlayer = GameSparksLogic.Instance.SessionInfo.HostPlayer;
		if (hostPlayer.Team == t){
			count += _voteCount[index];
		}
		return count;
	}

	public IEnumerator ShowWinnerEffect(){

		yield return new WaitForSeconds(1f);

		for (int i = 0; i < _currentEventOptionCount; i++){

			Team winner = GetWinnerForEffect(i);
			int voteCountRed = GetVoteCountForEffect(Team.Red,i);
			int voteCountBlue = GetVoteCountForEffect(Team.Blue,i);

			UIManager.Instance.SetAndShowWinnerEffect(winner, voteCountRed, voteCountBlue, i);
			yield return new WaitForSeconds(2f);

			UIManager.Instance.HideWinnerEffect();
			yield return new WaitForSeconds(1f);
		}

		ResetResults();
		ResetOptionNames();
		_currentEventOptionCount = -1;

		UIManager.Instance.ResetNumbers();
		UIManager.Instance.ResetAndHideWinnerEffect();

	}

	void OnChatMsgRecieved(string msg)
	{
		if (_chatEventActive){
			int msgIndex = msg.IndexOf("PRIVMSG #"); 
			string content = msg.Substring(msgIndex + TwitchIRC.channelName.Length + 11);		
			ParseMessageRecieved(content);
		}
	}

	public void SendMsg(string msg){
		TwitchIRC.SendMsg(msg);
	}

	public void SetOptionNames(List<CharacterEffects> effects){

		ResetOptionNames();

		for (int i = 0; i < effects.Count; i++){
			_optionNames[i] = effects[i];
		}
	}

	public void SetOtherPlayerPoints(int peerId, int[] points){
		for (int i = 0; i < _currentEventOptionCount; i++){
			(_otherPlayersVotes[peerId])[i] = points[i];
		}
	}

	public void ResetOptionNames(){
		for (int i = 0; i < _optionNames.Length; i++){
			_optionNames[i] = CharacterEffects.None;
		}
	}

	public void ResetResults(){
		for (int i = 0; i < _optionNames.Length; i++){
			_voteCount[i] = 0;
		}

		_otherPlayersVotes = new Dictionary<int, int[]>();
	}

	public void ParseMessageRecieved(string msg){
		
		int result;
		if (int.TryParse(msg.Trim(),out result)){
			if (result > 0 && result <= _currentEventOptionCount && _chatEventActive){
				_voteCount[result - 1]++;
				UIManager.Instance.IncreaseNumber(result - 1);
			}
		}
		
	}

	public void ShowResults(){
		for (int i = 0; i < _currentEventOptionCount; i++){
			Debug.Log ( _optionNames[i] + " -- " + _voteCount[i]);
		}
	}

	public void ActiveEffect(int choice, int peerId){

		CharController character = GameSparksLogic.Instance.SessionInfo.GetRTPlayer(peerId).CharacterController;
		switch(OptionNames[choice]){

		case CharacterEffects.IncMovementSpeed: 	character.IncreaseMovementSpeed(1f); break;
		case CharacterEffects.DecAbilityECooldown: 	character.DecreaseAbilityECooldown(1f); break;
		case CharacterEffects.DecAbilityQCooldown: 	character.DecreaseAbilityQCooldown(1f); break;
		case CharacterEffects.DecDashCooldown: 		character.DecreaseAbilitySpaceCooldown(0.5f); break;
		case CharacterEffects.IncWeaponDamage:		//character.IncreaseWeaponDamage(); break;
		case CharacterEffects.IncAttackSpeed: 		//character.IncreaseAttackSpeed(); break;
		case CharacterEffects.IncBulletSpeed: 		//character.IncreaseBulletSpeed(); break;
		case CharacterEffects.IncHitPoints: 		character.HealDamage(character.MaxHP); break;
		case CharacterEffects.IncArmor: 			//character.IncreaseArmor(); break;
		case CharacterEffects.IncScore: 			//character.IncreaseScorePoints(10f); break;
		default:
			break;

		}
	}

	public List<CharacterEffects> getRandomEffects(int count, List<CharacterEffects> currentEffects = null){
				
		if (currentEffects == null){
			currentEffects = new List<CharacterEffects>();
		}

		int currentEffectsCount = currentEffects.Count;
		int enumCount = System.Enum.GetValues(typeof(CharacterEffects)).Length;

		while (currentEffects.Count < count){

			CharacterEffects randomEffect = (CharacterEffects)Random.Range(1,enumCount);
			if (!currentEffects.Contains(randomEffect)){
				currentEffects.Add(randomEffect);
			}
		}

		return currentEffects;
	}

	public string EffectToNames(CharacterEffects ce){

		string retVal = "";

		switch(ce){
		case CharacterEffects.IncMovementSpeed: 	retVal = "Increase movement speed"; break;
		case CharacterEffects.DecAbilityECooldown: 	retVal = "Decrease AbilityE cooldown"; break;
		case CharacterEffects.DecAbilityQCooldown: 	retVal = "Decrease AbilityQ cooldown"; break;
		case CharacterEffects.DecDashCooldown: 		retVal = "Decrease Dash cooldown"; break;
		case CharacterEffects.IncWeaponDamage:		retVal = "Increase Weapon damage"; break;
		case CharacterEffects.IncAttackSpeed: 		retVal = "Increase Attack speed"; break;
		case CharacterEffects.IncBulletSpeed: 		retVal = "Increase Bullet speed"; break;
		case CharacterEffects.IncHitPoints: 		retVal = "Increase HP"; break;
		case CharacterEffects.IncArmor: 			retVal = "Increase Armor"; break;
		case CharacterEffects.IncScore: 			retVal = "Increase Score"; break;
		default:
			break;
			
		}

		return retVal;
	}

}
