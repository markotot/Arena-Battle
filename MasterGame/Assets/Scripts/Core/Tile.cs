using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType{
	None = 0,
	GrassTile = 1,
	DirtTile = 2,
	ConcreteTile = 3,
	SandTile = 4,
	WaterTile = 5
}

public class Tile : MonoBehaviour {


	public bool Walkable;
	public bool Ice;
	public float SpeedReduction;

	private Tile _leftNeighbour;
	private Tile _rightNeighbour;
	private Tile _topNeighbour;
	private Tile _bottomNeighbour;

	private TileType _tileType = TileType.None;
	public TileType TileType {
		get { return _tileType;}
	}

	private Vector2 _tilePosition;
	public Vector2 TilePosition{
		get { return _tilePosition;}
	}

	public SpriteRenderer SpriteRenderer;

	private bool _playerOnTile;
	public bool PlayerOnTile{
		get { return _playerOnTile;}
	}
	// Use this for initialization
	void Start () {

		RaycastHit2D[] hitsTop = Physics2D.RaycastAll(gameObject.transform.position,Vector2.up,1f);
		RaycastHit2D[] hitsBottom = Physics2D.RaycastAll(gameObject.transform.position,Vector2.down,1f);
		RaycastHit2D[] hitsLeft = Physics2D.RaycastAll(gameObject.transform.position,Vector2.left,1f);
		RaycastHit2D[] hitsRight = Physics2D.RaycastAll(gameObject.transform.position,Vector2.right,1f);
		
		foreach(RaycastHit2D hit in hitsTop){
			if (hit.collider.gameObject != gameObject && hit.collider.gameObject.GetComponent<Tile>() != null){
				_topNeighbour = hit.collider.gameObject.GetComponent<Tile>();
				break;
			}
		}
		foreach(RaycastHit2D hit in hitsLeft){
			if (hit.collider.gameObject != gameObject && hit.collider.gameObject.GetComponent<Tile>() != null){
				_leftNeighbour = hit.collider.gameObject.GetComponent<Tile>();
				break;
			}
		}
		foreach(RaycastHit2D hit in hitsRight){
			if (hit.collider.gameObject != gameObject && hit.collider.gameObject.GetComponent<Tile>() != null){
				_rightNeighbour = hit.collider.gameObject.GetComponent<Tile>();
				break;
			}
		}
		foreach(RaycastHit2D hit in hitsBottom){
			if (hit.collider.gameObject != gameObject && hit.collider.gameObject.GetComponent<Tile>() != null){
				_bottomNeighbour = hit.collider.gameObject.GetComponent<Tile>();
				break;
			}
		}

		_tilePosition = transform.position;
		BattleLogic.Instance.MapManager.AddTile(this);	
	}

	void OnTriggerEnter2D(Collider2D col){
		if (col.tag == "Character" || col.tag == "Player"){
			if (col.Equals(BattleLogic.Instance.PlayerController.MovementCollider) && !_playerOnTile){
				BattleLogic.Instance.PlayerController.AddTile(this);
				_playerOnTile = true;
			}
		}
	}

	void OnTriggerExit2D(Collider2D col){
		if (col.tag == "Character" || col.tag == "Player"){
			if (col.Equals(BattleLogic.Instance.PlayerController.MovementCollider) && _playerOnTile){
				BattleLogic.Instance.PlayerController.RemoveTile(this);
				_playerOnTile = false;
			}
		}
	}
}
