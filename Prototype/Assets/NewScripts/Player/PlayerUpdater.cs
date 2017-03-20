using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUpdater : MonoBehaviour {

    //Slowly being updated

    //For holding pointer information
    Rigidbody myBody;
    AnimationManager myAnims;
    AudioManager myAudio;

    //if the character is in control
    public bool inControl;

    //shards
    bool showingUI;
    int shardsCollected;
    public Text txtShardsCollected;
    public Image imageShards;
    float displayTimer;
    public float UIDisplayTime;

    //moving
    public float moveSpeed, runSpeed, climbingSpeed;

    //Ground checks
    bool isOnGround, isAbleToDrop;

    //drop platforms
    public float myFullDropTime;
    float myCurrentDropTime;

    //For jumping checks
    bool isGoingUp;
    float fallingSpeed = 0.0f;

    //For holding all interactions booleans
    bool isTouchingPushable, isPushing, isPushingFromLeft;
    
    //For holding all gameobjects
    GameObject pushableObj;
 
    //for culling abilities
    int cullNum = -1;
    bool cullPush, cullPick, cullClimb;

    // Use this for initialization
    void Start () {
        //Create pointers to the needed scripts and objects
        myBody = GetComponent<Rigidbody>();
        myAnims = this.GetComponent<AnimationManager>();
        myAudio = this.GetComponent<AudioManager>();
        isOnGround = true;
    }
	
	// Update is called once per frame
	void Update () {

        //Check for any updates whether the player is controlled, or not
        NonMovementUpdates();

        //While the player is being controlled
		if(inControl)
        {
            CheckInputs();
        }

    }

    void NonMovementUpdates()
    {
        //Check if the player is on the ground
        GroundCheck();

        //Update the UI timer when it is being shown
        if (showingUI)
        {
            UpdateUI();
        }

        //Let the player leave the game
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void CullNext()
    {
        cullNum++;

        switch (cullNum)
        {
            case 0:
                cullPush = true;
                break;
            case 1:
                cullClimb = true;
                break;
            case 2:
                cullPick = true;
                break;
            default:
                break;
        }
    }

    //General input taking
    void CheckInputs()
    {
        if (!Interacting())
        {
            DirectionalMovement();
            Jump();

            if (isAbleToDrop)// && !isClimbing)
            {
                CheckPlayerDropping();
            }

            CanInteract();
        }
        else
        {
            if (isPushing)
            {
                PushMovement();
            }
        }
    }

    bool Interacting()
    {
        if(isPushing)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void CanInteract()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            //If the push/pull ability is still in use
            //if (!cullPush)
            //{
            //    if (isTouchingDoor)
            //    {
            //        if (!doorObj.GetComponent<Door>().isOpen)
            //        {
            //            OpenDoor();
            //        }
            //    }
            //}

            if (isTouchingPushable && isOnGround)
            {
                InitialPush();
            }
        }
    }

    void GroundCheck()
    {
        if ((myBody.velocity.y >= -0.001f && myBody.velocity.y <= 0.001f) & !isGoingUp)
        {
            isOnGround = true;
        }
        else
        { 
            isOnGround = false;
        }

        if(!isOnGround && !isGoingUp && myBody.velocity.y > 0)
        {
            myAnims.PlayJump();
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

    void Jump()
    {
        if (isOnGround)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown((KeyCode.UpArrow)))
            {
                myAnims.PlayJump();
                myBody.velocity = new Vector3(myBody.velocity.x, 15, 0);
                isOnGround = false;
                isGoingUp = true;
            }
        }
        else if (!isGoingUp)
        {
            myAnims.PlayFall();
            fallingSpeed += Time.deltaTime;
            myBody.velocity = new Vector3(myBody.velocity.x, myBody.velocity.y - fallingSpeed, 0);
        }
        else if (isGoingUp)
        {
            if(myBody.velocity.y <= -0.001f)
            {
                isGoingUp = false;
                myAnims.PlayFall();
            }
            else
            {
                myBody.velocity = new Vector3(myBody.velocity.x, myBody.velocity.y - Time.deltaTime, 0);
            }
        }
    }

    //For general movement
    void DirectionalMovement()
    {
        //move left and right, right always takes prio
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                Move(false);
            }
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                Move(true);
            }
            else {
                myBody.velocity = new Vector3(0, myBody.velocity.y, myBody.velocity.z);
                if (isOnGround)
                {
                    myAnims.PlayIdle();
                }
            }
        }
        else if(isOnGround)
        {
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                Run(false);
            }
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                Run(true);
            }
            else {
                myBody.velocity = new Vector3(0, myBody.velocity.y, myBody.velocity.z);
                myAnims.PlayIdle();
            }
        }
    }

    void Run(bool moveDir)
    {
        //move right
        if (!moveDir)
        {
            myBody.velocity = new Vector3(runSpeed, myBody.velocity.y, myBody.velocity.z);
            myAnims.PlayRun(false);
        }
        else {
            myBody.velocity = new Vector3(-runSpeed, myBody.velocity.y, myBody.velocity.z);
            myAnims.PlayRun(true);
        }
    }

    void Move(bool moveDir)
    {
        //move right
        if (!moveDir)
        {
            myBody.velocity = new Vector3(moveSpeed, myBody.velocity.y, myBody.velocity.z);
            if (isOnGround)
            {
                myAnims.PlayWalk(false);
                myAudio.Footsteps();
            }
        }
        else {
            myBody.velocity = new Vector3(-moveSpeed, myBody.velocity.y, myBody.velocity.z);
            if (isOnGround)
            {
                myAnims.PlayWalk(true);
                myAudio.Footsteps();
            }
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

        //Stop pushing the object under the below conditions
        if (Input.GetKeyDown(KeyCode.E) || transform.position.y >= pushableObj.transform.position.y + 5.0f
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

    void InitialPush()
    {
        isPushing = true;

        //Work out the way the player should face
        if (transform.position.x < pushableObj.transform.position.x)
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

    //UI functions
    void UpdateUI()
    {
        if (displayTimer > 0.0f)
        {
            displayTimer -= Time.deltaTime;
        }
        else
        {
            txtShardsCollected.enabled = false;
            imageShards.enabled = false;
            showingUI = false;
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
        showingUI = true;
    }

    void OnCollisionEnter(Collision col)
    {
        //If you collide with the ground
        if ((col.gameObject.layer == LayerMask.NameToLayer("Ground")
            || col.gameObject.layer == LayerMask.NameToLayer("Dropable")))
        {
            if (!isOnGround)
            {
                isOnGround = true;
                fallingSpeed = 0.0f;
                //set to idle animation
                myAnims.PlayIdle();
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Collectable")
        {
            myAudio.CrystalCollect();
            Destroy(col.gameObject);
            shardsCollected++;
            ShowUI();
        }

        if (col.gameObject.tag == "Pushable" && !isPushing)
        {
            pushableObj = col.gameObject;
            isTouchingPushable = true;
        }

        if (col.gameObject.layer == LayerMask.NameToLayer("Dropable"))
        {
            isOnGround = true;
            isAbleToDrop = true;
        }

        if (col.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isOnGround = true;
        }
    }

    void OnTriggerExit(Collider col)
    {
         if (col.gameObject.layer == LayerMask.NameToLayer("Dropable"))
        {
            isAbleToDrop = false;
            myCurrentDropTime = 0.0f;
           //if (!isClimbing)
           //{
              this.GetComponent<BoxCollider>().isTrigger = false;
           //}
        }

        if (col.gameObject.tag == "Pushable")
        {
            if (!isPushing)
            {
                isTouchingPushable = false;
                pushableObj = null;
            }
        }
    }
}
