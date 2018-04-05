using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using UnityEngine.UI;
using System;

[System.Serializable]
public class ToggleEvent : UnityEvent<bool> { }
public class PlayerControlScript : NetworkBehaviour
{

    public float cameraDamping = 1;
    public GameObject WeaponObject1;
    public GameObject WeaponObject2;
    public GameObject WeaponObject3;

    [SerializeField] ToggleEvent OnToggleShared;
    [SerializeField] ToggleEvent OnToggleLocal;
    [SerializeField] ToggleEvent OnToggleRemote;
    private Dictionary<WeaponType, Vector2> ReloadTime;
    private bool[] WeaponToFire = { false, false, false };
    //private float[] CurrentReloadTime = {0f,0f,0f};
    public float RotationAuth = 2.0f;
    public float ThrustAuth = 2.0f;

    [SyncVar(hook = "OnNameChanged")] public string playerName;
    [SyncVar(hook = "OnColorChanged")] public Color playerColor;
    [SyncVar(hook = "OnScoreChange")] public int score = 0;
   
    //public RectTransform[] ReloadBar = new RectTransform[3];
    // public RectTransform SecondaryReloadBar;
    // public RectTransform ShieldRechargeBar;

    public Transform WeaponLaunchPoint;

    //static Dictionary<NetworkInstanceId, PlayerControlScript> players = new Dictionary<NetworkInstanceId, PlayerControlScript>();  //Server list of players

    private float vertValue = 0;
    private float horizValue = 0;
    private float strafeValue = 0;

    public MeshRenderer localRadarPos;

    public float respawnTime = 5f;

    private Camera playerCamera;

    private Rigidbody RB;
    // Use this for initialization
    void Awake()
    {
        
    }

    void Start()
    {
        RB = GetComponent<Rigidbody>();
        ReloadTime = new Dictionary<WeaponType, Vector2>();
        if (WeaponObject1 != null)
            ReloadTime.Add(WeaponObject1.GetComponent<WeaponStats>().Type, new Vector2(0, WeaponObject1.GetComponent<WeaponStats>().ReloadTime));
        if (WeaponObject2 != null)
            ReloadTime.Add(WeaponObject2.GetComponent<WeaponStats>().Type, new Vector2(0, WeaponObject2.GetComponent<WeaponStats>().ReloadTime));
        if (WeaponObject3 != null)
            ReloadTime.Add(WeaponObject3.GetComponent<WeaponStats>().Type, new Vector2(0, WeaponObject3.GetComponent<WeaponStats>().ReloadTime));

        Debug.Log("Current Reload Time Length: " + ReloadTime.Count);

        playerCamera = Camera.main;

        

        EnablePlayer();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        OnNameChanged(playerName);
        OnColorChanged(playerColor);

    }

    [ServerCallback]
    private void OnEnable()
    {
        if (!GameManager.Players.Contains(this))
            GameManager.Players.Add(this);
    }

    [ServerCallback]
    private void OnDisable()
    {
        if (GameManager.Players.Contains(this))
            GameManager.Players.Remove(this);
    }

    void EnablePlayer()
    {
        OnToggleShared.Invoke(true);

        if (isLocalPlayer)
        {
            //if (playerCamera != null)
            //    playerCamera.GetComponentInChildren<Canvas>().gameObject.SetActive(true);
            PlayerCanvas.canvas.Initialize();

            OnToggleLocal.Invoke(true);
        }
        else
            OnToggleRemote.Invoke(true);
    }

    void DisablePlayer()
    {
        OnToggleShared.Invoke(false);

        if (isLocalPlayer)
        {
            //if (playerCamera != null)
            //    playerCamera.GetComponentInChildren<Canvas>().gameObject.SetActive(false);
            OnToggleLocal.Invoke(false);
        }
        else
            OnToggleRemote.Invoke(false);
    }

    public void Die()
    {

        if (isLocalPlayer)
        {
            PlayerCanvas.canvas.SetGameStatusText("You Died!");
            Debug.Log("You Died!");
        }

        DisablePlayer();

        Invoke("Respawn", respawnTime);
    }



    void Respawn()
    {
        if (isLocalPlayer)
        {
            Transform spawn = NetworkManager.singleton.GetStartPosition();
            transform.position = spawn.position;
            transform.rotation = spawn.rotation;
            RB.velocity = Vector3.zero;
            RB.angularVelocity = Vector3.zero;
        }

        EnablePlayer();

    }

    // Update is called once per frame
    void Update()
    {
        
        if (!isLocalPlayer)
            return;

        for (int i = 0; i < ReloadTime.Count; i++)
        {
            Vector2 reloadPair = ReloadTime[ReloadTime.Keys.ElementAt(i)];
            if (reloadPair.x == 0)
                continue;
            else if (reloadPair.x < 0)
            {
                ReloadTime[ReloadTime.Keys.ElementAt(i)] = new Vector2(0, reloadPair.y);
                PlayerCanvas.canvas.SetStatusBar(i, ReloadTime[ReloadTime.Keys.ElementAt(i)].x, ReloadTime[ReloadTime.Keys.ElementAt(i)].y);
            }
            else if (reloadPair.x > 0)
            {

                ReloadTime[ReloadTime.Keys.ElementAt(i)] = new Vector2(reloadPair.x - Time.deltaTime,
                    reloadPair.y);
                PlayerCanvas.canvas.SetStatusBar(i, ReloadTime[ReloadTime.Keys.ElementAt(i)].x, ReloadTime[ReloadTime.Keys.ElementAt(i)].y);
            }
            //RectTransform Foreground = ReloadBar[i].Find("Foreground").GetComponent<RectTransform>();
            //Foreground.sizeDelta = new Vector2(Foreground.sizeDelta.x, (reloadPair.y - reloadPair.x) / reloadPair.y * ReloadBar[i].sizeDelta.y);
            
        }



        vertValue += Input.GetAxis("Vertical");
        horizValue += Input.GetAxis("Horizontal");


        if (Input.GetButton("Fire1") && ReloadTime[ReloadTime.Keys.ElementAt(0)].x <= 0)
        {
            WeaponToFire[0] = true;
        }
        if (Input.GetButton("Fire2") && ReloadTime[ReloadTime.Keys.ElementAt(1)].x <= 0)
        {
            WeaponToFire[1] = true;
        }
        if (Input.GetButton("Fire3") && ReloadTime[ReloadTime.Keys.ElementAt(2)].x <= 0)
        {
            WeaponToFire[2] = true;

        }

        if(Input.GetKey(KeyCode.L))
        {
            GetComponent<ShipStats>().TakeDamage(10);
        }

        //camera.transform.LookAt(transform.position, Vector3.forward);
        //camera.transform.position = new Vector3(transform.position.x, camera.transform.position.y, transform.position.z);

    }

    void LateUpdate()
    {
        if (!isLocalPlayer)
            return;

        Vector3 desiredPos = new Vector3(transform.position.x, playerCamera.transform.position.y, transform.position.z);
        playerCamera.transform.position = Vector3.Lerp(playerCamera.transform.position, desiredPos, Time.deltaTime * cameraDamping);
        //var TargetRotation = Quaternion.LookRotation(transform.position - camera.transform.position);
        //camera.transform.rotation = Quaternion.Slerp(camera.transform.rotation, TargetRotation, Time.deltaTime * cameraDamping);
        playerCamera.transform.LookAt(transform.position, Vector3.forward);

    }

    void FixedUpdate()
    {
        if (!isLocalPlayer)
            return;

        for (int i = 0; i < WeaponToFire.Length; i++)
        {
            if (WeaponToFire[i])
            {
                ReloadTime[ReloadTime.Keys.ElementAt(i)] = new Vector2(ReloadTime.Values.ElementAt(i).y, ReloadTime.Values.ElementAt(i).y);
                CmdFireWeapon(ReloadTime.Keys.ElementAt(i));
                WeaponToFire[i] = false;
            }

        }
        RB.AddForce(vertValue * ThrustAuth * transform.forward, ForceMode.Force);
        RB.MoveRotation(Quaternion.Euler(0, transform.eulerAngles.y + horizValue * RotationAuth * Time.deltaTime, 0));
        //RB.AddTorque(horizValue * RotationAuth * transform.up, ForceMode.Impulse);
        vertValue = 0;
        horizValue = 0;
    }

    [Command]
    void CmdFireWeapon(WeaponType type)
    {
        GameObject newWeapon;

        switch (type)
        {
            case WeaponType.Gun:
                {
                    newWeapon = Instantiate(WeaponObject1, WeaponLaunchPoint.position,
                        transform.rotation);
                    // GameManager.WeaponList.Add(newWeapon);

                    break;
                }
            case WeaponType.Bomb:
                {
                    newWeapon = Instantiate(WeaponObject2, WeaponLaunchPoint.position, transform.rotation);
                    break;
                }
            case WeaponType.Shield:
                {
                    
                    EnableShield();
                    //WeaponObject3.SetActive(true);
                    newWeapon = null;
                    break;
                }
            default:
                newWeapon = null;
                break;
        }

        if (newWeapon != null)
        {
            //Rigidbody newWeaponRB = newWeapon.GetComponent<Rigidbody>();
            WeaponStats weaponStats = newWeapon.GetComponent<WeaponStats>();
            newWeapon.GetComponent<Rigidbody>().velocity = RB.velocity + weaponStats.InitialForce * newWeapon.transform.forward;
            weaponStats.owner = this;
            //newWeapon.GetComponent<Rigidbody>().AddForce(weaponStats.InitialForce * transform.forward);
            Debug.Log("Owner Id: " + weaponStats.owner.netId);
            Debug.Log("RB Velocity: " + RB.velocity + newWeapon.GetComponent<Rigidbody>().velocity);
            //CurrentReloadTime[weaponStats.Type] = weaponStats.ReloadTime;
            NetworkServer.Spawn(newWeapon);

        }

    }

    public override void OnStartLocalPlayer()
    {
        //gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.blue;
        // HUDCanvas.SetActive(true);
        localRadarPos.material.color = Color.blue;
        // HealthCanvas.SetActive(false);
  
    }

    void OnNameChanged(string value)
    {
        playerName = value;
        gameObject.name = playerName;
        //Add code for name above the head  GetComponentInChildren<Text>(true).text = playerName;
        GetComponentInChildren<Text>(true).text = playerName;
    }

    void OnColorChanged(Color value)
    {
        playerColor = value;
        gameObject.GetComponentInChildren<MeshRenderer>().material.color = playerColor;
    }

    [Server]
    public void EnableShield()
    {
        WeaponObject3.SetActive(true);
        RpcEnableShield();
    }
    [ClientRpc]
    void RpcEnableShield()
    {
        WeaponObject3.SetActive(true);
    }

    [Server]
    public void ScoreKill()
    {
        score++;
        Debug.Log("Kill Scored for: " + netId + " Total Kills: " + score);
        if (GameManager.gameType == GameType.DeathMatch)
            if (score >= GameManager.KillsToWin)
                Won();
        //PlayerCanvas.canvas.SetKills(score);       
    }
    [Server]
    private void Won()
    {
        for (int i = 0; i < GameManager.Players.Count; i++)
            GameManager.Players[i].RpcGameOver(netId, name);

        Invoke("BackToLobby", 5f);
    }
    [ClientRpc]
    private void RpcGameOver(NetworkInstanceId networkId, string name)
    {
        DisablePlayer();
        if(isLocalPlayer)
        {
            if (netId == networkId)
                PlayerCanvas.canvas.SetGameStatusText("You Won!");
            else
                PlayerCanvas.canvas.SetGameStatusText("Game Over!\n" + name + " Won");
        }
    }

    void BackToLobby()
    {
        FindObjectOfType<NetworkLobbyManager>().SendReturnToLobby();
    }

    public void OnScoreChange(int amount)
    {
        score = amount;
        if (isLocalPlayer)
            PlayerCanvas.canvas.SetKills(score);
    }

    public void FireAsBot()
    {
        CmdFireWeapon(WeaponType.Gun);
    }

    public void MoveAsBot(Vector3 movement)
    {
        horizValue += movement.x;
        vertValue += movement.y;

    }

}
