using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


public class ShipStats : NetworkBehaviour {

	public const float MaxHealth = 100f;
    public bool destroyOnDeath;
	[SyncVar(hook = "OnHealthChange")] float currentHealth;

    public RectTransform LocalHealthBar;
    public RectTransform RemoteHealthBar;
    public RectTransform healthBar;

    //AI Stuff
    public float lookSphereCastRadius = 1f;
    public float lookRange = 80f;
    public float attackRange = 40f;
    public float attackRate = 1f;
    public float searchingTurnSpeed = 60f;
    public float searchDuration = 15f;
    //private NetworkStartPosition[] spawnPoints;

    private PlayerControlScript playerScript;

    private void Awake()
    {
        playerScript = GetComponent<PlayerControlScript>();
    }
    [ServerCallback]
    private void Start()
    {
        currentHealth = MaxHealth;
    }

    [ServerCallback]
    private void OnEnable()
    {
        currentHealth = MaxHealth;
    }
    
    [Server]
    public bool TakeDamage(float amount)
	{
        //if (!isServer)
        //    return;
        bool died = false;
        
        if (currentHealth <= 0)
            return died;

        Debug.Log("Take " + amount + " Damage");

		currentHealth -= amount;

        died = currentHealth <= 0;

        RpcTakeDamage(currentHealth, died);

        return died;
    		
	}

    [ClientRpc]
    void RpcTakeDamage(float value, bool died)
    {
        currentHealth = value;
        if (died)
            playerScript.Die();

    }
    

    void OnHealthChange(float health)
    {
        currentHealth = health;
        
        if(isLocalPlayer)
        {
            Debug.Log("Change Health: " + health + ", " + isLocalPlayer);
            //LocalHealthBar.Find("Foreground").GetComponent<RectTransform>().sizeDelta = new Vector2(health / MaxHealth * LocalHealthBar.sizeDelta.x, LocalHealthBar.sizeDelta.y);
            PlayerCanvas.canvas.SetHealth(health, MaxHealth);
        }
        else
            RemoteHealthBar.Find("Foreground").GetComponent<RectTransform>().sizeDelta = new Vector2(health / MaxHealth * RemoteHealthBar.sizeDelta.x, RemoteHealthBar.sizeDelta.y);
        //healthBar.Find("Foreground").GetComponent<RectTransform>().sizeDelta = new Vector2(health / MaxHealth * healthBar.sizeDelta.x, healthBar.sizeDelta.y);
    }

	
}
