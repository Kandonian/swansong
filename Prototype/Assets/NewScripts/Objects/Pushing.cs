using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pushing : MonoBehaviour {

    public Transform parentTransform;
    public bool refuseMoveRight, refuseMoveLeft;
    bool beingHeld;
    Vector2 distance;

    void Update()
    {
        //To keep the box next to the player
        if (beingHeld)
        {
            this.transform.position = new Vector3(parentTransform.position.x + distance.x, this.transform.position.y + distance.y, parentTransform.position.z);
        }
    }

    public void SetParent(bool NegativeX)
    {
        //Set the needed content
        //So that the player can give the illusion of actually touching the object
        beingHeld = true;
        

        if(!NegativeX)
        {
            distance = new Vector2(2.3f, -0.0f);
        }
        else
        {
            distance = new Vector2(2.3f, -0.0f);
        }
        
    }

    public void RemoveParent()
    {
        //Revert the object back to normal
        beingHeld = false;
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Wall" || other.gameObject.tag == "Window" ||
            other.gameObject.tag == "Door")
        {
            if (this.transform.position.x < other.gameObject.transform.position.x)
            {
                refuseMoveRight = true;
            }
            else
            {
                refuseMoveLeft = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Wall" || other.gameObject.tag == "Window" ||
            other.gameObject.tag == "Door")
        {
            if (this.transform.position.x < other.gameObject.transform.position.x)
            {
                refuseMoveRight = false;
            }
            else
            {
                refuseMoveLeft = false;
            }
        }
    }
}
