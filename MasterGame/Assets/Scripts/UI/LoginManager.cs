using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSparks.Api.Responses;
using UnityEngine.SceneManagement;

public class LoginManager : ISelectableManager {

	public InputField UsernameInput;
	public InputField PasswordInput;

	public Button LoginButton;
	public Button RegisterButton;
	public Button BackButton;

	public void Start(){

		_selectableList.Add(UsernameInput);
		_selectableList.Add(PasswordInput);
		_selectableList.Add(LoginButton);
		_selectableList.Add(RegisterButton);
		_selectableList.Add(BackButton);
		
		FocusNext();
	}

	public void Update(){

		if (Input.GetKeyDown(KeyCode.Tab)){
			if (Input.GetKey(KeyCode.LeftShift)){
				FocusPrevious();
			} else {
				FocusNext();
			}
		}

		if (Input.GetKeyDown(KeyCode.Escape)){
			BackAction();
		}

		if (Input.GetKeyDown(KeyCode.Return) && _currentlySelected != BackButton){
			LoginAction();
		}

	}

	public string getUsernameText(){
		return UsernameInput.text;
	}

	public string getPasswordText(){
		return PasswordInput.text;
	}

	public void LoginAction(){
		GameSparksLogic.Instance.AuthenticatePlayerByUsernameAndPass(getUsernameText(),getPasswordText(),OnAuthentication);
	}

	private void OnAuthentication(AuthenticationResponse response){
		ClearText();
		LobbyManager.Instance.HideLoginManager();
		LobbyManager.Instance.ShowGameLobbyManager();
	}

	public void RegisterAction(){
		LobbyManager.Instance.HideLoginManager();
		LobbyManager.Instance.ShowRegisterManager();
	}

	public void BackAction(){
		GameLogic.Instance.LoadMainMenuScene();
	}

	public void ClearText(){
		ClearUsernameText();
		ClearPasswordText();
	}
	
	public void ClearUsernameText(){
		UsernameInput.text = "";
	}
	
	public void ClearPasswordText(){
		PasswordInput.text = "";
	}


}
