using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {

	public bool isOpen;
	public bool openDir;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (isOpen) {
			Open ();
		}
	}

	void Open(){
		if (openDir) {
			Vector3 destinationAngle = new Vector3(0,210,0);
			transform.eulerAngles = Vector3.Lerp(transform.rotation.eulerAngles, 
			                                     destinationAngle, 
			                                     Time.deltaTime);
		} else {
			Vector3 destinationAngle = new Vector3(0,210,0);
			transform.eulerAngles = Vector3.Lerp(transform.rotation.eulerAngles, 
			                                     destinationAngle, 
			                                     Time.deltaTime);
		}
	}
}
