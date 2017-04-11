using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    FollowMe followScript;
    ZoneCamera zoneScript;

    void Start()
    {
        followScript = GetComponent<FollowMe>();
        followScript.enabled = false;
        zoneScript = GetComponent<ZoneCamera>();
    }

	public void SwitchToFollow()
    {
        zoneScript.enabled = false;
        followScript.enabled = true;
    }

    public void SwitchToRoom()
    {
        zoneScript.enabled = true;
        followScript.enabled = false;
    }
}
