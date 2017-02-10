﻿using UnityEngine;
using System.Collections;

public class ZoneCamSwitch : MonoBehaviour {

	public ZoneCamera myCam;

	public int myRoomNumber;

	void OnTriggerEnter(Collider col){
		if(col.gameObject.name == "MyPlayer"){
			myCam.currentRoom = myRoomNumber;
		}
	}

}
