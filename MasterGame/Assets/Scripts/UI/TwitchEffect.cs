using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TwitchEffect : MonoBehaviour {

	public Image Image;
	public Text Number;

	public void SetImage(Sprite sprite){
		Image.sprite = sprite;
	}

	public void ShowImage(){
		Image.gameObject.SetActive(true);
	}

	public void HideImage(){
		Image.gameObject.SetActive(false);
	}

	public void ShowNumber(){
		Image.gameObject.SetActive(true);
	}

	public void HideNumber(){
		Image.gameObject.SetActive(false);
	}

	public void ShowEffect(){
		gameObject.SetActive(true);
	}

	public void HideEffect(){
		gameObject.SetActive(false);
	}

	public void IncreaseNumber(){

		int result = 0;
		int.TryParse(Number.text,out result);
		result++;
		Number.text = result.ToString();
	}

	public void SetNumber(string text){
		Number.text = text;
	}

	public void ResetNumber(){
		Number.text = "0";
	}
}
