using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSparks.Api.Responses;

public class RegisterManager : ISelectableManager {

	public InputField UsernameInput;
	public InputField PasswordInput;
	public InputField DisplayNameInput;
	public Button SubmitButton;
	public Button CancelButton;
	
	public void Start(){
		
		_selectableList.Add(UsernameInput);
		_selectableList.Add(PasswordInput);
		_selectableList.Add (DisplayNameInput);
		_selectableList.Add (SubmitButton);
		_selectableList.Add (CancelButton);
		
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
			CancelAction();
		}

		if (Input.GetKeyDown(KeyCode.Return) && _currentlySelected != CancelButton){
			SubmitAction();
		}
		
	}

	public string getUsernameText(){
		return UsernameInput.text;
	}
	
	public string getPasswordText(){
		return PasswordInput.text;
	}

	public string getDisplayNameText(){
		return DisplayNameInput.text;
	}
	
	public void SubmitAction(){
		GameSparksLogic.Instance.RegisterPlayer(getDisplayNameText(),getUsernameText(),getPasswordText(),OnRegister);
	}

	private void OnRegister(RegistrationResponse response){
		ClearText();
		LobbyManager.Instance.HideRegisterManager();
		LobbyManager.Instance.ShowLoginManager();
	}
	
	public void CancelAction(){

		ClearText();
		LobbyManager.Instance.HideRegisterManager();
		LobbyManager.Instance.ShowLoginManager();

	}

	public void ClearText(){
		ClearUsernameText();
		ClearPasswordText();
		ClearDisplayNameText();
	}

	public void ClearUsernameText(){
		UsernameInput.text = "";
	}

	public void ClearPasswordText(){
		PasswordInput.text = "";
	}

	public void ClearDisplayNameText(){
		DisplayNameInput.text = "";
	}

}
