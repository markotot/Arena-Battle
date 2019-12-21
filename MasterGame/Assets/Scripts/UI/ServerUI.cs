using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ServerUI : MonoBehaviour {

	public static ServerUI Instance;

	public Text LatencyText;
	public Text ServerTimeText;
	public Text TimeDeltaText;


	private DateTime _serverClock;
	public DateTime ServerClock{
		get { return _serverClock;}
	}

	private float _latency;
	public float Latency {
		get { return _latency;}
	}

	private float _timeDelta;
	public float TimeDelta {
		get { return _timeDelta;}
	}

	// Use this for initialization
	void Start () {

		if (Instance == null){
			Instance = this;
		}

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetLatency(){
		LatencyText.text = Mathf.Ceil(_latency).ToString() + " ms";
	}

	public void SetServerTime(){
		ServerTimeText.text = _serverClock.TimeOfDay.ToString();
	}

	public void SetTimeDelta(){
		TimeDeltaText.text = _timeDelta.ToString();
	}

	public void UpdateServerInfo(long clientTime, long serverTime){
		CalculateTimeDelta(clientTime,serverTime);
	}

	public void CalculateTimeDelta(long clientTime, long serverTime){

		// calculate the time taken from the packet to be sent from the client and then for the server to return it //
		int roundTrip = (int)((long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds - clientTime);
		_latency = roundTrip / 2; // the latency is half the round-trip time
		// calculate the server-delta from the server time minus the current time
		int serverDelta = (int)(serverTime - (long)(DateTime.UtcNow - new DateTime (1970, 1, 1, 0, 0, 0)).TotalMilliseconds);
		_timeDelta = serverDelta + _latency;

		DateTime dateNow = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc); // get the current time
		_serverClock = dateNow.AddMilliseconds (serverTime + _timeDelta).ToLocalTime ();

		SetLatency();
		SetTimeDelta();
		SetServerTime();
	}
}
