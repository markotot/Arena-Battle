using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSparks.Api.Messages;

public enum GameType {
	None = 0,
	SinglePlayer = 1,
	Unranked1v1 = 2,
	Unranked2v2 = 3,
	Unranked4v4 = 4,
	Ranked1v1 = 5,
	Ranked2v2 = 6,
	Ranked4v4 = 7
}

public class RTSessionInfo
{
	private string _hostURL;
	public string HostURL{
		get { return _hostURL;}
	}
	
	private string _acccessToken;
	public string AccessToken{
		get { return _acccessToken;}
	}
	
	private int _portID;
	public int PortID{
		get { return _portID;}
	}
	
	private string _matchID;
	public string MatchID{
		get { return _matchID;}
	}

	private GameType _gameType;
	public GameType GameType{
		get { return _gameType;}
		set { _gameType = value;}
	}

	private RTPlayer _hostPlayer;
	public RTPlayer HostPlayer{
		get { return _hostPlayer;}
	}
	
	private List<RTPlayer> _playerList = new List<RTPlayer> ();
	public List<RTPlayer> PlayerList{
		get {return _playerList;}
	}
	
	/// <summary>
	/// Creates a new RTSession object which is held until a new RT session is created
	/// </summary>
	/// <param name="_message">Message.</param>
	public RTSessionInfo (MatchFoundMessage message, GameType gameType){
		_portID = (int)message.Port;
		_hostURL = message.Host;
		_acccessToken = message.AccessToken;
		_matchID = message.MatchId;

		int numberOfPlayers = 0;
		foreach(MatchFoundMessage._Participant p in message.Participants){
			numberOfPlayers++;
		}
		int numberOfPlayersPerTeam = numberOfPlayers / 2;
		int currentPlayer = 1;
		// we loop through each participant and get their peerId and display name //
		foreach(MatchFoundMessage._Participant p in message.Participants){

			Team t = (currentPlayer <= numberOfPlayersPerTeam) ? Team.Red : Team.Blue;
			int teamPosition = (currentPlayer <= numberOfPlayersPerTeam) ? currentPlayer : (currentPlayer - numberOfPlayersPerTeam);
			_playerList.Add(new RTPlayer(p.DisplayName, p.Id, (int)p.PeerId, t, teamPosition));
			currentPlayer++;
		}

		_gameType = gameType;
	}

	public int PeerId(Team team, int position){

		int retVal = -1;
		foreach(RTPlayer rtp in PlayerList){
			if (rtp.Team == team && rtp.TeamPosition == position){
				retVal = rtp.PeerId;
			}
		}
		return retVal;
	}

	public RTPlayer GetRTPlayer(int peerId){

		RTPlayer retVal = null;
		foreach(RTPlayer rtp in PlayerList){
			if(rtp.PeerId == peerId){
				retVal = rtp; 
				break;
			}
		}
		return retVal;
	}

	public RTPlayer GetRTPlayer(CharController charController){
		RTPlayer retVal = null;
		foreach(RTPlayer rtp in PlayerList){
			if(rtp.CharacterController.Equals(charController)){
				retVal = rtp; 
				break;
			}
		}
		return retVal;
	}

	public RTPlayer GetRTPlayer(Team team, int position){
		RTPlayer retVal = null;
		foreach(RTPlayer rtp in PlayerList){
			if (rtp.Team == team && rtp.TeamPosition == position){
				retVal = rtp;
				break;
			}
		}
		return retVal;
	}

	public void SetHostPlayer(string id){

		foreach(RTPlayer rtp in PlayerList){
			if(rtp.Id.Equals(id)){
				_hostPlayer = rtp; 
				break;
			}
		}
	}
}
