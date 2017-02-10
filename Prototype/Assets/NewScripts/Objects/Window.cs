using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window : MonoBehaviour {

    void OnTriggerEnter(Collider other)
    {
        //if the window is hit by a throwable object
        if(other.tag == "Throwable")
        {
            //Set the rigidbody
            Rigidbody throwingBody = other.GetComponent<Rigidbody>();

            if (throwingBody.velocity.x >= 10.0f || throwingBody.velocity.x <= -10.0f)
            {
                //Allow the player to now interact/pass through the window
                GetComponent<BoxCollider>().isTrigger = true;

                //Add code here for any change in the appearance of the window
            }
        }
    }
}
