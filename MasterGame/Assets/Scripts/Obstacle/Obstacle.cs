using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour {

	public Collider2D Collider;
	private SpriteRenderer[] _sprites;
	public SpriteRenderer[] Sprites {
		get { return _sprites;}
	}
	private Vector2 _position;
	public Vector2 Position{
		get { return _position;}
	}

	// Use this for initialization
	void Start () {

		_sprites = GetComponentsInChildren<SpriteRenderer>();
		setPosition();
		BattleLogic.Instance.DynamicSorting.AddObstacle(this);
	}
	
	// Update is called once per frame
	void Update () {
		if (tag != "StaticObstacle"){
			setPosition();
		}
	}

	private void setPosition(){
		float minX = _sprites[0].bounds.min.x;
		float minY = _sprites[0].bounds.min.y;
		foreach(SpriteRenderer sr in _sprites){
			if (sr.bounds.min.y < minY){
				minY = sr.bounds.min.y;
			}
			if (sr.bounds.min.x < minX){
				minX = sr.bounds.min.x;
			}
		}
		_position = new Vector2(minX,minY);
	}

	private void OnDestroy(){
		BattleLogic.Instance.DynamicSorting.RemoveObstacle(this);
	}
}
