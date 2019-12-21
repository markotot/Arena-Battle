using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHUDManager : MonoBehaviour {

	public AbilityUI abilityQUI;
	public AbilityUI abilityEUI;
	public AbilityUI abilitySpaceUI;

	public void SetupAbilityUI(Ability aQ, Ability aE, Ability aSpace){

		abilityQUI.SetAbility(aQ);
		abilityEUI.SetAbility(aE);
		abilitySpaceUI.SetAbility(aSpace);
	}
}
