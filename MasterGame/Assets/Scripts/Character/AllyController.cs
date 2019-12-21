using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyController : CharController {

	protected Vector2 _previousPos;
	protected float _goToRot;

	new public void Start(){

		base.Start();
		_goToRot = transform.rotation.eulerAngles.z;
	}

	protected override void Move (){
		CharacterRigidbody.transform.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(transform.eulerAngles.z, _goToRot, Time.deltaTime / GameSparksLogic.Instance.UpdateRate));
	}
	
	protected override void Attack(){
		
	}

	protected override void Rotate(){

	}

	public void MovementUpdate(Vector2 position, Vector2 velocity, float rotationZ){

		// Ako je razlika stvarne pozicije i ocekivane velika, pomeri se ka ocekivanoj 
		if (Vector2.Distance(CharacterRigidbody.position,_previousPos) > 0.1f){
			CharacterRigidbody.position = Vector2.MoveTowards(CharacterRigidbody.position,_previousPos,0.05f);
		}

		CharacterRigidbody.velocity = velocity;

		_previousPos = position;
		_goToRot = rotationZ;
		

	}


}
