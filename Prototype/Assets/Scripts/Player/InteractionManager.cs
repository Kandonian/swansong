using UnityEngine;
using System.Collections;

public class InteractionManager : MonoBehaviour {

	//For holding the body
	Rigidbody myBody;
	Rigidbody myPushableBody;
	Rigidbody myThrowableBody;
	public PlayerMovement myPlayer;

	//Set the speed dependant on each interaction
	public float myClimbingSpeed;
	public float myPushSpeed;
	public Vector2 myThrowingForce;

	//For deciding whether an object is placed or thrown
	public float myTimeToThrow;
	float myThrowingTimer;

	//bools to check what interactions can-be/are done
	bool isClimbing;
	bool isMovingObj;
	bool isMovingDoor;
	public bool isHoldingObj;
	bool isAbleToClimb;
	bool isAbleToMoveObj;
	bool isAbleToHoldObj;
	bool isAbleToMoveDoor;
	bool isPushingDoorRight;
	public bool wasClimbing;
	public bool wasHoldingObj;

	//if the character is interacting
	public bool isInteracting;

	//For use with collectable objects
	int score;
	public float myScoreDisplayTime;
	float myScoreTimeRemaining;
	public UnityEngine.UI.Text myScore;
	public UnityEngine.UI.Image myCollectableImage;

	// Use this for initialization
	void Start () {
		myBody = gameObject.GetComponent<Rigidbody>();
		myThrowingTimer = 0.0f;
		score = 0;
		myScore.enabled = false;
		myCollectableImage.enabled = false;
	}

	// Update is called once per frame
	void Update () {
		
        //Check the timer for the score to display is above 0
		if (myScoreTimeRemaining > 0) 
		{
            ManageScore();
		}

        //Check for any interaction changes (space key activated)
        InteractionUpdateCheck();

        //Check if the player is now interacting with anything
		CheckIfInteracting();

        //If interacting allow the appropriate movement and input options
		if (isInteracting)
		{
			CheckInput();
		}
	}

    void InteractionUpdateCheck()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CheckInteraction();
        }
        else if (Input.GetKey(KeyCode.Space) && wasHoldingObj)
        {
            myThrowingTimer += Time.deltaTime;

            if (myThrowingTimer >= myTimeToThrow)
            {
                CheckIfThrown();
            }
        }
        else if (Input.GetKeyUp(KeyCode.Space) && wasHoldingObj)
        {
            myThrowingTimer = 0;
            wasHoldingObj = false;
        }
    }

	void ManageScore()
	{
        myScoreTimeRemaining -= Time.deltaTime;

        //If the time is now below 0, disable the UI currently being displayed
        if (myScoreTimeRemaining <= 0)
        {
            myScore.enabled = false;
            myCollectableImage.enabled = false;
        }
    }

	void CheckInteraction()
	{
        //Check if the player is able to climb
		if(isAbleToClimb && !isClimbing)
		{
			myBody.velocity = new Vector3(0, 0, 0);
			myBody.useGravity = false;
			isClimbing = true;
            this.GetComponent<BoxCollider>().isTrigger = true;
		}
		else if (isAbleToClimb && isClimbing)
		{
			myBody.useGravity = true;
			isClimbing = false;
			wasClimbing = true;
            this.GetComponent<BoxCollider>().isTrigger = false;
		}

        //Check if the player is moving an object
		if (isAbleToMoveObj && !isMovingObj)
		{
			isMovingObj = true;
			myPushableBody.constraints =  RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
		}
		else if (isAbleToMoveObj && isMovingObj)
		{
			isMovingObj = false;
			myPushableBody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
		}
        
        //Check if the player is interacting with a door
		if (isAbleToMoveDoor && !isMovingDoor)
		{
			isMovingDoor = true;
			myPushableBody.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
			if (myPushableBody.transform.position.x > transform.position.x)
			{
				isPushingDoorRight = true;
			}
		}
		else if (isAbleToMoveDoor && isMovingDoor)
		{
			isMovingDoor = false;
			myPushableBody.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
			isPushingDoorRight = false;
		}

        //Check if an object is being held currently
		if (isAbleToHoldObj && !isHoldingObj)
		{
			isHoldingObj = true;
		}
		else if(isAbleToHoldObj && isHoldingObj)
		{
			myThrowingTimer += Time.deltaTime;
			isHoldingObj = false;
			wasHoldingObj = true;
		}
	}

	void CheckIfThrown()
	{
		if (myThrowableBody != null)
		{
			myThrowableBody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

			//Checking which way to throw the object - to be updated with the player is facing left or right after
			if (myThrowableBody.transform.position.x < transform.position.x)
			{
				myThrowableBody.velocity = new Vector3(-myThrowingForce.x, myThrowingForce.y, 0);
			}
			else
			{
				myThrowableBody.velocity = new Vector3(myThrowingForce.x, myThrowingForce.y, 0);
			}
		}

		wasHoldingObj = false;
		myThrowingTimer = 0;
	}

	void CheckIfInteracting()
	{
        //See if the player is interacting or not
		if (isClimbing || isMovingObj || isHoldingObj || wasHoldingObj || isMovingDoor)
		{
			isInteracting = true;
		}
		else
		{
			isInteracting = false;
		}
	}

	void CheckInput()
	{
        //Run the appropriate interaction input options
		if(isClimbing)
		{
			ClimbingInput();
		}

		if(isMovingObj)
		{
			MovingObjInput();
		}

		if (isMovingDoor)
		{
			MovingDoorInput();
		}

		if(isHoldingObj)
		{
			HoldingObjInput();
		}
	}

	void ClimbingInput()
	{
		if (Input.GetKey(KeyCode.W))
		{
            myPlayer.myAnims.PlayClimbingUp();
			myBody.velocity = new Vector3(0, myClimbingSpeed, 0);
		}

		if (Input.GetKey(KeyCode.S))
		{
            myPlayer.myAnims.PlayClimbingDown();
			myBody.velocity = new Vector3(0, -myClimbingSpeed, 0);
		}

		if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
		{
            myPlayer.myAnims.PauseClimbingAnimations();
			myBody.velocity = new Vector3(0, 0, 0);
		}
	}

	void MovingDoorInput()
	{

		if (!isPushingDoorRight)
		{
			if (Input.GetKey(KeyCode.A))
			{
				myPlayer.myAnims.PlayPull(true);
				myBody.velocity = new Vector3(-myPushSpeed, 0, 0);
				myPushableBody.transform.Rotate(0, 1.0f, 0);
			}

			if (Input.GetKey(KeyCode.D))
			{
				myPlayer.myAnims.PlayPull(true);
				myBody.velocity = new Vector3(myPushSpeed, 0, 0);
				myPushableBody.transform.Rotate(0, -1.0f, 0);
			}
		}
		else
		{
			if (Input.GetKey(KeyCode.A))
			{
				myPlayer.myAnims.PlayPull(false);
				myBody.velocity = new Vector3(-myPushSpeed, 0, 0);
				myPushableBody.transform.Rotate(0, 1.0f, 0);
			}

			if (Input.GetKey(KeyCode.D))
			{
				myPlayer.myAnims.PlayPush(false);
				myBody.velocity = new Vector3(myPushSpeed, 0, 0);
				myPushableBody.transform.Rotate(0, -1.0f, 0);
			}
		}

		if(!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
		{
			myPlayer.myAnims.PlayIdle();
		}

	}

	void MovingObjInput()
	{
		if (myPushableBody.transform.position.x < transform.position.x)
		{
			if (Input.GetKey(KeyCode.A))
			{
				myBody.velocity = new Vector3(-myPushSpeed, myBody.velocity.y, 0);
				myPushableBody.velocity = new Vector3(-myPushSpeed, myPushableBody.velocity.y, 0);
			}

			if (Input.GetKey(KeyCode.D))
			{
				myBody.velocity = new Vector3(myPushSpeed, myBody.velocity.y, 0);
				myPushableBody.velocity = new Vector3(myPushSpeed * 1.3f, myPushableBody.velocity.y, 0);
			}
		}
		else
		{
			if (Input.GetKey(KeyCode.A))
			{
				myBody.velocity = new Vector3(-myPushSpeed, myBody.velocity.y, 0);
				myPushableBody.velocity = new Vector3(-myPushSpeed * 1.3f, myPushableBody.velocity.y, 0);
			}

			if (Input.GetKey(KeyCode.D))
			{
				myBody.velocity = new Vector3(myPushSpeed, myBody.velocity.y, 0);
				myPushableBody.velocity = new Vector3(myPushSpeed, myPushableBody.velocity.y, 0);
			}
		}
		if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
		{
			myBody.velocity = new Vector3(0, myBody.velocity.y, 0);
			myPushableBody.velocity = new Vector3(0, myPushableBody.velocity.y, 0);
		}
	}

	void HoldingObjInput()
	{
		if (myThrowableBody.transform.position.x < transform.position.x)
		{
			if (Input.GetKey(KeyCode.A))
			{
				myBody.velocity = new Vector3(-myPushSpeed, myBody.velocity.y, 0);
				myThrowableBody.velocity = new Vector3(-myPushSpeed, myThrowableBody.velocity.y, 0);
			}

			if (Input.GetKey(KeyCode.D))
			{
				myBody.velocity = new Vector3(myPushSpeed, myBody.velocity.y, 0);
				myThrowableBody.velocity = new Vector3(myPushSpeed * 1.3f, myThrowableBody.velocity.y, 0);
			}
		}
		else
		{
			if (Input.GetKey(KeyCode.A))
			{
				myBody.velocity = new Vector3(-myPushSpeed, myBody.velocity.y, 0);
				myThrowableBody.velocity = new Vector3(-myPushSpeed * 1.3f, myThrowableBody.velocity.y, 0);
			}

			if (Input.GetKey(KeyCode.D))
			{
				myBody.velocity = new Vector3(myPushSpeed, myBody.velocity.y, 0);
				myThrowableBody.velocity = new Vector3(myPushSpeed, myThrowableBody.velocity.y, 0);
			}
		}
		if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
		{
			myBody.velocity = new Vector3(0, myBody.velocity.y, 0);
			myThrowableBody.velocity = new Vector3(0, 0, 0);
		}
	}

	void OnTriggerEnter(Collider other)
	{

		if (other.tag == "Ladder")
		{
			isAbleToClimb = true;
		}

		if(other.tag == "Pushable")
		{
			if (myPushableBody == null)
			{
				myPushableBody = other.GetComponent<Rigidbody>();
				isAbleToMoveObj = true;
			}
		}

		if(other.tag == "Throwable")
		{
			if(myThrowableBody == null)
			{
				myThrowableBody = other.GetComponent<Rigidbody>();
				isAbleToHoldObj = true;
			}
		}

		if (other.tag == "Door")
		{
			if (myPushableBody == null)
			{
				myPushableBody = other.GetComponent<Rigidbody>();
				isAbleToMoveDoor = true;
				BoxCollider box = other as BoxCollider;
				box.size = new Vector3(120.0f, 200.0f, 100.0f);
			}
			myBody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
		}

		if (other.tag == "Collectable") 
		{
			//Update and display the score
			score++;
			myScore.text = "" + score;
			myScore.enabled = true;

			//Set how long the score should display for,
			//then delete the picked up object
			myScoreTimeRemaining = myScoreDisplayTime;
			myCollectableImage.enabled = true;
			Destroy (other.gameObject);
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Ground")
		{
			myPlayer.isOnGround = false;
		}

		if (other.tag == "Ladder")
		{
			isAbleToClimb = false;
			isClimbing = false;
			wasClimbing = false;
			myBody.useGravity = true;
            this.GetComponent<BoxCollider>().isTrigger = false;
		}

		if(other.tag == "Pushable")
		{

			if (myPushableBody == other.GetComponent<Rigidbody>())
			{
				isAbleToMoveObj = false;
				isMovingObj = false;
				myPushableBody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
				myPushableBody = null;
			}
		}

		if (other.tag == "Throwable")
		{
			if (myThrowableBody == other.GetComponent<Rigidbody>())
			{
				isAbleToHoldObj = false;
				isHoldingObj = false;
				myThrowableBody = null;
				wasHoldingObj = false;
			}
		}

		if (other.tag == "Door")
		{
			if (myPushableBody == other.GetComponent<Rigidbody>())
			{
				isAbleToMoveDoor = false;
				isMovingDoor = false;
				myPushableBody.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
				myPushableBody = null;
				BoxCollider box = other as BoxCollider;
				box.size = new Vector3(120.0f, 200.0f, 25.0f);

			}
			myBody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
		}
	}
}
