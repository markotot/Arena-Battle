using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PresceneLoading : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GameLogic.Instance.LoadMainMenuScene();
	}

}
