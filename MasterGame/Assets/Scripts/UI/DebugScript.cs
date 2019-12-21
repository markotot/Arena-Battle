using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugScript : MonoBehaviour {

	public static DebugScript Instance;

	public Text Text1;
	public Text Text2;
	public Text Text3;
	public Text Text4;
	public Text Text5;

	void Start(){

		if (Instance == null){
			Instance = this;
		}

	}
	public void WriteToText1(string msg){
		Text1.text = msg;
	}

	public void WriteToText2(string msg){
		Text2.text = msg;
	}

	public void WriteToText3(string msg){
		Text3.text = msg;
	}

	public void WriteToText4(string msg){
		Text4.text = msg;
	}

	public void WriteToText5(string msg){
		Text5.text = msg;
	}
}
