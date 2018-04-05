using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WeaponControlScript : NetworkBehaviour {

	private WeaponStats stats;
	public ExplosionPhysicsForce Explosion;
	private float CurrentHealth;
	//private float LifeTime;
	// Use this for initialization
	void Start () 
	{
		stats = GetComponent<WeaponStats>();
		CurrentHealth = stats.MaxHealth;
        Explosion.explosionForce = stats.MaxExplosionForce;
        Explosion.blastRadius = stats.BlastRadius;
        Explosion.GetComponent<ParticleSystemMultiplier>().multiplier = stats.BlastRadius / 1000;
		//LifeTime = stats.LifeSpan;

        if(isServer)
        {
            Destroy(gameObject, stats.LifeSpan);


        }
       
        
	}
	
	// Update is called once per frame
	void Update () 
	{
		//LifeTime -= Time.deltaTime;

  //      if (LifeTime <= 0 || CurrentHealth <= 0)
  //      {
  //          SplashDamage();
  //          Explode();
  //      }

	}
    
    void SplashDamage()
    {
        float r = stats.BlastRadius;
        var cols = Physics.OverlapSphere(transform.position, r);
        //var objects = new List<GameObject>();
        foreach (var col in cols)
        {
            var hit = col.gameObject;
            var ShipStats = hit.GetComponent<ShipStats>();
            if (ShipStats != null)
            {
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                float splashDamage = stats.Damage * (stats.BlastRadius - distance) / stats.BlastRadius;
                splashDamage = Mathf.Max(0, splashDamage);
                bool wasKilled = ShipStats.TakeDamage(splashDamage);  /* **** this is a problem to be called on the client */
                if (wasKilled)
                {
                    stats.owner.ScoreKill();
                    //Score a Kill for the weapon's owner
                }
                Rigidbody rig = hit.GetComponent<Rigidbody>();
                if (rig != null)
                    rig.AddExplosionForce(stats.MaxExplosionForce, transform.position, stats.BlastRadius, 0, ForceMode.Impulse);
            }
            
        }
    }
    void Explode()
    {
       
        Explosion.transform.parent = null;
        Explosion.gameObject.SetActive(true);
        ParticleSystem ps = Explosion.GetComponentInChildren<ParticleSystem>();
        if (ps != null)
            Destroy(Explosion.gameObject, ps.main.duration);
        else
            Destroy(Explosion.gameObject, 5);

        SplashDamage();
    }

    [ServerCallback]
	void OnCollisionEnter(Collision collision)
	{
		var hit = collision.gameObject;
        if (stats.owner == hit.GetComponent<PlayerControlScript>())
            return;
		var ShipStats = hit.GetComponent<ShipStats>();
		if(ShipStats != null)
		{
            bool wasKilled = ShipStats.TakeDamage(stats.Damage);
            if (wasKilled)
            {
                //hit.GetComponent<PlayerControlScript>().ScoreKill(stats.ownerId);
                //Score a Kill for the weapon's owner
                Debug.Log("Players in the game: " + GameManager.Players.Count.ToString() + " " + stats.owner.netId);
                //GameManager.Players[stats.ownerId].ScoreKill();
                stats.owner.ScoreKill();
            }
        }

        //if (!NetworkClient.active)
        //    SplashDamage();
        
		//Explode();

        NetworkServer.Destroy(gameObject);
	}

    public override void OnNetworkDestroy()
    {
        Explode();
        base.OnNetworkDestroy();

        /*if(!GameManager.isShuttingDown)
		{
			GameObject explosion = Instantiate(Explosion, transform.position, transform.rotation);
			Destroy(explosion, 3f);
		}*/
        //GameManager.WeaponList.Remove(gameObject);
    }
}
