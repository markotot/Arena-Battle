using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum CameraMode{
	Instant = 0,
	Follow = 1
}

public class BattleCamera : MonoBehaviour {

	private PlayerController _playerController;
	private BattleField _battleField;
	public Camera ActiveCamera;
	private float camWidth;
	private float camHeight;
	// Use this for initialization
	void Start () {

		_playerController = BattleLogic.Instance.PlayerController;
		_battleField = BattleLogic.Instance.BattleField;
		
		camHeight = 2f * Camera.main.orthographicSize;
		camWidth = camHeight * Camera.main.aspect;
	}
	
	// Update is called once per frame
	void Update () {

		if (_playerController != null){
			moveToPlayer();
		} else {
			_playerController = BattleLogic.Instance.PlayerController;
		}
	}

	private void moveToPlayer(){

		bool moveX = false;
		bool moveY = false;
		float positionX = _playerController.transform.position.x;
		float positionY = _playerController.transform.position.y;
		
		bool leftEdgeReached, rightEdgeReached, topEdgeReached,bottomEdgeReached;

		leftEdgeReached = positionX - camWidth / 2 <= _battleField.MinWidth;
		rightEdgeReached = positionX + camWidth / 2 >= _battleField.MaxWidth;
		bottomEdgeReached = positionY - camHeight / 2 <= _battleField.MinHeight;
		topEdgeReached = positionY + camHeight / 2 >= _battleField.MaxHeight;

		if (!leftEdgeReached && !rightEdgeReached) {
			moveX = true;
		}

		if (!bottomEdgeReached && !topEdgeReached) {
			moveY = true;
		}
			
		float edgeX = 0;
		float edgeY = 0;

		if (!moveX){
			if (leftEdgeReached && !rightEdgeReached){
				edgeX = _battleField.MinWidth + camWidth / 2;
			} else if (rightEdgeReached && !leftEdgeReached){
				edgeX = _battleField.MaxWidth - camWidth / 2;
			}
		}

		if (!moveY){
			if (bottomEdgeReached && !topEdgeReached){
				edgeY = _battleField.MinHeight + camHeight / 2;
			} else if (topEdgeReached && !bottomEdgeReached){
				edgeY = _battleField.MaxHeight - camHeight / 2;
			}
		}

		transform.position = new Vector3 (moveX ? positionX : edgeX, moveY ? positionY : edgeY, transform.position.z);
		

	}
}
