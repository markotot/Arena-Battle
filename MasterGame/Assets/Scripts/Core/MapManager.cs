using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {

	private List<Tile> _tileList = new List<Tile>();
	public List<Tile> TileList{
		get{ return _tileList;}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void AddTile(Tile t){

		Vector2 pos = t.TilePosition;
		for (int i = 0; i < _tileList.Count; i++){

			if ( pos.x >= _tileList[i].TilePosition.x && pos.y >= _tileList[i].TilePosition.y){
				_tileList.Insert(i,t);
			}
		}
	}

	public void SortTiles(){
		for(int i = 0; i < _tileList.Count; i++){
			_tileList[i].SpriteRenderer.sortingOrder = i;
		}
	}

}
