using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct EffectImage {
	public CharacterEffects effect;
	public Sprite image;
}

public class TwitchEffectsManager : MonoBehaviour {


	public EffectImage[] EffectImages;
	public TwitchEffect[] effects = new TwitchEffect[5];

	public TwitchWinnerEffect winnerEffect;

	private Dictionary<CharacterEffects,Sprite> imageDict = new Dictionary<CharacterEffects, Sprite>();

	void Start () {

		for (int i = 0; i < EffectImages.Length; i++){
			imageDict.Add(EffectImages[i].effect,EffectImages[i].image);
		}
	}

	public void SetImage(int index, CharacterEffects effect){
		effects[index].SetImage(imageDict[effect]);
	}

	public void ShowEffect(int index){
		effects[index].ShowEffect();
	}

	public void HideEffect(int index){
		effects[index].HideEffect();
	}

	public void HideAllEffects(){
		for (int i = 0; i < effects.Length; i++){
			HideEffect(i);
		}
	}

	public void IncreaseNumber(int index){
		effects[index].IncreaseNumber();
	}

	public void ResetNumber(int index){
		effects[index].ResetNumber();
	}

	public void ResetNumbers(){
		for (int i = 0; i < effects.Length; i++){
			ResetNumber(i);
		}
	}

	public void SetWinnerEffect(Team winner, int voteCountRed, int voteCountBlue, int index){

		winnerEffect.SetNumberRed(voteCountRed);
		winnerEffect.SetNumberBlue(voteCountBlue);
		winnerEffect.SetImage(effects[index].Image.sprite);

	}

	public void ResetWinnerNumbers(){
		winnerEffect.ResetNumbers();
	}

	public void ShowWinnerEffect(){
		winnerEffect.ShowEffect();
	}

	public void HideWinnerEffect(){
		winnerEffect.HideEffect();
	}

}
