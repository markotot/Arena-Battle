using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : Ability {

	public float BulletSpeed = 10f;

	protected new void Start(){
		base.Start();
		
		AbilityCooldown = 2f;
		_abilityType = AbilityType.AbilityQ;
		BulletGameObject = GameLogic.Instance.FireBallBullet;
	}

	protected override bool Use () {
		createFireBall(); 
		return true;
	}

	private void createFireBall(){
		
		Vector2 angle = BattleLogic.Instance.playerToMouseVector();
		GameObject bulletGO = (GameObject) Instantiate (BulletGameObject, transform.position, BattleLogic.Instance.addAngleToRotation(90,transform.rotation));

		bulletGO.GetComponent<Rigidbody2D>().velocity = new Vector2(BulletSpeed * angle.x, BulletSpeed * angle.y);
		bulletGO.transform.parent = BattleLogic.Instance.BulletContainer.transform;

		AbilityBullet ab = bulletGO.GetComponent<AbilityBullet>();
		ab.Id = BattleLogic.Instance.BulletContainer.NextAbilityBulletId;
		ab.Owner = _playerController;

		SendAbilityUsed(ab.Id,ab.GetOwnerPeerId()); // send message to gamesparks

	}

	// Used in GameSparksLogic class to create projectiles for ally players
	public override void CreateProjectile(Vector2 startPosition, Vector2 targetPosition, int targetPeerId, int bulletId, int ownerId){

		Vector2 movementVector = (targetPosition - startPosition).normalized;

		GameObject bulletGO = (GameObject) Instantiate (BulletGameObject, startPosition, BattleLogic.Instance.addAngleToRotation(90,transform.rotation));
		bulletGO.GetComponent<Rigidbody2D>().velocity = new Vector2(BulletSpeed * movementVector.x, BulletSpeed * movementVector.y);
		bulletGO.transform.parent = BattleLogic.Instance.BulletContainer.transform;

		AbilityBullet ab = bulletGO.GetComponent<AbilityBullet>();
		ab.Id = BattleLogic.Instance.BulletContainer.NextAbilityBulletId;
		ab.Owner = GameSparksLogic.Instance.SessionInfo.GetRTPlayer(ownerId).CharacterController;

	}

}
