using UnityEngine;
using System.Collections;

public class IntroSmokey : MonoBehaviour {

	public Transform[] movePositions;

	public float moveSpeed;
	float moveTimer;
	public float waitSpeed;
	float waitTimer;

	public float smoothTime = 0.3f;
	private Vector3 velocity = Vector3.zero;

	public int curStage;

	public PlayerControl myPlayer;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (curStage == 0) {
			Vector3 targetPosition = movePositions [curStage].TransformPoint (new Vector3 (0, 0, 0));
			transform.position = Vector3.SmoothDamp (transform.position, targetPosition, ref velocity, smoothTime);
			waitTimer += Time.deltaTime;
			if (waitTimer > 2f) {
				curStage = 1;
				waitTimer = 0;
			}
		} else if (curStage == 1) {
			Vector3 targetPosition = movePositions [curStage].TransformPoint (new Vector3 (0, 0, 0));
			transform.position = Vector3.SmoothDamp (transform.position, targetPosition, ref velocity, smoothTime);
			waitTimer += Time.deltaTime;
			if(waitTimer > 1f){
				myPlayer.inControl = true;
			}
		}
	}
}
