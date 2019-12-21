using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : Ability {

	public float HealAmount = 10f;

	protected new void Start(){
		base.Start();

		AbilityCooldown = 10f;
		_abilityType = AbilityType.AbilitySpace;
		
	}

	protected override bool Use ()
	{
		return HealTarget();
	}
	
	private bool HealTarget(){

		bool retVal = false;
		Vector2 mousePosition = BattleLogic.Instance.MouseWorldPosition;
		float distance = BattleLogic.Instance.PlayerToMouseDistance;
		
		if (checkRange(distance)){ // check if target in range

			RaycastHit2D[] hits = Physics2D.RaycastAll(mousePosition, Vector2.zero);
			CharController charController = null;
			foreach(RaycastHit2D hit in hits){

				if (hit.collider.tag == "Character"){ // find if mouse hit a target
					if ((charController = hit.collider.gameObject.GetComponent<CharController>()) != null){

						charController.HealDamage(HealAmount);
						if (GameLogic.Instance.IsMultiplayer){
							int peerId = GameSparksLogic.Instance.SessionInfo.GetRTPlayer(charController).PeerId;
							SendAbilityUsed(-1,-1,peerId);
						}
						retVal = true;
						break;
					}
				}
			}
		}
		
		return retVal;
	}
	
	// Used in GameSparksLogic class to create projectiles for ally players
	public override void CreateProjectile(Vector2 startPosition, Vector2 targetPosition, int targetPeerId, int bulletId, int ownerId){

		Debug.Log ("Heal recieved");
		RTPlayer rtp = GameSparksLogic.Instance.SessionInfo.GetRTPlayer(targetPeerId);
		rtp.CharacterController.HealDamage(5);

	}
	
	private bool checkRange(float distance){
		return MaxRange > distance;
	}
}
