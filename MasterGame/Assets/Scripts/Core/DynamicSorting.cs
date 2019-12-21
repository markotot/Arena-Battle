using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class DynamicSorting : MonoBehaviour {

	private List<Obstacle> _obstacleList = new List<Obstacle>();

	// Use this for initialization
	void Start () {
		GameObject.FindGameObjectsWithTag("UndestructibleObstacle");
	}
	
	// Update is called once per frame
	void Update () {

		IEnumerable sortedList = _obstacleList.OrderByDescending( temp => temp.Position.y);

		int count = 0;
		foreach(Obstacle o in sortedList){
			foreach(SpriteRenderer s in o.Sprites){
				s.sortingOrder = count++;
			}
		}
	}

	public void AddObstacle(Obstacle o){
		if (!_obstacleList.Contains(o)){
			_obstacleList.Add(o);
		}
	}

	public void RemoveObstacle(Obstacle o){
		_obstacleList.Remove(o);
	}
}
