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

    //For holding the camera's room position
    public ZoneCamera myZoneCam;

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
    bool isOnGround, justAboveGround;

    //For jumping checks
    bool isPreJumping;
    float fallingSpeed = 0.0f;
    
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

        GroundCheck();

        //While the player is being controlled
		if(inControl)
        {
            CheckInputs();
        }

        //Update the UI timer when it is being shown
        if(showingUI)
        {
            UpdateUI();
        }

        //Let the player leave the game
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
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

    //General input taking
    void CheckInputs()
    {
        DirectionalMovement();
        Jump();
      
    }

    void GroundCheck()
    {
        if(!isOnGround)
        {

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
                isPreJumping = true;
            }
        }
        else if (!isPreJumping)
        {
            myAnims.PlayFall();
            fallingSpeed += Time.deltaTime;
            myBody.velocity = new Vector3(myBody.velocity.x, myBody.velocity.y - fallingSpeed, 0);
        }
        else if (isPreJumping)
        {
            if(myBody.velocity.y <= 0.0f)
            {
                isPreJumping = false;
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
                myAudio.Footsteps();
                Move(false);
            }
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                Move(true);
                myAudio.Footsteps();
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
                if (isOnGround)
                {
                    myAnims.PlayIdle();
                }
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
            }
        }
        else {
            myBody.velocity = new Vector3(-moveSpeed, myBody.velocity.y, myBody.velocity.z);
            if (isOnGround)
            {
                myAnims.PlayWalk(true);
            }
        }
    }

    void OnCollisionEnter(Collision col)
    {
        //If you collide with the ground
        if ((col.gameObject.layer == LayerMask.NameToLayer("Ground")
            || col.gameObject.layer == LayerMask.NameToLayer("Dropable")))
        {
            //if (isClimbing && col.transform.position.y < (this.transform.position.y + 0.1f))
            //{
            //    isClimbing = false;
            //}

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
    }

}
