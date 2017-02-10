using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{

	//for movement
	public Rigidbody myBody;

	//for controlling the animation
	public AnimationManager myAnims;

	//use for interactions
	public InteractionManager myInteractions;

	//movement speeds for general movement
	public float mySpeed;
	public float myRunningSpeed;
	public float myJumpForce;
	public float myJumpSpeed;

	//for jumping mechanic
	public bool isOnGround;
	bool isGoingUp;

	//used for when the player can move
	public bool isControlling;


	// Use this for initialization
	void Start()
	{
		myBody = gameObject.GetComponent<Rigidbody>();
	}


	// Update is called once per frame
	void Update()
	{
		//If the player is being controlled allow the player to be controlled
		if (isControlling)
		{
			if (!myInteractions.isInteracting)
			{
				CheckGrounded();
				CheckInputs();
			}
		}
	}

	void CheckGrounded()
	{
		if ((myBody.velocity.y >= -0.001f && myBody.velocity.y <= 0.001f) & !isGoingUp)
		{
			isOnGround = true;
		}
		else
		{
			isOnGround = false;
		}
	}

	void CheckInputs()
	{
		//Check movement for when the player is grounded
		if (isOnGround)
		{
			GroundMovment();
		}
		//check movement for when the player is not grounded
		else
		{
			AirMovement();
		}

		//Slow down the player while there is no movement input and the player is grounded
		if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && isOnGround && !isGoingUp)
		{
			if (myBody.velocity.x > 0)
			{
				myBody.velocity = new Vector3(myBody.velocity.x - 0.1f, myBody.velocity.y, 0);
			}
			else if (myBody.velocity.x < 0)
			{
				myBody.velocity = new Vector3(myBody.velocity.x + 0.1f, myBody.velocity.y, 0);
			}

			myAnims.PlayIdle();
		}
	}

	void GroundMovment()
	{
		//Movement input for running
		if (Input.GetKey(KeyCode.LeftShift))
		{
			//run left
			if (Input.GetKey(KeyCode.A))
			{
				myAnims.PlayRun(true);
				myBody.velocity = new Vector3(-myRunningSpeed, myBody.velocity.y, 0.0f);
			}
			//run right
			else if (Input.GetKey(KeyCode.D))
			{
				myAnims.PlayRun(false);
				myBody.velocity = new Vector3(myRunningSpeed, myBody.velocity.y, 0.0f);
			}
		}
		//Movement input for walking
		else
		{
			//walk left
			if (Input.GetKey(KeyCode.A))
			{
				myAnims.PlayWalk(true);
				myBody.velocity = new Vector3(-mySpeed, myBody.velocity.y, 0.0f);
			}
			//walk right
			if (Input.GetKey(KeyCode.D))
			{
				myAnims.PlayWalk(false);
				myBody.velocity = new Vector3(mySpeed, myBody.velocity.y, 0.0f);
			}

			//Jump up
			if (Input.GetKey(KeyCode.W))
			{
				myAnims.PlayJump();
				isGoingUp = true;
			}
		}
	}

	void AirMovement()
	{
		//while the jumping animation is still playing
		if (myAnims.JumpingUp() && isGoingUp)
		{
			myBody.velocity = new Vector3(myBody.velocity.x, myJumpForce, 0.0f);
			isGoingUp = false;
		}

		if(!isGoingUp)
		{
			myAnims.PlayFall();
		}
	}

}

