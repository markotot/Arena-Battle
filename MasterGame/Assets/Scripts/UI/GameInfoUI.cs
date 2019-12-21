using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameInfoUI : MonoBehaviour {

	private float _fps = 0.0f;
	private float _deltaTime = 0.0f;

	public Text FPSText;
	public Text MemoryText;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
		_deltaTime += (Time.deltaTime - _deltaTime) * 0.1f;
		_fps = 1 / _deltaTime;

		SetFPS();
		SetMemoryUsage();
	}

	void SetFPS(){
		FPSText.text = Mathf.Round(_fps).ToString();
	}

	void SetMemoryUsage(){
	
		// memory is in bytes 1024 * 1024 converts it to megabyte
		MemoryText.text = (UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong() / (1024 * 1024)).ToString() + "mb";
	}
}
