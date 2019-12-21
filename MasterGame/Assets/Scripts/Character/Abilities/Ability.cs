using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityType{
	None = 0,
	AbilityQ = 1,
	AbilityE = 2,
	AbilitySpace = 3
}

/*
public enum AbilityQSpells{
	None = 0,
	FireBall = 1
}

public enum AbilityESpells{
	None = 0,
	IceWall = 1
}

public enum AbilitySpaceSpalls{
	None = 0,
	Dash = 1,
	Heal = 2
}
*/

public abstract class Ability : MonoBehaviour {

	protected bool _targetingActive = false;
	public bool isActive(){
		return _targetingActive;
	}

	public float AbilityCooldown = 3f;
	public float AbilityCooldownRemaining = 0f;
	private float _abilityUsedTime;
	protected bool _abilityReady;

	public bool AbilityReady{
		get {return _abilityReady;}
	}

	public float MaxRange;
	public bool AlwaysSmartCast;

	protected AbilityType _abilityType;
	public AbilityType AbilityType{
		get { return _abilityType;}
		set { _abilityType = value;}
	}

	public GameObject BulletGameObject;
	protected PlayerController _playerController;

	// Use this for initialization
	protected void Start () {

		_playerController = BattleLogic.Instance.PlayerController;
		_abilityUsedTime = 0f;
		_abilityReady = true;

		MaxRange = float.PositiveInfinity;
	}
	
	// Update is called once per frame
	protected void Update () {

		if (_targetingActive && _abilityReady){
			if ((GameLogic.Instance.SmartCast || AlwaysSmartCast || Input.GetMouseButtonDown(0)) && checkRange()){
				if (Use()){
					Deactivate();
					StartCoroutine(StartCooldown());
				}
			}
		}
	}

	protected abstract bool Use();
	public abstract void CreateProjectile(Vector2 startPosition, Vector2 targetPosition, int targetPeerId, int bulletId, int ownerId);

	public virtual void SendAbilityUsed(int bulletId, int ownerId,int targetPeerId = -1){
		GameSparksLogic.Instance.SendAbilityUsedMessage(BattleLogic.Instance.PlayerController.transform.position,BattleLogic.Instance.MouseWorldPosition,_abilityType,targetPeerId,bulletId);
	}

	public void Activate(){
		_targetingActive = true;
	}
	public void Deactivate(){
		_targetingActive = false;
	}

	protected IEnumerator StartCooldown(){
		_abilityUsedTime = Time.time;
		_abilityReady = false;

		while ( Time.time < _abilityUsedTime + AbilityCooldown){
			AbilityCooldownRemaining = AbilityCooldown - (Time.time - _abilityUsedTime);
			yield return new WaitForEndOfFrame();
		}

		AbilityCooldownRemaining = 0f;
		_abilityReady = true;
	}

	public float AbilityCooldownPercent(){
		return AbilityCooldownRemaining / AbilityCooldown;
	}

	protected bool checkRange(){
		return MaxRange >= BattleLogic.Instance.PlayerToMouseDistance;
	}
}
