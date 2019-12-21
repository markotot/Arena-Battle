using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team {
	None = 0,
	Red = 1,
	Blue = 2,
}

public class RTPlayer
{

	private string _displayName;
	public string DisplayName {
		get { return _displayName;}
		set { _displayName = value;}
	}

	private string _id;
	public string Id {
		get { return _id;}
		set { _id = value;}
	}

	private int _peerId;
	public int PeerId {
		get { return _peerId;}
		set { _peerId = value;}
	}

	private CharController _characterController;
	public CharController CharacterController{
		get { return _characterController;}
		set { _characterController = value;}
	}

	private Team _team;
	public Team Team {
		get { return _team;}
		set { _team = value;}
	}

	private int _teamPosition;
	public int TeamPosition{
		get { return _teamPosition;}
		set { _teamPosition = value;}
	}

	public int AbilityQIndex;
	public int AbilityEIndex;
	public int AbilitySpaceIndex;

	public RTPlayer(string displayName, string id, int peerId,Team team, int teamPosition){
		_displayName = displayName;
		_id = id;
		_peerId = peerId;
		_team = team;
		_teamPosition = teamPosition;
	}

}