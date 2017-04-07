using UnityEngine;
using System.Collections;

public class AnimationManager : MonoBehaviour {

	//Used for animations
	Animation myAnimation;
	bool facingLeft, climbing;

	// Use this for initialization
	void Start () {
		myAnimation = gameObject.GetComponent<Animation>();

		//Set all appropriate animations to loop and blend
		myAnimation["Walk"].wrapMode = WrapMode.Loop;
		myAnimation["Walk"].blendMode = AnimationBlendMode.Blend;

		myAnimation["Idle"].wrapMode = WrapMode.Loop;
		myAnimation["Idle"].blendMode = AnimationBlendMode.Blend;

		myAnimation["Run"].wrapMode = WrapMode.Loop;
		myAnimation["Run"].blendMode = AnimationBlendMode.Blend;

		myAnimation["Jump"].blendMode = AnimationBlendMode.Blend;

		myAnimation["Fall"].blendMode = AnimationBlendMode.Blend;
		myAnimation["Fall"].speed = 0.3f;

		myAnimation["Push"].wrapMode = WrapMode.Loop;
		myAnimation["Push"].blendMode = AnimationBlendMode.Blend;

		myAnimation["Pull"].wrapMode = WrapMode.Loop;
		myAnimation["Pull"].blendMode = AnimationBlendMode.Blend;

		myAnimation["Ladder"].wrapMode = WrapMode.Loop;
		myAnimation["Ladder"].blendMode = AnimationBlendMode.Blend;

        myAnimation["Pick Up"].blendMode = AnimationBlendMode.Blend;

        myAnimation["Throw"].blendMode = AnimationBlendMode.Blend;
    }

	// Play the idle animation
	public void PlayIdle()
	{
		myAnimation.Play("Idle");
	}

	public void PlayWalk(bool goingLeft)
	{
		ChoosingDirection(goingLeft);
		myAnimation.Play("Walk");
	}

	public void PlayRun(bool goingLeft)
	{
		ChoosingDirection(goingLeft);
		myAnimation.Play("Run");
	}

	public void PlayPull(bool goingLeft)
	{
		ChoosingDirection(goingLeft);
        myAnimation["Pull"].speed = 1.0f;
        myAnimation.Play("Pull");
	}

	public void PlayPush(bool goingLeft)
	{
		ChoosingDirection(goingLeft);
        myAnimation["Push"].speed = 1.0f;
        myAnimation.Play("Push");
	}

	public void PlayJump()
	{
		myAnimation ["Jump"].normalizedSpeed = 6f;
		myAnimation.Play("Jump");
	}

	public bool JumpingUp()
	{
		if (myAnimation["Jump"].normalizedTime >= 0.8f)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

    public bool FinishedPickingUp()
    {
        if (myAnimation["Pick Up"].normalizedTime >= 0.8f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

	public void PlayFall()
	{
		myAnimation.Play("Fall");
	}

    public void PlayClimbingUp()
    {
        if(!climbing)
        {
            climbing = true;
            if(facingLeft)
            {
                this.transform.Rotate(0, 90, 0);
            }
            else
            {
                this.transform.Rotate(0, -90, 0);
            }
        }

        //Play the animation at normal speed
        myAnimation["Ladder"].speed = 1.0f;
        myAnimation.Play("Ladder");
    }

    public void PlayClimbingDown()
    {
        if (!climbing)
        {
            climbing = true;
            if (facingLeft)
            {
                this.transform.Rotate(0, 90, 0);
            }
            else
            {
                this.transform.Rotate(0, -90, 0);
            }
        }

        //Play the animation at the correct speed
        myAnimation["Ladder"].speed = -1.0f;
        myAnimation.Play("Ladder");
    }

    public void PickUpObject()
    {
        myAnimation.Play("Pick Up");
    }

    public bool ThrowReady()
    {
        if (myAnimation["Throw"].normalizedTime >= 0.4f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool ThrowAnimationFinished()
    {
        if (myAnimation["Throw"].normalizedTime >= 0.9f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ThrowObject()
    {
        myAnimation.Play("Throw");
    }

    //Pause both the climbing animations
    public void PauseClimbingAnimations()
    {
        myAnimation["Ladder"].speed = 0.0f;
        myAnimation["Ladder"].speed = 0.0f;
    }

    public void PausePushPull()
    {
        myAnimation["Push"].speed = 0.0f;
        myAnimation["Pull"].speed = 0.0f;
    }

	void ChoosingDirection(bool goingLeft)
	{
		if (goingLeft)
		{
			if (!facingLeft)
			{
				facingLeft = true;
				this.transform.Rotate(0.0f, 180.0f, 0.0f);
			}
		}
		else
		{
			if(facingLeft)
			{
				facingLeft = false;
				this.transform.Rotate(0.0f, 180.0f, 0.0f);
			}
		}
	}

    public void PausePushPullAfter()
    {
        if(myAnimation.IsPlaying("Push"))
        {
            //Round to 1 d.p. in order to more easily check the animation is divisible by 1 roughly
            float nonDecTime = (float)System.Math.Round(myAnimation["Push"].normalizedTime, 1);
            
            if(nonDecTime % 1 == 0)
            {
                PausePushPull();
            }
        }
        else if(myAnimation.IsPlaying("Pull"))
        {
            float nonDecTime = (float)System.Math.Round(myAnimation["Pull"].normalizedTime, 1);

            if (nonDecTime % 1 == 0)
            {
                PausePushPull();
            }
        }
    }

    public bool FacingLeft()
    {
        return facingLeft;
    }

    public void AfterClimb()
    {
        climbing = false;

        if (!facingLeft)
        {
            facingLeft = true;
            this.transform.Rotate(0.0f, -90.0f, 0.0f);
        }
        else if (facingLeft)
        {
            this.transform.Rotate(0.0f, -90.0f, 0.0f);
        }
    }

}
