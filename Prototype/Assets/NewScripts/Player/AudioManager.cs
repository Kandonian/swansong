using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {

    //Audio Source on the camera for non walk sounds
    public AudioSource CameraAudio;

    //All audio clips to be used
    public AudioClip Collectable;
    public AudioClip Door;
    public AudioClip Walk;
	
    //For when the player comes in contact with a crystal
	public void CrystalCollect()
    {
        CameraAudio.clip = Collectable;
        PlayCameraSound();
    }
    
    //When the player is opening a door
    public void DoorOpen()
    {
        CameraAudio.clip = Door;
        PlayCameraSound();
    }

    //Playing the walking sound
    public void Footsteps()
    {
        if (!WalkingPlaying())
        {
            GetComponent<AudioSource>().clip = Walk;
            PlaySound();
        }
    }

    //Stops the audio being played when called
    public void StopAudio()
    {
        GetComponent<AudioSource>().Stop();
    }

    //Check if the walking sound is already playing
    bool WalkingPlaying()
    {
        //If the clip is playing, and that clip is the walk sound effect
        if (GetComponent<AudioSource>().clip == Walk && GetComponent<AudioSource>().isPlaying)
        {
            return true;
        }

        return false;
    }

    //Playing the appropriate sound on the Player
    void PlaySound()
    {
        GetComponent<AudioSource>().Play();
    }

    void PlayCameraSound()
    {
        CameraAudio.Play();
    }
}
