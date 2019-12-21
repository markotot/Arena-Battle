using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : Ability {

	float dashSpeedIncrease = 5;

	protected new void Start(){
		base.Start();

		AlwaysSmartCast = true;
		AbilityCooldown = 5f;

		_abilityType = AbilityType.AbilitySpace;

	}
	protected override bool Use ()
	{
		StartCoroutine(ActivateDash());
		Deactivate();
		StartCoroutine(StartCooldown());

		return true;
	}

	public override void CreateProjectile(Vector2 startPosition, Vector2 targetPosition, int targetPeerId, int bulletId, int ownerId){

	}

	private IEnumerator ActivateDash(){
		
		float oldMovementSpeed = _playerController.MovementSpeed;
		_playerController.MovementSpeed += dashSpeedIncrease;
		_playerController.IgnoreMovementPenalty = true;

		yield return new WaitForSeconds(0.15f);

		_playerController.MovementSpeed = oldMovementSpeed;
		_playerController.IgnoreMovementPenalty = false;
	}
}
