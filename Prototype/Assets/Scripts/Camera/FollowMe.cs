using UnityEngine;
using System.Collections;

public class FollowMe : MonoBehaviour {

    //Take in the object that is to be followed
    public Rigidbody myPlayer;

    //For holding the offset the camera shall sit at
    Vector3 myOffset;

	// Use this for initialization
	void Start () {
        //work out the intial offset that should be maintained
       myOffset = transform.position - myPlayer.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        //every run through make the camera follow the player
       transform.position = myPlayer.transform.position + myOffset;
	}
}
