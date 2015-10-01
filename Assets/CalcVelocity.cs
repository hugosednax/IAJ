using UnityEngine;
using System.Collections;

public class CalcVelocity : MonoBehaviour {
	Vector3 previousPos;
	Vector3 velocity = new Vector3(0f, 0f, 0f);

	// Use this for initialization
	void Start () {
		previousPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		velocity = (transform.position - previousPos) / Time.deltaTime;
		previousPos = transform.position;
	}

	public Vector3 getVelocity(){
		return velocity;
	}
}
