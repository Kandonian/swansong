using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour {

	Rigidbody myBody;

	AnimationManager myAnims;

    AudioManager myAudio;

	public bool inControl;

	//shards
	int shardsCollected;
	public Text txtShardsCollected;
    public Image imageShards;
    float displayTimer;
    public float UIDisplayTime;

	//jumping
	bool isPreJumping;

    //Fixing landing stuff
    bool justAboveGround;

	//falling
	public bool isOnGround;
	float fallSpeed;

    //drop platforms
    public float myFullDropTime;
    float myCurrentDropTime;
    bool isAbleToDrop;

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
    public Vector2 throwSpeed;

    //push/pull
    bool isTouchingPushable;
    GameObject pushableObj;
    bool isPushing, isPushingFromLeft;

	//moving
	public float moveSpeed;
	public float runSpeed;
	public float climbingSpeed;

    //for culling abilities
    int cullNum = -1;
    bool cullPush, cullPick, cullClimb;

	// Use this for initialization
	void Start () {
        myBody = this.GetComponent<Rigidbody>();
        myAnims = this.GetComponent<AnimationManager>();
        myAudio = this.GetComponent<AudioManager>();
        throwTimer = 0.0f;
        myCurrentDropTime = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		GroundCheck ();
		if (inControl) {
			CheckInputs ();
            if (isAbleToDrop && !isClimbing)
            {
                CheckPlayerDropping();
            }
        }
        UpdateUI();

        //Escape
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

	void UpdateUI(){
        if(displayTimer>0.0f)
        {
            displayTimer -= Time.deltaTime;
        }
        else
        {
            txtShardsCollected.enabled = false;
            imageShards.enabled = false;
        }
	}

    void ShowUI()
    {
        //Set the time for the UI to display
        displayTimer = UIDisplayTime;

        //Do appropriate steps to show the UI
        txtShardsCollected.enabled = true;
        imageShards.enabled = true;
        txtShardsCollected.text = shardsCollected.ToString();
    }

	void CheckInputs(){
			//make sure we are not opening a door
			if (!isOpening && !isClimbing && !isPushing) {
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
				if (Input.GetKeyDown (KeyCode.E))
                {
                         //If the push/pull ability is still in use
                        if (!cullPush)
                        {
                            if (isTouchingDoor)
                            {
                                if (!doorObj.GetComponent<Door>().isOpen)
                                {
                                    OpenDoor();
                                }
                            }
                        }

                        if (isTouchingPushable && isOnGround)
                        {
                            InitialPush();
                        }
                    

                    //if the player can climb and is allowed to
                    if (isTouchingLadder && !cullClimb)
                    {
						    ClimbLadder ();
					}

                    if(isTouchingThrowable && !cullPick)
                    {
                        PickUpThrowable();
                    }
				}
                //Get ready to throw if required
                else if(Input.GetKey (KeyCode.E) && wasHoldingThrowable)
                {
                    throwTimer += Time.deltaTime;
                }
                else if(Input.GetKeyUp(KeyCode.E) && wasHoldingThrowable)
                {
                    int strength = (int)System.Math.Round(throwTimer);
                    if(strength > 3)
                    {
                        strength = 3;
                    }
                    ThrowingAbility(strength);     
                }

                //Jump section
				if (Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.UpArrow)) {
					if (isOnGround) {
						Jump ();
					}
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
					myBody.useGravity = true;
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
						myBody.useGravity = true;
						myAnims.PlayIdle ();
						myBody.velocity = new Vector3 (0, 400 * Time.deltaTime, 0);
					}
				}
			}
            else if(isPushing)
            {
                if (!cullPush)
                {
                    PushMovement();
                }
              
                //Stop pushing the object under the below conditions
                if(Input.GetKeyDown(KeyCode.E) || transform.position.y>=pushableObj.transform.position.y + 5.0f 
                || transform.position.y < pushableObj.transform.position.y - 5.0f)
                {
                    isPushing = false;
                    isTouchingPushable = false;
                    myAnims.PlayIdle();
                    myBody.velocity = new Vector3(0.0f, myBody.velocity.y, 0.0f);
                    pushableObj.GetComponent<Pushing>().RemoveParent();
                    pushableObj = null;
                }
            }
	}

	void ClimbLadder(){
		
			myBody.useGravity = false;
			myAnims.PlayClimbingUp();
			myAnims.PauseClimbingAnimations();
			isClimbing = true;
			transform.position = new Vector3(ladderObj.transform.position.x,transform.position.y,transform.position.z);
	
	}

    void CullNext()
    {
        cullNum++;

        switch(cullNum)
        {
            case 0: cullPush = true;
                break;
            case 1: cullClimb = true;
                break;
            case 2: cullPick = true;
                break;
            default:
                break;
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
		myBody.velocity = new Vector3 (0, 20, 0);
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
			if(!isPreJumping && !justAboveGround){
				myAnims.PlayFall();
			}
            else if (!isPreJumping && justAboveGround)
            {
                if (myBody.velocity.y >= -0.5f)
                {
                    myBody.velocity = new Vector3(myBody.velocity.x, myBody.velocity.y - 0.5f, 0);
                    fallSpeed = 0.0f;
                    isOnGround = true;
                }
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

    void ThrowingAbility(int Strength)
    {
       
        throwableObj.GetComponent<Throwing>().RemoveParent();

        //Dependant on direction
        if (myAnims.FacingLeft())
        { 
            throwableObj.GetComponent<Rigidbody>().velocity = new Vector3(-throwSpeed.x * Strength, throwSpeed.y * Strength, 0.0f);
        }
        else
        {
            throwableObj.GetComponent<Rigidbody>().velocity = new Vector3(throwSpeed.x * Strength, throwSpeed.y * Strength, 0.0f);
        }

        throwTimer = 0.0f;
        wasHoldingThrowable = false;
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

    void PushMovement()
    {
        if (isPushingFromLeft)
        {
            //Take in general movement, right direction takes priority
            if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) 
                && (!pushableObj.GetComponent<Pushing>().refuseMoveRight))
            {
                myBody.velocity = new Vector3(moveSpeed * 0.7f, myBody.velocity.y, 0.0f);
                myAnims.PlayPull(isPushingFromLeft);
            }
            else if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) 
                && (!pushableObj.GetComponent<Pushing>().refuseMoveLeft))
            {
                myBody.velocity = new Vector3(-moveSpeed * 0.7f, myBody.velocity.y, 0.0f);
                myAnims.PlayPush(isPushingFromLeft);
            }
            else
            {
                //There has been no movement
                myAnims.PausePushPull();
                myBody.velocity = new Vector3(0.0f, myBody.velocity.y, 0.0f);
            }
        }
        else
        {
            //Take in general movement, right direction takes priority
            if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                && (!pushableObj.GetComponent<Pushing>().refuseMoveRight))
            {
                myBody.velocity = new Vector3(moveSpeed * 0.7f, myBody.velocity.y, 0.0f);
                myAnims.PlayPush(isPushingFromLeft);
            }
            else if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) 
                && (!pushableObj.GetComponent<Pushing>().refuseMoveLeft))
            {
                myBody.velocity = new Vector3(-moveSpeed * 0.7f, myBody.velocity.y, 0.0f);
                myAnims.PlayPull(isPushingFromLeft);
            }
            else
            {
                //There has been no movement
                myAnims.PausePushPullAfter();        //Pause the animations after the remainder has been played
                                                
                myBody.velocity = new Vector3(0.0f, myBody.velocity.y, 0.0f);
            }
        }
    }

    void InitialPush()
    {
        isPushing = true;

        //Work out the way the player should face
        if(transform.position.x < pushableObj.transform.position.x)
        {
            isPushingFromLeft = false;
        }
        else
        {
            isPushingFromLeft = true;
        }

        //Play the push animation while facing the right way
        //Then pause the animation since the player is not yet moving
        myAnims.PlayPush(isPushingFromLeft);
        myAnims.PausePushPull();

        pushableObj.GetComponent<Pushing>().SetParent(isPushingFromLeft);
    }

    void OnCollisionEnter(Collision col){
        if ((col.gameObject.layer == LayerMask.NameToLayer("Ground")
            || col.gameObject.layer == LayerMask.NameToLayer("Dropable")))
            {
            if (isClimbing && col.transform.position.y < (this.transform.position.y + 0.1f))
            {
                isClimbing = false;
            }

            if (!isOnGround) {
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
            ShowUI();
		}

        //the collection for throwable objects
        if(col.gameObject.tag == "Throwable")
        {
            throwableObj = col.gameObject;
            isTouchingThrowable = true;
        }

        if(col.gameObject.tag == "Pushable" && !isPushing)
        {
            pushableObj = col.gameObject;
            isTouchingPushable = true;
        }

        if(col.gameObject.layer == LayerMask.NameToLayer("Dropable"))
        {
            justAboveGround = true;
            isAbleToDrop = true;
        }

        if (col.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            justAboveGround = true;
        }
    }
    
    void OnTriggerStay(Collider col)
    {
        //the collection for throwable objects
        if (col.gameObject.tag == "Throwable")
        {
            isTouchingThrowable = true;
			if (throwableObj == null) {
				throwableObj = col.gameObject;
			}
        }

		if (col.gameObject.tag == "Ladder") {
			isTouchingLadder = true;
			if (ladderObj == null) {
				ladderObj = col.gameObject;
			}
		}

		if (col.gameObject.tag == "Door") {
			isTouchingDoor = true;
			if (doorObj == null) {
				doorObj = col.gameObject;
			}
		}
    }

	void OnTriggerExit(Collider col){
		if (col.gameObject.tag == "Ladder") {
			myBody.useGravity = true;
			isTouchingLadder = false;
			ladderObj = null;
		}

        if (col.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            justAboveGround = false;
        }

        if(col.gameObject.layer == LayerMask.NameToLayer("Dropable"))
        {
            isAbleToDrop = false;
            justAboveGround = false;
            myCurrentDropTime = 0.0f;
            if (!isClimbing)
            {
                this.GetComponent<BoxCollider>().isTrigger = false;
            }
        }

        if(col.gameObject.tag == "Throwable")
        {
            isTouchingThrowable = false;
            throwableObj = null;
        }

        if(col.gameObject.tag == "Pushable")
        {
            if (!isPushing)
            {
                isTouchingPushable = false;
                pushableObj = null;
            }
        }
	}

    void CheckPlayerDropping()
    {
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            //Update the time the player has wanted to drop for
            myCurrentDropTime += Time.deltaTime;

            //If the full required time has been hit,
            //then reset the time and drop the player
            if (myCurrentDropTime >= myFullDropTime)
            {
                myCurrentDropTime = 0.0f;
                this.GetComponent<BoxCollider>().isTrigger = true;
                myAnims.PlayFall();
            }
        }
        else
        {
            //reset the time incase the player hit S by accident
            myCurrentDropTime = 0.0f;
        }
    }
}
