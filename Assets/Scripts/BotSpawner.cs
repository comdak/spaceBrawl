using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;

public class BotSpawner : NetworkBehaviour {

    [SerializeField] GameObject botPrefab;

    [ServerCallback]
    private void Start()
    {
        GameObject obj = Instantiate(botPrefab, transform.position, transform.rotation);
        obj.GetComponent<NetworkIdentity>().localPlayerAuthority = false;
        obj.AddComponent<Bot>();
        obj.AddComponent<NavMeshAgent>();
        StateController stateController = obj.AddComponent<StateController>();
        stateController.eyes = obj.transform.Find("Eyes");
        stateController.currentState = GameManager.m_Instance.DefaultState;
        stateController.remainState = GameManager.m_Instance.RemainInState;
        stateController.shipStats = obj.GetComponent<ShipStats>();

        stateController.SetupAI(GameManager.m_Instance.isAiActive, GameManager.m_Instance.waypointsForAI);

        NetworkServer.Spawn(obj);
    }
}
