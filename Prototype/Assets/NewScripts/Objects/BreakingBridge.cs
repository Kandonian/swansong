using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakingBridge : MonoBehaviour {

	void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "Pushable")
        {
            GetComponent<BoxCollider>().enabled = false;
        }
    }
}
