using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamSelectionManager : MonoBehaviour {

	public PlayerSelectionManager[] Players = new PlayerSelectionManager[4];

	// Use this for initialization
	void Start () {

		switch(GameSparksLogic.Instance.SessionInfo.GameType){
		case GameType.Ranked1v1:
		case GameType.Unranked1v1: 
			HidePlayer(1);
			HidePlayer(2);
			HidePlayer(3);
			break;
		case GameType.Ranked2v2:
		case GameType.Unranked2v2:
			HidePlayer(2);
			HidePlayer(3);
			break;
		case GameType.Ranked4v4:
		case GameType.Unranked4v4:
			break;
		default:
			break;
		}
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void HidePlayer(int index){
		Players[index].gameObject.SetActive(false);
	}
	public void ShowPlayer(int index){
		Players[index].gameObject.SetActive(true);
	}


	public void HideAllPlayers(){
		for (int i = 0; i < Players.Length; i++){
			HidePlayer(i);
		}
	}
	public void ShowAllPlayers(){
		for (int i = 0; i < Players.Length; i++){
			ShowPlayer(i);
		}
	}

	public void SetPlayerReady(int teamPosition, bool ready){
		Players[teamPosition - 1].SetPlayerReady(ready);
	}

	public void SetPlayerAbility(int teamPosition, AbilityType abilityType, int abilityIndex){
		Players[teamPosition - 1].SetAbility(abilityType,abilityIndex);
	}

	public void SetupPlayer(Team team, int teamPosition, string displayName, bool host){

		Players[teamPosition - 1].PlayerDisplayName.text = displayName;
		Players[teamPosition - 1].Team = team;
		Players[teamPosition - 1].TeamPosition = teamPosition;
		Players[teamPosition - 1].ReadyButton.interactable = host;
		Players[teamPosition -1].RTPlayer = GameSparksLogic.Instance.SessionInfo.GetRTPlayer(team,teamPosition);

		if (host){
			Players[teamPosition - 1].EnableAbilityButtons();
		} else {
			Players[teamPosition - 1].DisableAbilityButtons();
		}

		if (team == GameSparksLogic.Instance.SessionInfo.HostPlayer.Team){
			Players[teamPosition - 1].ShowAbilityButtons();
		} else {
			Players[teamPosition - 1].HideAbilityButtons();
		}
	}

}
