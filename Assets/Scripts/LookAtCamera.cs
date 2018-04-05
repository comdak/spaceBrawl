using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour {

    Transform mainCamera;

	// Use this for initialization
	void Start () {
        mainCamera = Camera.main.transform;
	}

    private void LateUpdate()
    {
        if (mainCamera == null)
            return;

        transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.position,Vector3.forward);
    }
}
