using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum WeaponType
{
    Gun = 0,
    Bomb = 1,
    Shield = 2
};
public enum GameType
{
    DeathMatch,
    TeamDeathMatch,
    BattleRoyal,
    AssetDefense
}
public class GameManager : NetworkBehaviour
{
    //public static List<GameObject> WeaponList;
    public static GameManager m_Instance;
    public static List<PlayerControlScript> Players = new List<PlayerControlScript>();

    public List<Transform> waypointsForAI = new List<Transform>();
    public bool isAiActive = true;

    public State DefaultState;
    public State RemainInState;

    public static bool isShuttingDown = false;

    public static GameType gameType = GameType.DeathMatch;
    public static int KillsToWin = 3;
    // Use this for initialization
   
   void Awake()
   {
        m_Instance = this;
   }
   [ServerCallback]
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {

    }

    public static void AddPlayer()
    { }

    void OnApplicationQuit()
    {
        isShuttingDown = true;
    }
}
