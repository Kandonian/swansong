using UnityEngine;
using System.Collections;

public class Throwing : MonoBehaviour {

    public Transform parentTransform;
    public SphereCollider sphere;
    bool beingHeld;

    void Update()
    {
        //To keep the ball on the player even when jumping and such
        if(beingHeld)
        {
            this.transform.position = new Vector3(parentTransform.position.x, parentTransform.position.y, parentTransform.position.z);
        }
    }
	
	public void SetParent()
    {
        //Set the needed content
        //So that the player can give the illusion of holding the object
        beingHeld = true;
        sphere.isTrigger = true;
        this.GetComponent<Rigidbody>().useGravity = false;
        this.transform.position = new Vector3(parentTransform.position.x, parentTransform.position.y, parentTransform.position.z);
        this.transform.SetParent(parentTransform);
    }

    public void RemoveParent()
    {
        //Revert the object back to normal
        beingHeld = false;
        sphere.isTrigger = false;
        this.transform.position = new Vector3(parentTransform.position.x, parentTransform.position.y, 15.4f);
        this.GetComponent<Rigidbody>().useGravity = true;
        this.transform.parent = null;
    }

}
