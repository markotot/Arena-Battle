using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUI : MonoBehaviour {

	// Use this for initialization
	public Image _abilityImage;
	public Image CooldownOverlay;
	public Text CooldownText;
	private Ability _ability;

	public void SetAbility(Ability ability){

		_ability = ability;
		RTPlayer rtp = GameSparksLogic.Instance.SessionInfo.HostPlayer;
		Sprite abilitySprite = null;
		switch(_ability.AbilityType){
		case AbilityType.AbilityQ:{
			abilitySprite = UIManager.Instance.GetAbilitySprite(AbilityType.AbilityQ, rtp.AbilityQIndex);
			break;
		}
		case AbilityType.AbilityE:{
			abilitySprite = UIManager.Instance.GetAbilitySprite(AbilityType.AbilityE, rtp.AbilityEIndex);
			break;
		}
		case AbilityType.AbilitySpace:{
			abilitySprite = UIManager.Instance.GetAbilitySprite(AbilityType.AbilitySpace, rtp.AbilitySpaceIndex);
			break;
		}
		default:
			Debug.Log ("AbilityType invalid");
			break;
		}
		_abilityImage.sprite = abilitySprite;

	}
	// Update is called once per frame
	void Update () {
		if (_ability != null){
			if (_ability.AbilityReady){
				CooldownOverlay.fillAmount = 0;
				CooldownText.text = "";
			} else {
				CooldownOverlay.fillAmount = _ability.AbilityCooldownPercent();
				CooldownText.text = Mathf.CeilToInt(_ability.AbilityCooldownRemaining).ToString();
			}
		}
	}
}
