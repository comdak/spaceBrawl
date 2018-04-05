using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WeaponStats : MonoBehaviour {

//	[SerializeField]
	public WeaponType Type;
    public PlayerControlScript owner;
	public float MaxHealth = 1;
	public float InitialForce = 1;
	public float ReloadTime = 1;
	public float Damage = 10;
	public float BlastRadius = 100;
	public float MaxExplosionForce = 1000;
	public float LifeSpan = 2;

    private void Start()
    {
 //       Debug.Log(GetComponent<Rigidbody>().velocity.ToString());
    }
}
