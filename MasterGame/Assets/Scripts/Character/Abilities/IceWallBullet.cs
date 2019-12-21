using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceWallBullet : AbilityBullet {

	public float TimeAlive = 5f;
	// Use this for initialization
	protected new void Start () {
		base.Start();
		StartCoroutine(MeltWall());
	}
	
	// Update is called once per frame
	protected new void Update () {
		
	}

	private IEnumerator MeltWall(){

		yield return new WaitForSeconds(5f);
		Destroy(gameObject);

	}
}
