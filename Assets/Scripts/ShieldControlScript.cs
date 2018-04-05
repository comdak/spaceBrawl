using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldControlScript : MonoBehaviour {

	private WeaponStats stats;
	private float currentLifeTime = 0;

	void Awake()
	{
		stats = GetComponent<WeaponStats>();
	}
	void OnEnable()
	{
		currentLifeTime = stats.LifeSpan;
	}

	void Update()
	{
		currentLifeTime -= Time.deltaTime;
		Debug.Log("Shield Current lifespan: " + currentLifeTime.ToString());
		if(currentLifeTime <= 0)
			gameObject.SetActive(false);
	}

	void OnTriggerEnter(Collider other) {
        Destroy(other.gameObject);
    }

}
