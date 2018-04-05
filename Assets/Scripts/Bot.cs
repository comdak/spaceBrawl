using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bot : NetworkBehaviour {

    public bool botCanShoot = true;
    [SerializeField] float shotCoolDown = 1f;
    private PlayerControlScript playerControl;

    float ellapsedTime = 0f;

    private void Awake()
    {
        playerControl = GetComponent<PlayerControlScript>();
        playerControl.playerName = "Bot";
    }

    [ServerCallback]
    private void Update()
    {
        //BotAutoFire();
    }

    [Server]
    private void BotAutoFire()
    {
        ellapsedTime += Time.deltaTime;
        if (ellapsedTime < shotCoolDown)
            return;

        ellapsedTime = 0f;

        if (botCanShoot)
            playerControl.FireAsBot();
    }
}
