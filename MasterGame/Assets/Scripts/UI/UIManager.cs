using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour {

	private static UIManager _instance;
	public static UIManager Instance{
		get { return _instance;}
	}

	public HUDManager HUD;
	public GameObject MainMenu;
	public Image FadeImage;

	public Sprite[] AbilityQSprites;
	public Sprite[] AbilityESprites;
	public Sprite[] AbilitySpaceSprites;

	// Use this for initialization
	void Start () {

		if (Instance == null){
			_instance = this;
			DontDestroyOnLoad(gameObject);
		} else {
			Destroy(gameObject);
		}
	}

	public Sprite GetNextAbilityQ(int index){
		index = (index + 1) % AbilityQSprites.Length;
		return AbilityQSprites[index];		
	}

	public Sprite GetNextAbilityE(int index){
		index = (index + 1) % AbilityESprites.Length;
		return AbilityESprites[index];
	}

	public Sprite GetNextAbilitySpace(int index){
		index = (index + 1) % AbilitySpaceSprites.Length;
		return AbilitySpaceSprites[index];
	}

	public Sprite GetAbilitySprite(AbilityType at, int index){

		Sprite retVal = null;
		switch(at){
		case AbilityType.AbilityQ:
			retVal = AbilityQSprites[index];
			break;
		case AbilityType.AbilityE:
			retVal = AbilityESprites[index];
			break;
		case AbilityType.AbilitySpace:
			retVal = AbilitySpaceSprites[index];
			break;
		default:
			Debug.Log ("Ability type is invalid, UIManager: GetAbility()");
			break;
		}
		return retVal;

	}

	public IEnumerator FadeInLevel(float time){
		
		
		float fadeStarted = Time.time;
		while (Time.time < time + fadeStarted) {
			float alpha = Mathf.Lerp (1, 0, (Time.time - fadeStarted) / time);
			FadeImage.color = new Color (0f, 0f, 0f, alpha);
			yield return new WaitForEndOfFrame ();
		}

		BattleLogic.Instance.GameState = GameState.Playing;
		PlayerController playerController = BattleLogic.Instance.PlayerController;
		UIManager.Instance.SetupPlayerHUD(playerController.AbilityQ,playerController.AbilityE, playerController.AbilitySpace);
		UIManager.Instance.ShowHUD();

	}

	/*
	private IEnumerator fadeOut(float time){
		
		yield return new WaitForSeconds (1f);
		UIManager.Instance.HideHUD ();
		
		_fadeStarted = Time.time;
		while (Time.time < time + _fadeStarted) {
			float alpha = Mathf.Lerp (0, 1, (Time.time - _fadeStarted) / _fadeOutTime);
			fadeImage.color = new Color (0f, 0f, 0f, alpha);
			yield return new WaitForEndOfFrame ();
		}
		
		gameLogic.loadNextLevel ();
		yield return null;
		
	}
	*/

	public void ShowHUD(){
		HUD.ShowHUD();
	}

	public void HideHUD(){
		HUD.HideHUD();
	}

	public void ShowMainMenu(){
		MainMenu.SetActive(true);
	}

	public void HideMainMenu(){
		MainMenu.SetActive(false);
	}

	public void ShowFadeImage(){
		FadeImage.gameObject.SetActive(true);
	}

	public void HideFadeImage(){
		FadeImage.gameObject.SetActive(false);
	}

	public void SetFadeImageTransparency(float alpha){
		FadeImage.color = new Color(FadeImage.color.r,FadeImage.color.g,FadeImage.color.b,alpha);
	}

	public void SetupPlayerHUD(Ability aQ, Ability aE, Ability aSpace){
		HUD.SetupPlayerHUD(aQ,aE,aSpace);
	}

	public void SetTwitchEffectIcons(List<CharacterEffects> effectList){
		HUD.SetTwitchEffectIcons(effectList);
	}

	public void HideTwitchEffectIcons(){
		HUD.HideTwitchEffectIcons();
	}

	public void IncreaseNumber(int index){
		HUD.IncreaseNumber(index);
	}

	public void ResetNumbers(){
		HUD.ResetNumbers();
	}

	public void SetAndShowWinnerEffect(Team winner, int voteCountRed, int voteCountBlue, int index){
		HUD.SetAndShowWinnerEffect(winner, voteCountRed, voteCountBlue, index);
	}

	public void ResetAndHideWinnerEffect(){
		HUD.ResetAndHideWinnerEffect();
	}

	public void HideWinnerEffect(){
		HUD.HideWinnerEffect ();
	}
}
