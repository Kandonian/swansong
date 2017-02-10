using UnityEngine;
using System.Collections;

public class AnimationManager : MonoBehaviour {

	//Used for animations
	Animation myAnimation;
	bool facingLeft;

	// Use this for initialization
	void Start () {
		myAnimation = gameObject.GetComponent<Animation>();

		//Set all appropriate animations to loop and blend
		myAnimation["walk"].wrapMode = WrapMode.Loop;
		myAnimation["walk"].blendMode = AnimationBlendMode.Blend;

		myAnimation["idle"].wrapMode = WrapMode.Loop;
		myAnimation["idle"].blendMode = AnimationBlendMode.Blend;

		myAnimation["run"].wrapMode = WrapMode.Loop;
		myAnimation["run"].blendMode = AnimationBlendMode.Blend;

		myAnimation["Jump up"].blendMode = AnimationBlendMode.Blend;

		myAnimation["Fall / Drop"].blendMode = AnimationBlendMode.Blend;
		myAnimation["Fall / Drop"].speed = 0.3f;

		myAnimation["push"].wrapMode = WrapMode.Loop;
		myAnimation["push"].blendMode = AnimationBlendMode.Blend;

		myAnimation["pull"].wrapMode = WrapMode.Loop;
		myAnimation["pull"].blendMode = AnimationBlendMode.Blend;

		myAnimation["Up ladder"].wrapMode = WrapMode.Loop;
		myAnimation["Up ladder"].blendMode = AnimationBlendMode.Blend;

		myAnimation["down ladder"].wrapMode = WrapMode.Loop;
		myAnimation["down ladder"].blendMode = AnimationBlendMode.Blend;

	}

	// Play the idle animation
	public void PlayIdle()
	{
		myAnimation.Play("idle");
	}

	public void PlayWalk(bool goingLeft)
	{
		ChoosingDirection(goingLeft);
		myAnimation.Play("walk");
	}

	public void PlayRun(bool goingLeft)
	{
		ChoosingDirection(goingLeft);
		myAnimation.Play("run");
	}

	public void PlayPull(bool goingLeft)
	{
		ChoosingDirection(goingLeft);
		myAnimation.Play("pull");
	}

	public void PlayPush(bool goingLeft)
	{
		ChoosingDirection(goingLeft);
		myAnimation.Play("push");
	}

	public void PlayJump()
	{
		myAnimation ["Jump up"].normalizedSpeed = 6f;
		myAnimation.Play("Jump up");
	}

	public bool JumpingUp()
	{
		if (myAnimation["Jump up"].normalizedTime >= 0.8f)
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
		myAnimation.Play("Fall / Drop");
	}

    public void PlayClimbingUp()
    {
        //Make sure the player faces the ladder
        if(!facingLeft)
        {
            facingLeft = true;
            this.transform.Rotate(0.0f, 180.0f, 0.0f);
        }

        //Play the animation at normal speed
        myAnimation["Up ladder"].speed = 1.0f;
        myAnimation.Play("Up ladder");
    }

    public void PlayClimbingDown()
    {
        //Make sure the player faces the ladder
        if (!facingLeft)
        {
            facingLeft = true;
            this.transform.Rotate(0.0f, 180.0f, 0.0f);
        }
        
        //Play the animation at the correct speed
        myAnimation["down ladder"].speed = 1.0f;
        myAnimation.Play("down ladder");
    }

    //Pause both the climbing animations
    public void PauseClimbingAnimations()
    {
        myAnimation["Up ladder"].speed = 0.0f;
        myAnimation["down ladder"].speed = 0.0f;
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

    public bool FacingLeft()
    {
        return facingLeft;
    }

}
