using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSparks.Api.Messages;

public class GameLobbyManager : ISelectableManager {

	public Text DisplayNameText;
	public Text SearchingText;

	public Button Ranked1v1Button;
	public Button Ranked2v2Button;
	public Button Ranked4v4Button;
	public Button Unranked1v1Button;
	public Button Unranked2v2Button;
	public Button Unranked4v4Button;

	public Button Back;
	public Button Logout;
	public Button Cancel;

	private bool _startSearching;
	private bool _matchFound;

	private GameType _searchingForGameType;
	public void Start(){

		_startSearching = false;
		_matchFound = false;
		SearchingText.gameObject.SetActive(false);
		HideCancelButton();

	}
	
	public void Update(){

	}

	public void Setup(){
		DisplayNameText.text = GameSparksLogic.Instance.UserDisplayName;
	}

	public void LogoutAction(){
		GameSparksLogic.Instance.LogoutUser();
		LobbyManager.Instance.HideGameLobbyManager();
		LobbyManager.Instance.ShowLoginManager();
	}

	public void MatchNotFound(){
		_startSearching = false;
		_matchFound = false;
		_searchingForGameType = GameType.None;
	}
	
	public void MatchFound(){
		_startSearching = false;
		_matchFound = true;
	}

	public void CancelAction(){
		GameSparksLogic.Instance.CancelFindMatch(_searchingForGameType);
	}

	public void BackAction(){
		GameLogic.Instance.LoadMainMenuScene();
	}

	public void Ranked1v1Action(){
		GameSparksLogic.Instance.FindMatch(GameType.Ranked1v1);
		_searchingForGameType = GameType.Ranked1v1;
		StartSearching();
	}

	public void Ranked2v2Action(){
		GameSparksLogic.Instance.FindMatch(GameType.Ranked2v2);
		_searchingForGameType = GameType.Ranked2v2;
		StartSearching();
	}

	public void Ranked4v4Action(){
		GameSparksLogic.Instance.FindMatch(GameType.Ranked4v4);
		_searchingForGameType = GameType.Ranked4v4;
		StartSearching();
	}

	public void Unranked1v1Action(){
		GameSparksLogic.Instance.FindMatch(GameType.Unranked1v1);
		_searchingForGameType = GameType.Unranked1v1;
		StartSearching();
	}

	public void Unranked2v2Action(){
		GameSparksLogic.Instance.FindMatch(GameType.Unranked2v2);
		_searchingForGameType = GameType.Unranked2v2;
		StartSearching();
	}

	public void Unranked4v4Action(){
		GameSparksLogic.Instance.FindMatch(GameType.Unranked4v4);
		_searchingForGameType = GameType.Unranked4v4;
		StartSearching();
	}

	public void StartSearching(){

		_startSearching = true;

		Back.gameObject.SetActive(false);
		Logout.gameObject.SetActive(false);
		SearchingText.gameObject.SetActive(true);

		ShowCancelButton();
		DisableMatchButtons();

		StartCoroutine(WriteSearchingText());
	}

	private IEnumerator WriteSearchingText(){
		
		string text = "...Searching...";
		int currentIndex = 0;
		while (_startSearching){

			SearchingText.text = text.Substring(0,currentIndex);
			currentIndex = (currentIndex + 1) % (text.Length + 1);
			yield return new WaitForSeconds(0.15f);
		}

		StopSearching();
	}

	public void StopSearching(){

		HideCancelButton();

		if (_matchFound){
			SearchingText.text = "Game found";
			SearchingText.alignment = TextAnchor.MiddleCenter;
		} else {
			Back.gameObject.SetActive(true);
			Logout.gameObject.SetActive(true);
			SearchingText.gameObject.SetActive(false);
			EnableMatchButtons();
		}
	}

	public void ShowCancelButton(){
		Cancel.gameObject.SetActive(true);
	}

	public void HideCancelButton(){
		Cancel.gameObject.SetActive(false);
	}

	public void DisableMatchButtons(){

		Ranked1v1Button.interactable = false;
		Ranked2v2Button.interactable = false;
		Ranked4v4Button.interactable = false;
		Unranked1v1Button.interactable = false;
		Unranked2v2Button.interactable = false;
		Unranked4v4Button.interactable = false;

	}

	public void EnableMatchButtons(){

		Ranked1v1Button.interactable = true;
		Ranked2v2Button.interactable = true;
		Ranked4v4Button.interactable = true;
		Unranked1v1Button.interactable = true;
		Unranked2v2Button.interactable = true;
		Unranked4v4Button.interactable = true;

	}

}
