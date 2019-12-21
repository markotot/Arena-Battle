using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallBullet : AbilityBullet {


	void OnTriggerEnter2D(Collider2D col){

		switch (col.tag){
		case "UndestructibleObstacle":
			Destroy(gameObject);
			break;
		case "DestructibleObstacle":
			Destroy(col.gameObject);
			Destroy (gameObject);
			break;
		case "Character": {

			if (Owner.Equals(BattleLogic.Instance.PlayerController)){ // bilo moj bullet udari karaktera

				CharController cc = col.GetComponent<CharController>();
				int targetId = GameSparksLogic.Instance.SessionInfo.GetRTPlayer(cc).PeerId;
				cc.TakeDamage(5);
				GameSparksLogic.Instance.SendBulletHitPlayerMessage(_id,targetId);
				BattleLogic.Instance.BulletContainer.DestroyAbilityBullet(_id,GetOwnerPeerId());
			}
			break;
		}
		default:
			break;
		}


	}
}
