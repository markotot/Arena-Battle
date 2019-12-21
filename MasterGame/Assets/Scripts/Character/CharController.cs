using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterEffects { 
	None = 0,
	IncMovementSpeed = 1,
	DecAbilityECooldown = 2,
	DecAbilityQCooldown = 3,
	DecDashCooldown = 4,
	IncWeaponDamage = 5,
	IncAttackSpeed = 6,
	IncBulletSpeed = 7,
	IncHitPoints = 8,
	IncArmor = 9,
	IncScore = 10,
	IgnoreMovementPenatly = 11,
}

public enum Direction {
	Left = 0,
	Right = 1,
	Up = 2,
	Down = 3,
	Idle = 4
}

public enum CharacterState {
	Idle = 0,
	Walking = 1,
	Attacking = 2,
	Dashing = 3
}

public enum CharacterClass{
	BattleMage = 0,
	Scout = 1
}

public abstract class CharController : MonoBehaviour {

	public Rigidbody2D CharacterRigidbody;
	public SpriteRenderer CharacterSpriteRenderer;

	public Weapon weapon;
	public Ability AbilityQ;
	public Ability AbilityE;
	public Ability AbilitySpace;

	public HealthBar HealthBar;

	public CircleCollider2D MovementCollider;

	protected CharacterState _characterState;
	public CharacterState CharacterState {
		get { return _characterState; }
		set { _characterState = value; }
	}

	public float MovementSpeed = 5f;
	public bool IgnoreMovementPenalty = false;
	protected bool _touchingIce = false;
	public bool TouchingIce {
		get { return _touchingIce; }
	}

	public float MaxHP = 50f;
	protected float _currentHP;
	public float CurrentHP{
		get { return _currentHP;}
		set { _currentHP = value;}
	}

	protected bool _dead;
	public bool Dead{
		get { return _dead;}
		set { _dead = value;}
	}

	protected List<Tile> _onTiles = new List<Tile>();
	public List<Tile> OnTiles {
		get { return _onTiles;}
	}
	// Use this for initialization
	protected void Start () {
		_currentHP = MaxHP;
		CreateHealthBar();
	}
	
	// Update is called once per frame
	protected void Update () {
		if (BattleLogic.Instance.GameState == GameState.Playing){
			Move();
			Rotate();
			Attack();
		}
		
		HealthBar.SetPosition(CharacterRigidbody.position.x, CharacterRigidbody.position.y);
	}

	protected abstract void Move ();
	protected abstract void Attack ();
	protected abstract void Rotate ();

	public float CalculateCurrentMovementSpeed(){

		float slowDownSum = 0;
		foreach(Tile t in _onTiles){
			slowDownSum += t.SpeedReduction;
		}
		float avgSlowDown = slowDownSum / _onTiles.Count;
		return MovementSpeed * (1 - avgSlowDown);
	}

	public void AddTile(Tile t){
		_onTiles.Add(t);
		if (t.Ice){
			_touchingIce = true;
		}
	}

	public void RemoveTile(Tile t){
		_onTiles.Remove(t);

		foreach(Tile tile in _onTiles){
			if (tile.Ice){
				_touchingIce = true;
				return;
			}
		}
		_touchingIce = false;
	}

	public virtual void TakeDamage(float amount){
		_currentHP -= amount;
		HealthBar.UpdateHealth(MaxHP,_currentHP);
		if (_currentHP <= 0){
			_dead = true;
		}
	}

	public void HealDamage(float amount){
		_currentHP += amount;
		if (_currentHP > MaxHP){
			_currentHP = MaxHP;
		}
		HealthBar.UpdateHealth(MaxHP,_currentHP);
	}

	public void CreateHealthBar(){

		GameObject hpGO = (GameObject) Instantiate (GameLogic.Instance.HealthBar, CharacterRigidbody.position, Quaternion.Euler(0f,0f,0f));
		hpGO.transform.parent = BattleLogic.Instance.CharacterContainer.transform;
		HealthBar = hpGO.GetComponent<HealthBar>();
	}

	public void SetupAbilities(RTPlayer rtplayer){

		int aQind = rtplayer.AbilityQIndex;
		int aEind = rtplayer.AbilityEIndex;
		int aSpaceInd = rtplayer.AbilitySpaceIndex;
		
		switch(aQind){
		case 0: // fireball spell
			AbilityQ = gameObject.AddComponent<FireBall>();
			break;
		default:
			Debug.Log ("CharacterController: SetupAbilities, ability Q not valid");
			break; 
		}
		switch(aEind){
		case 0: // ice wall spell
			AbilityE = gameObject.AddComponent<IceWall>();
			break;
		default:
			Debug.Log ("CharacterController: SetupAbilities, ability E not valid");
			break; 
		}
		switch(aSpaceInd){
		case 0:
			AbilitySpace = gameObject.AddComponent<Dash>();
			break;
		case 1:
			AbilitySpace = gameObject.AddComponent<Heal>();
			break;
		default:
			Debug.Log ("CharacterController: SetupAbilities, ability SPACE not valid");
			break;
		}
	}

	#region PlayerStats
	
	public void IncreaseMovementSpeed(float speed){
		MovementSpeed += speed;
	}
	
	public void DecreaseAbilityQCooldown(float time){
		AbilityQ.AbilityCooldown -= time;
		if (AbilityQ.AbilityCooldown <= 0.25f){
			AbilityQ.AbilityCooldown = 0.25f;
		}
	}
	
	public void DecreaseAbilityECooldown(float time){
		AbilityE.AbilityCooldown -= time;
		if (AbilityE.AbilityCooldown <= 0.25f){
			AbilityE.AbilityCooldown = 0.25f;
		}
	}
	
	public void DecreaseAbilitySpaceCooldown(float time){
		AbilitySpace.AbilityCooldown -= time;
		if (AbilitySpace.AbilityCooldown <= 0.25f){
			AbilitySpace.AbilityCooldown = 0.25f;
		}
	}
	
	#endregion

	protected void OnDestroy(){
		Destroy(HealthBar.gameObject);
		StopAllCoroutines();
	}

	public void Die(){
		_dead = true;
		Destroy (transform.parent.gameObject); // destroy the player
	}

}
