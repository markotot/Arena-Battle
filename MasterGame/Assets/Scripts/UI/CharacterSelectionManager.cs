using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionManager : MonoBehaviour {

	private static CharacterSelectionManager _instance;
	public static CharacterSelectionManager Instance{
		get { return _instance;}
	}

	public TeamSelectionManager TeamRed;
	public TeamSelectionManager TeamBlue;

	public Text CountdownText;
	// Use this for initialization
	void Start () {

		if (Instance == null){
			_instance = this;
		} else {
			Destroy(gameObject);
		}

		CountdownText.gameObject.SetActive(false);

		RTPlayer hostPlayer = GameSparksLogic.Instance.SessionInfo.HostPlayer;
		foreach(RTPlayer rtp in GameSparksLogic.Instance.SessionInfo.PlayerList){

			if (rtp.Team == Team.Red){
				TeamRed.SetupPlayer(rtp.Team,rtp.TeamPosition,rtp.DisplayName, (hostPlayer == rtp));
			} else if (rtp.Team == Team.Blue){
				TeamBlue.SetupPlayer(rtp.Team,rtp.TeamPosition,rtp.DisplayName, (hostPlayer == rtp));
			}
		}
	}

	public void SetPlayerReady(Team team, int position, bool ready){

		if (team == Team.Red){
			TeamRed.SetPlayerReady(position,ready);
		} else if (team == Team.Blue){
			TeamBlue.SetPlayerReady(position,ready);
		}

	}

	public void SetPlayerAbility(Team team, int position, AbilityType abilityType, int abilityIndex){
		if (team == Team.Red){
			TeamRed.SetPlayerAbility(position,abilityType,abilityIndex);
		} else if (team == Team.Blue){
			TeamBlue.SetPlayerAbility(position,abilityType,abilityIndex);
		}

	}

	public void AllPlayersReady(){
		StartCoroutine(StartCoundDown());
	}

	private IEnumerator StartCoundDown(){

		int startNumber = 3;
		CountdownText.gameObject.SetActive(true);
		while (startNumber > 0){
			CountdownText.text = startNumber.ToString();
			startNumber--;
			yield return new WaitForSeconds(1f);
		}

		CountdownText.gameObject.SetActive(false);
		GameLogic.Instance.LoadBattleGroundScene();
	}
}
