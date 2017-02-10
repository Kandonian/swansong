using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour {

	Rigidbody myBody;

	AnimationManager myAnims;

    AudioManager myAudio;

	public ZoneCamera myZoneCam;

	public bool inControl;

	//shards
	int shardsCollected;
	public Text txtShardsCollected;

	//jumping
	bool isJumping;
	bool isPreJumping;

	//falling
	bool isOnGround;
	float fallSpeed;

	//door
	bool isTouchingDoor;
	GameObject doorObj;
	float doorTimer;
	bool isOpening;

	//ladder
	bool isTouchingLadder;
	GameObject ladderObj;
	bool isClimbing;

    //throwing
    bool isTouchingThrowable;
    GameObject throwableObj;
    bool isHoldingThrowable, wasHoldingThrowable;
    float throwTimer;

	//moving
	public float moveSpeed;
	public float runSpeed;
	public float climbingSpeed;

	// Use this for initialization
	void Start () {
        myBody = this.GetComponent<Rigidbody>();
        myAnims = this.GetComponent<AnimationManager>();
        myAudio = this.GetComponent<AudioManager>();
        throwTimer = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		GroundCheck ();
		if (inControl) {
			CheckInputs ();
		}
		UpdateUI ();
	}

	void UpdateUI(){
		txtShardsCollected.text = shardsCollected.ToString ();
	}

	void CheckInputs(){
			//make sure we are not opening a door
			if (!isOpening && !isClimbing) {
				//move left and right, right always takes prio
				if (!Input.GetKey (KeyCode.LeftShift)) {
					if (Input.GetKey (KeyCode.D) || Input.GetKey (KeyCode.RightArrow)) {
                        myAudio.Footsteps();
						Move (false);
					} else if (Input.GetKey (KeyCode.A) || Input.GetKey (KeyCode.LeftArrow)) {
						Move (true);
                        myAudio.Footsteps();
                } else {
						myBody.velocity = new Vector3 (0, myBody.velocity.y, myBody.velocity.z);
						if (isOnGround) {
							myAnims.PlayIdle ();
						}
					}
				} else {
					if (Input.GetKey (KeyCode.D) || Input.GetKey (KeyCode.RightArrow)) {
						Run (false);
					} else if (Input.GetKey (KeyCode.A) || Input.GetKey (KeyCode.LeftArrow)) {
						Run (true);
					} else {
						myBody.velocity = new Vector3 (0, myBody.velocity.y, myBody.velocity.z);
						if (isOnGround) {
							myAnims.PlayIdle ();
						}
					}
				}

                //Interactions currently
				if (Input.GetKeyDown (KeyCode.E)) {
					if (isTouchingDoor) {
						if (!doorObj.GetComponent<Door> ().isOpen) {
							OpenDoor ();
						}
					}

					if (isTouchingLadder) {
						ClimbLadder ();
					}

                    if(isTouchingThrowable)
                    {
                        PickUpThrowable();
                    }
				}
                //Get ready to throw if required
                else if(Input.GetKey (KeyCode.E) && wasHoldingThrowable)
                {
                    ThrowingAbility();
                }
                else if(Input.GetKeyUp(KeyCode.E) && wasHoldingThrowable)
                {
                    throwTimer = 0.0f;
                    wasHoldingThrowable = false;
                    throwableObj.GetComponent<Throwing>().RemoveParent();
                }

                //Jump section
				if (Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.UpArrow)) {
					if (isOnGround) {
						Jump ();
					}
				}

                //Escape
				if (Input.GetKey (KeyCode.Escape)) {
					Application.Quit ();
				}
			} else if (isOpening) {
				doorTimer += Time.deltaTime;
				if (doorTimer > 0.5f) {
					doorTimer = 0;
					isOpening = false;
				}
			} else if (isClimbing) {
				isOnGround = false;

				if (!isTouchingLadder) {
					isClimbing = false;
					myAnims.PlayIdle ();
				} else {
					if (Input.GetKey (KeyCode.S) || Input.GetKey (KeyCode.DownArrow)) {
						myAnims.PlayClimbingDown ();
						myBody.velocity = new Vector3 (0, -climbingSpeed, 0);
					} else if (Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.UpArrow)) {
						myAnims.PlayClimbingUp ();
						myBody.velocity = new Vector3 (0, climbingSpeed, 0);
					} else {
						myAnims.PauseClimbingAnimations ();
						myBody.velocity = new Vector3 (0, 0, 0);
					}

					if (Input.GetKeyDown (KeyCode.E)) {
						isClimbing = false;
						myAnims.PlayIdle ();
						myBody.velocity = new Vector3 (0, 400 * Time.deltaTime, 0);
					}
				}
			}
	}

	void ClimbLadder(){
		if (isClimbing) {
			isClimbing = false;
			isTouchingLadder = false;
			ladderObj = null;
		} else {
			myAnims.PlayClimbingUp();
			myAnims.PauseClimbingAnimations();
			isClimbing = true;
			transform.position = new Vector3(ladderObj.transform.position.x,transform.position.y,transform.position.z);
		}
	}

	void OpenDoor(){
		isOpening = true;
        myAudio.DoorOpen();
		if (transform.position.x > doorObj.transform.position.x) {
			//open door left
			doorObj.GetComponent<Door>().isOpen = true;
			doorObj.GetComponent<Door>().openDir = false;
			myAnims.PlayPull(true);
		} else {
			//open door right
			doorObj.GetComponent<Door>().isOpen = true;
			doorObj.GetComponent<Door>().openDir = true;
			myAnims.PlayPush(false);
		}
	}

	void Jump(){
		myAnims.PlayJump ();
		myBody.velocity = new Vector3 (0, 15, 0);
		isOnGround = false;
		isPreJumping = true;
	}

	void Run(bool moveDir){
		//move right
		if (!moveDir) {
			myBody.velocity = new Vector3(runSpeed,myBody.velocity.y,myBody.velocity.z);
			if(isOnGround){
				myAnims.PlayRun(false);
			}
		} else {
			myBody.velocity = new Vector3(-runSpeed,myBody.velocity.y,myBody.velocity.z);
			if(isOnGround){
				myAnims.PlayRun(true);
			}
		}
	}

	void Move(bool moveDir){
		//move right
		if (!moveDir) {
			myBody.velocity = new Vector3(moveSpeed,myBody.velocity.y,myBody.velocity.z);
			if(isOnGround){
				myAnims.PlayWalk(false);
			}
		} else {
			myBody.velocity = new Vector3(-moveSpeed,myBody.velocity.y,myBody.velocity.z);
			if(isOnGround){
				myAnims.PlayWalk(true);
			}
		}
	}

	void GroundCheck(){
		if (!isOnGround && !isClimbing) {
			//play falling animation
			if(!isPreJumping){
				myAnims.PlayFall();
			}
			if(fallSpeed < 1.75){
				fallSpeed += 0.025f;
				myBody.velocity = new Vector3(myBody.velocity.x,myBody.velocity.y - fallSpeed,0);
				//check to change from prejump to falling
				if(isPreJumping){
					if(myBody.velocity.y < 0){
						isPreJumping = false;
					}
				}
			}
		}
	}

    void ThrowingAbility()
    {
        if (throwTimer >= 1.0f)
        {
            throwableObj.GetComponent<Throwing>().RemoveParent();

            //Dependant on direction
            if (myAnims.FacingLeft())
            { 
                throwableObj.GetComponent<Rigidbody>().velocity = new Vector3(-15.0f, 10.0f, 0.0f);
            }
            else
            {
                throwableObj.GetComponent<Rigidbody>().velocity = new Vector3(15.0f, 10.0f, 0.0f);
            }

            throwTimer = 0.0f;
            wasHoldingThrowable = false;
        }
        else
        {
            throwTimer += Time.deltaTime;
        }
    }

    void PickUpThrowable()
    {
        //Check if an object is being held currently
        if (isTouchingThrowable && !isHoldingThrowable)
        {
            isHoldingThrowable = true;
            throwableObj.GetComponent<Throwing>().SetParent();
        }
        else if (isTouchingThrowable && isHoldingThrowable)
        {
            throwTimer += Time.deltaTime;
            isHoldingThrowable= false;
            wasHoldingThrowable = true;
        }
    }

	void OnCollisionEnter(Collision col){
		if (col.gameObject.tag == "Ground") {
			if(!isOnGround){
				fallSpeed = 0;
				isOnGround = true;
				//set to idle animation
				myAnims.PlayIdle();
			}
		}
	}

	void OnCollisionExit(Collision col){
		if (col.gameObject.tag == "Ground") {
			isOnGround = false;
		}
	}

	void OnTriggerEnter(Collider col){
		if (col.gameObject.tag == "Door") {
			if (col.gameObject.GetComponent<Door> ().isOpen == false) {
				isTouchingDoor = true;
				doorObj = col.gameObject;
			}
		} else if(col.gameObject.tag != "Switch"){
			isTouchingDoor = false;
		}

		if (col.gameObject.tag == "Ladder") {
			isTouchingLadder = true;
			ladderObj = col.gameObject;
		} else if(col.gameObject.tag != "Switch"){
			isTouchingLadder = false;
		}

		if (col.gameObject.tag == "Collectable") {
            myAudio.CrystalCollect();
			Destroy(col.gameObject);
			shardsCollected ++;
		}

        //the collection for throwable objects
        if(col.gameObject.tag == "Throwable")
        {
            throwableObj = col.gameObject;
            isTouchingThrowable = true;
        }
	}

    //Temporary fix only
    void OnTriggerStay(Collider col)
    {
        //the collection for throwable objects
        if (col.gameObject.tag == "Throwable")
        {
            throwableObj = col.gameObject;
            isTouchingThrowable = true;
        }
    }

	void OnTriggerExit(Collider col){
		if (col.gameObject.tag == "Ladder") {
			isTouchingLadder = false;
			ladderObj = null;
		}

        if(col.gameObject.tag == "Throwable")
        {
            isTouchingThrowable = false;
            throwableObj = null;
        }
	}
}
