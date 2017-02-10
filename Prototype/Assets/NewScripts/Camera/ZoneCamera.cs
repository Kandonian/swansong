using UnityEngine;
using System.Collections;

public class ZoneCamera : MonoBehaviour {

	public Transform[] roomPositions;
	public int currentRoom;

	public float smoothTime = 0.3f;
	private Vector3 velocity = Vector3.zero;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		CamMove ();
	}

	void CamMove(){
		Vector3 targetPosition = roomPositions[currentRoom].TransformPoint(new Vector3(0, 0, 0));
		transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
	}
}
