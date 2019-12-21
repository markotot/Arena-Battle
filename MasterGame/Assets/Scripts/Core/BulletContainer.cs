using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletContainer : MonoBehaviour {

	private List<AbilityBullet> abilityBullets = new List<AbilityBullet>();
	private int _nextAbilityBulletId = 0;
	public int NextAbilityBulletId {
		get { return _nextAbilityBulletId++;}
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void AddNewAbilityBullet(AbilityBullet ab){
		abilityBullets.Add(ab);
	}

	public void RemoveAbilityBullet(AbilityBullet ab){
		abilityBullets.Remove(ab);
	}

	public void RemoveAbilityBullet(int bulletId, int ownerId){
		AbilityBullet ab = FindAbilityBullet(bulletId,ownerId);
		RemoveAbilityBullet(ab);
	}

	public AbilityBullet FindAbilityBullet(int bulletId, int ownerId){

		AbilityBullet retVal = null;
		foreach(AbilityBullet ab in abilityBullets){
			if (ab.Id == bulletId && ab.GetOwnerPeerId() == ownerId){
				retVal = ab;
				break;
			}
		}

		return retVal;
	}

	public void DestroyAbilityBullet(int bulletId, int ownerId){

		AbilityBullet ab = FindAbilityBullet(bulletId,ownerId);
		RemoveAbilityBullet(ab);
		Destroy(ab.gameObject);

	}

}
