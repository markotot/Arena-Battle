using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TwitchWinnerEffect : MonoBehaviour {

	public Image Image;
	public Text NumberRed;
	public Text NumberBlue;

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
	
	public void SetNumberRed(int count){
		NumberRed.text = count.ToString();
	}

	public void SetNumberBlue(int count){
		NumberBlue.text = count.ToString();
	}
	
	public void ResetNumbers(){
		NumberRed.text = "0";
		NumberBlue.text = "0";
	}
}
