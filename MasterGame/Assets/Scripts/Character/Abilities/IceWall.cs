using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceWall : Ability {

	public float MaxCastDistance = 10f;

	protected new void Start(){
		base.Start();

		AbilityCooldown = 3f;
		_abilityType = AbilityType.AbilityE;
		BulletGameObject = GameLogic.Instance.IceWallBullet;
	}

	protected override bool Use () {

		CreateIceWall();
		return true;
	}

	private void CreateIceWall(){
		
		Vector2 targetingVector = BattleLogic.Instance.playerToMouseVector();
		float angle = BattleLogic.Instance.playerToMouseAngle();

		Vector2 wallPosition;
		Quaternion wallAngle = Quaternion.Euler(0,0,90 + angle);

		if (checkCastDistance()){
			wallPosition = BattleLogic.Instance.MouseWorldPosition;
		} else {
			wallPosition = BattleLogic.Instance.positionRelativeToPlayer(targetingVector * MaxCastDistance);
		}

		GameObject bulletGO = (GameObject) Instantiate (BulletGameObject, wallPosition, wallAngle);
		bulletGO.transform.parent = BattleLogic.Instance.BulletContainer.transform;

		AbilityBullet ab = bulletGO.GetComponent<AbilityBullet>();
		ab.Id = BattleLogic.Instance.BulletContainer.NextAbilityBulletId;
		ab.Owner = _playerController;

		SendAbilityUsed(ab.Id,ab.GetOwnerPeerId());
	}

	// Used in GameSparksLogic class to create projectiles for ally players
	public override void CreateProjectile(Vector2 startPosition, Vector2 targetPosition, int targetPeerId, int bulletId, int ownerId){

		Vector2 targetingVector = (targetPosition - startPosition).normalized;
		float angle = Vector2.SignedAngle(Vector2.right, targetingVector);
		Quaternion wallAngle = Quaternion.Euler(0,0,90 + angle);
		
		GameObject bulletGO = (GameObject) Instantiate (BulletGameObject, targetPosition, wallAngle);
		bulletGO.transform.parent = BattleLogic.Instance.BulletContainer.transform;

		AbilityBullet ab = bulletGO.GetComponent<AbilityBullet>();
		ab.Id = BattleLogic.Instance.BulletContainer.NextAbilityBulletId;
		ab.Owner = GameSparksLogic.Instance.SessionInfo.GetRTPlayer(ownerId).CharacterController;

	}

	private bool checkCastDistance(){
		return MaxCastDistance >= BattleLogic.Instance.PlayerToMouseDistance;

	}


}
