using UnityEngine;
using System.Collections;

public class FirePlace : MonoBehaviour {

    public ZoneCamera myZoneCamera;
    bool playing;
	
	// Update is called once per frame
	void Update () {
        if (myZoneCamera.currentRoom == 3 && playing == false)
        {
            GetComponent<AudioSource>().Play();
            playing = true;
        }
        else if (playing == true && myZoneCamera.currentRoom != 3)
        {
            GetComponent<AudioSource>().Stop();
            playing = false;
        }
	}
}
