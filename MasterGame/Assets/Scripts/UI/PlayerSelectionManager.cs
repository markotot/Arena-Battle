using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelectionManager : MonoBehaviour {

	private Team _team;
	public Team Team{
		get { return _team;}
		set { _team = value;}
	}

	private int _teamPosition;
	public int TeamPosition {
		get { return _teamPosition;}
		set { _teamPosition = value;}
	}

	private RTPlayer _rtPlayer;
	public RTPlayer RTPlayer{
		get { return _rtPlayer;}
		set { _rtPlayer = value;}
	}

	public Text PlayerDisplayName;

	public Button AbilityQ;
	public Button AbilityE;
	public Button AbilitySpace;

	private int _abilityQIndex = 0;
	private int _abilityEIndex = 0;
	private int _abilitySpaceIndex = 0;

	public Button ReadyButton;
	private bool _isReady = false;

	public void ReadyAction(){

		float colorMax = 255;
		if (_isReady){

			_isReady = false;

			ColorBlock colorBlock = ReadyButton.colors;
			colorBlock.normalColor = new Color(251 / colorMax, 255 / colorMax, 216 / colorMax);
			colorBlock.highlightedColor = new Color(237 / colorMax, 240 / colorMax, 205 / colorMax);
			ReadyButton.colors = colorBlock;

			PlayerNotReady();

		} else {

			_isReady = true;

			ColorBlock colorBlock = ReadyButton.colors;
			colorBlock.normalColor = new Color (103 / colorMax, 249 / colorMax, 158 / colorMax);
			colorBlock.highlightedColor = new Color(91 / colorMax, 233 / colorMax, 144 / colorMax);
			ReadyButton.colors = colorBlock;

			PlayerReady ();

		}
	}

	public void SetPlayerReady(bool ready){
	
		_isReady = ready;
		float colorMax = 255;
		ColorBlock colorBlock = ReadyButton.colors;
		if (ready){
			_isReady = true;
			colorBlock.disabledColor = new Color (103 / colorMax, 249 / colorMax, 158 / colorMax);
		} else {
			_isReady = false;
			colorBlock.disabledColor = new Color (200 / colorMax, 200 / colorMax, 200 / colorMax, 128 / colorMax);
		}

		ReadyButton.colors = colorBlock;
	}

	private void PlayerReady(){ 

		DisableAbilityButtons();
		GameSparksLogic.Instance.SendPlayerSelectionReady(Team,_teamPosition);
	}

	private void PlayerNotReady(){

		EnableAbilityButtons();
		GameSparksLogic.Instance.SendPlayerSelectionNotReady(Team,_teamPosition);
	}

	public void EnableAbilityButtons(){
		AbilityE.interactable = true;
		AbilityQ.interactable = true;
		AbilitySpace.interactable = true;
	}

	public void DisableAbilityButtons(){
		AbilityE.interactable = false;
		AbilityQ.interactable = false;
		AbilitySpace.interactable = false;
	}

	public void EnableReadyButton(){
		ReadyButton.interactable = true;
	}

	public void DisableReadyButton(){
		ReadyButton.interactable = false;
	}

	public void HideAbilityButtons(){
		AbilityQ.gameObject.SetActive(false);
		AbilityE.gameObject.SetActive(false);
		AbilitySpace.gameObject.SetActive(false);
	}

	public void ShowAbilityButtons(){
		AbilityQ.gameObject.SetActive(true);
		AbilityE.gameObject.SetActive(true);
		AbilitySpace.gameObject.SetActive(true);
	}

	public void SetAbility(AbilityType abilityType, int abilityIndex){ // method called from GameSparksLogic when other players change their ability

		Sprite abilityImage = UIManager.Instance.GetAbilitySprite(abilityType,abilityIndex);

		switch(abilityType){
		case AbilityType.AbilityQ:
			_rtPlayer.AbilityQIndex = abilityIndex;
			AbilityQ.image.sprite = abilityImage;
			break;
		case AbilityType.AbilityE:
			_rtPlayer.AbilityEIndex = abilityIndex;
			AbilityE.image.sprite = abilityImage;
			break;
		case AbilityType.AbilitySpace:
			_rtPlayer.AbilitySpaceIndex = abilityIndex;
			AbilitySpace.image.sprite = abilityImage;
			break;
		default:
			break;
		}
	}

	public void AbilityQAction(){
		
		Sprite abilityImage = UIManager.Instance.GetNextAbilityQ(_abilityQIndex);
		AbilityQ.image.sprite = abilityImage;
		_abilityQIndex = (_abilityQIndex + 1) % UIManager.Instance.AbilityQSprites.Length;
		_rtPlayer.AbilityQIndex = _abilityQIndex;
		AbilityAction(AbilityType.AbilityQ,_abilityQIndex + 1);

	}

	public void AbilityEAction(){

		Sprite abilityImage = UIManager.Instance.GetNextAbilityE(_abilityEIndex);
		AbilityE.image.sprite = abilityImage;
		_abilityEIndex = (_abilityEIndex + 1) % UIManager.Instance.AbilityESprites.Length;
		_rtPlayer.AbilityEIndex = _abilityEIndex;
		AbilityAction(AbilityType.AbilityE,_abilityEIndex);
	}

	public void AbilitySpaceAction(){

		Sprite abilityImage = UIManager.Instance.GetNextAbilitySpace(_abilitySpaceIndex);
		AbilitySpace.image.sprite = abilityImage;
		_abilitySpaceIndex = (_abilitySpaceIndex + 1) % UIManager.Instance.AbilitySpaceSprites.Length;
		_rtPlayer.AbilitySpaceIndex = _abilitySpaceIndex;
		AbilityAction(AbilityType.AbilitySpace, _abilitySpaceIndex);
	}	

	public void AbilityAction(AbilityType abilityType, int abilityIndex){
		GameSparksLogic.Instance.SendPlayerAbilitySelected(_team, _teamPosition, abilityType, abilityIndex);
	}

}
