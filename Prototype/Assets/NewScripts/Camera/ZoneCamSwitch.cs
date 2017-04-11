using UnityEngine;
using System.Collections;

public class ZoneCamSwitch : MonoBehaviour {

	public ZoneCamera myCam;

	public int myRoomNumber;

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.name == "MyPlayer")
        {
            if (myCam)
            {
                if (myCam.currentRoom == 4 && myRoomNumber == 2)
                {
                    myCam.GetComponent<CameraManager>().SwitchToRoom();
                }
                myCam.currentRoom = myRoomNumber;
            }
        }
    }
}
