using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControlScript : MonoBehaviour {

	private Transform localPlayerTransform;
	private Rigidbody localPlayerRB;
	public float ForwardCameraOffset = 0;
	// Use this for initialization
	void Start () 
	{
		localPlayerTransform = gameObject.GetComponentInParent<Transform>();
		localPlayerRB = gameObject.GetComponentInParent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.LookAt(localPlayerTransform.position + localPlayerRB.velocity.normalized*ForwardCameraOffset, Vector3.forward);
		transform.position = new Vector3(localPlayerTransform.position.x, transform.position.y, localPlayerTransform.position.z);
	}
}
