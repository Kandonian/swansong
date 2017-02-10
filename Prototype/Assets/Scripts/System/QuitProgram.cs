using UnityEngine;
using System.Collections;

public class QuitProgram : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        //If the player wants to close the window
	    if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
	}
}
