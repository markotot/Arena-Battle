
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : CharController {


	private Vector3 _movement = Vector3.zero;

	new public void Start(){
		base.Start();
		if (GameLogic.Instance.IsMultiplayer) {
			StartCoroutine (SendMovement ());
		}
	}

	new public void Update(){
		base.Update();
	}

	protected override void Move(){
		
		float currentMovementSpeed = IgnoreMovementPenalty ? MovementSpeed : CalculateCurrentMovementSpeed();

		if (CharacterState != CharacterState.Dashing){
			_movement = new Vector3(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"), 0f);
			_movement = _movement.normalized * currentMovementSpeed;
		}

		if (_touchingIce){
			CharacterRigidbody.AddForce(_movement);
		} else {

			if (!float.IsNaN(_movement.x) && !float.IsNaN(_movement.y)){
				CharacterRigidbody.velocity = _movement;
			}
		}

		if (Input.GetKeyDown(KeyCode.Space) && AbilitySpace.AbilityReady){
			AbilitySpace.Activate();
		}
		
	}

	protected override void Attack(){

		if (Input.GetKeyDown(KeyCode.Q) && AbilityQ.AbilityReady){
			AbilityQ.Activate();
			AbilityE.Deactivate();
		}

		if (Input.GetKeyDown(KeyCode.E) && AbilityE.AbilityReady){
			AbilityE.Activate();
			AbilityQ.Deactivate();
		}

		if (Input.GetMouseButtonDown(0)){
			if (!AbilityE.isActive() && !AbilityQ.isActive()){
				// attack with weapon
			}
		}

		if (Input.GetMouseButtonDown(1)){

			if (AbilityE.isActive()){
				AbilityE.Deactivate();
			}
			if (AbilityQ.isActive()){
				AbilityQ.Deactivate();
			}
		}
		
	}

	protected override void Rotate(){
		CharacterRigidbody.transform.eulerAngles = new Vector3(0f,0f, BattleLogic.Instance.playerToMouseAngle());
	}

	public override void TakeDamage(float amount){
		_currentHP -= amount;
		HealthBar.UpdateHealth(MaxHP,_currentHP);
		if (_currentHP <= 0){
			GameSparksLogic.Instance.SendPlayerDeadMessage();
			Die();
		}
	}

	#region GameSparks Methods

	private IEnumerator SendMovement(){
		Vector4 movement = new Vector4(transform.position.x,transform.position.y,CharacterRigidbody.velocity.x,CharacterRigidbody.velocity.y);
		float rotationZ = transform.eulerAngles.z;
		GameSparksLogic.Instance.SendPlayerMovement(movement,rotationZ);
		yield return new WaitForSeconds (GameSparksLogic.Instance.UpdateRate);
		StartCoroutine (SendMovement ());
	}


	#endregion




}
