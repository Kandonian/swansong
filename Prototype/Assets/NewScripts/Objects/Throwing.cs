using UnityEngine;
using System.Collections;

public class Throwing : MonoBehaviour {

    public Transform parentTransform;
    public SphereCollider sphere;
    bool beingHeld, dropping;
    public float fallSpeed = 0.0f;

    void Update()
    {
        //To keep the ball on the player even when jumping and such
        if(beingHeld)
        {
            this.transform.position = new Vector3(parentTransform.position.x, parentTransform.position.y, parentTransform.position.z);
        }
        else if(dropping)
        {
            if (fallSpeed < 1.0f)
            {
                fallSpeed += Time.deltaTime;
            }
            GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x, GetComponent<Rigidbody>().velocity.y - fallSpeed, 0);
        }
    }
	
	public void SetParent()
    {
        //Set the needed content
        //So that the player can give the illusion of holding the object
        beingHeld = true;
        dropping = false;
        sphere.isTrigger = true;
        this.GetComponent<Rigidbody>().useGravity = false;
        this.transform.position = new Vector3(parentTransform.position.x, parentTransform.position.y, parentTransform.position.z);
        this.transform.SetParent(parentTransform);
    }

    public void RemoveParent()
    {
        //Revert the object back to normal
        beingHeld = false;
        dropping = true;
        sphere.isTrigger = false;
        this.transform.position = new Vector3(parentTransform.position.x, parentTransform.position.y, 15.4f);
        this.GetComponent<Rigidbody>().useGravity = true;
        this.transform.parent = null;
    }

    void OnCollisionEnter(Collision col)
    {
        if ((col.gameObject.layer == LayerMask.NameToLayer("Ground")
            || col.gameObject.layer == LayerMask.NameToLayer("Dropable")))
        {
            dropping = false;
            fallSpeed = 0.0f;
            GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x, 0.0f, 0);
        }
    }
}
