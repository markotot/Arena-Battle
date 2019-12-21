using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour {

	public GameObject backgroundHpObject;
	public GameObject currentHpObject;

	void Update(){

	}

	public void UpdateHealth(float maxHp, float currentHp){

		if (currentHp < 0) {
			currentHp = 0;
		}
		currentHpObject.transform.localScale = new Vector3(currentHp / maxHp, 1, 1);

	}

	public void ResetHealth(){
		currentHpObject.transform.localScale = new Vector3 (1, 1, 1);
	}

	public void SetPosition(float x, float y){
		transform.position = new Vector2(x,y);
	}
}
