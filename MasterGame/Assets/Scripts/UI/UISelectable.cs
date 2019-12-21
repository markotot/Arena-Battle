using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISelectable : MonoBehaviour, IPointerEnterHandler, ISelectHandler {

	public ISelectableManager selectableManager;
	private Selectable sel;

	public void Start(){
		sel = gameObject.GetComponent<Selectable>();
	}
	public void OnPointerEnter(PointerEventData eventData){

	}

	public void OnSelect(BaseEventData eventData){

		selectableManager.SetFocus(sel);
		selectableManager.CurrentlySelected = sel;

	}
}
