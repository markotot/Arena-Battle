using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityBullet : MonoBehaviour {

	protected int _id;
	public int Id {
		get { return _id;}
		set { _id = value;}
	}
	protected CharController _owner;
	public CharController Owner{
		get {return _owner;}
		set { _owner = value;}
	}

	// Use this for initialization
	protected void Start () {
		BattleLogic.Instance.BulletContainer.AddNewAbilityBullet(this);
	}

	protected void OnDestroy(){
		BattleLogic.Instance.BulletContainer.RemoveAbilityBullet(this);
	}
	
	// Update is called once per frame
	protected void Update () {
		
	}

	public int GetOwnerPeerId(){
		RTPlayer rtp = GameSparksLogic.Instance.SessionInfo.GetRTPlayer(_owner);
		return rtp.PeerId;
	}

}
