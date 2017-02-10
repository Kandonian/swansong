using UnityEngine;
using System.Collections;

public class FreezePositions : MonoBehaviour {

    Rigidbody myRigidBody;
    public InteractionManager myInteractions;

    bool amIFrozen, amIFrozenTemporarily;

	// Use this for initialization
	void Start () {
        myRigidBody = GetComponent<Rigidbody>();
        amIFrozen = true;
	}
	
	// Update is called once per frame
	void Update () {
        if (myInteractions.isHoldingObj && (amIFrozen||amIFrozenTemporarily))
        {
            LetMeMove();
        }
        else if (myInteractions.wasHoldingObj && !amIFrozenTemporarily)
        {
            TempStopMeMoving();
        }
        else if(!myInteractions.isHoldingObj && !myInteractions.wasHoldingObj && !amIFrozen)
        {
            StopMeMoving();
        }
	}

    void LetMeMove()
    {
        amIFrozen = false;
        amIFrozenTemporarily = false;
        myRigidBody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
    }

    void TempStopMeMoving()
    {
        myRigidBody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        amIFrozenTemporarily = true;
    }

    void StopMeMoving()
    {
        if(myRigidBody.velocity.x == 0 && myRigidBody.velocity.y == 0)
        {
            amIFrozen = true;
            myRigidBody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        }
    }
}
