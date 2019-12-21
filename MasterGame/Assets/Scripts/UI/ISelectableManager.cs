using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ISelectableManager : MonoBehaviour {

	protected List<Selectable> _selectableList = new List<Selectable>();

	protected Selectable _currentlySelected;
	public Selectable CurrentlySelected{
		get { return _currentlySelected;}
		set { _currentlySelected = value;}
	}

	protected int _currentlySelectedIndex = -1;

	public int CurrentlySelectedIndex{
		get { return _currentlySelectedIndex;}
	}

	public void SetFocus(Selectable selectable){
		_currentlySelectedIndex = _selectableList.IndexOf(selectable);
		_selectableList[_currentlySelectedIndex].Select();
	}

	public void FocusNext(){

		_currentlySelectedIndex = (_currentlySelectedIndex + 1) % _selectableList.Count;
		_selectableList[_currentlySelectedIndex].Select();
	}

	public void FocusPrevious(){

		_currentlySelectedIndex = _currentlySelectedIndex - 1;
		if (_currentlySelectedIndex < 0){
			_currentlySelectedIndex = _selectableList.Count - 1;
		}
		_selectableList[_currentlySelectedIndex].Select();
	}

	public void FocusFirst(){

		_currentlySelectedIndex = 0;
		_selectableList[_currentlySelectedIndex].Select();

	}


	public void SetCurrentlySelectedIndex(Selectable selectable){
		_currentlySelectedIndex = _selectableList.IndexOf(selectable);
	}

}
