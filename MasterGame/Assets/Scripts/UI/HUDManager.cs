using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDManager : MonoBehaviour {

	public TwitchEffectsManager TwitchEffectsManager;
	public PlayerHUDManager PlayerHUDManager;
	public InGameUI InGameUI;
	public VictoryScreenUI VictoryScreenUI;

	public void HideHUD(){
		gameObject.SetActive (false);
	}
	
	public void ShowHUD(){
		gameObject.SetActive (true);
	}

	public void HideAll(){
		HideTwitchEffectIcons();
		HideVictoryScreen();
		HidePlayerHUD();
	}

	public void SetupPlayerHUD(Ability aQ, Ability aE, Ability aSpace){

		PlayerHUDManager.SetupAbilityUI(aQ,aE,aSpace);
		ShowPlayerHUD();

	}

	public void ShowPlayerHUD(){
		PlayerHUDManager.gameObject.SetActive(true);
	}

	public void HidePlayerHUD(){
		PlayerHUDManager.gameObject.SetActive(false);
	}

	public void SetTwitchEffectIcons(List<CharacterEffects> effectList){
		
		for (int i = 0; i < effectList.Count; i++){
			TwitchEffectsManager.SetImage(i,effectList[i]);
			TwitchEffectsManager.ShowEffect(i);
		}
	}

	public void HideTwitchEffectIcons(){
		TwitchEffectsManager.HideAllEffects();
	}


	public void IncreaseNumber(int index){
		TwitchEffectsManager.IncreaseNumber(index);
	}

	public void ResetNumbers(){
		TwitchEffectsManager.ResetNumbers();
	}

	public void ResetNumber(int index){
		TwitchEffectsManager.ResetNumber(index);
	}

	public void SetAndShowWinnerEffect(Team winner, int voteCountRed,int voteCountBlue, int index){
		TwitchEffectsManager.SetWinnerEffect(winner, voteCountRed, voteCountBlue, index);
		TwitchEffectsManager.ShowWinnerEffect();
	}

	public void ResetAndHideWinnerEffect(){
		TwitchEffectsManager.ResetWinnerNumbers();
		TwitchEffectsManager.HideWinnerEffect();
	}

	public void HideWinnerEffect(){
		TwitchEffectsManager.HideWinnerEffect();
	}

	public void ShowInGameMenu(){
		InGameUI.gameObject.SetActive(true);
	}

	public void HideInGameMenu(){
		InGameUI.gameObject.SetActive(false);
	}

	public void ShowVictoryScreen(){
		VictoryScreenUI.gameObject.SetActive(true);
	}

	public void HideVictoryScreen(){
		VictoryScreenUI.gameObject.SetActive(false);
	}

	public void ToggleInGameMenu(){
		if (InGameUI.gameObject.activeSelf){
			HideInGameMenu();
		} else {
			ShowInGameMenu();
		}
	}

	public bool IsIngameUIActive(){
		return InGameUI.gameObject.activeSelf;
	}
}
