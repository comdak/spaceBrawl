using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateController : MonoBehaviour {

    public State currentState;
    public ShipStats shipStats;
    public Transform eyes;
    public State remainState;


    [HideInInspector] public NavMeshAgent navMeshAgent;
    [HideInInspector] public PlayerControlScript player;
    [HideInInspector] public List<Transform> wayPointList;
    [HideInInspector] public int nextWaypoint;
    [HideInInspector] public Transform chaseTarget;
    [HideInInspector] public float stateTimeElapsed;


    private bool aiActive;


    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        player = GetComponent<PlayerControlScript>();
    }

    public void SetupAI(bool aiActivationFromGameManager, List<Transform> wayPointsFromGameManger)
    {
        wayPointList = wayPointsFromGameManger;
        aiActive = aiActivationFromGameManager;
        if (aiActive)
        {
            navMeshAgent.enabled = true;
        }
        else
        {
            navMeshAgent.enabled = false;
        }


    }
    private void Update()
    {
        if (!aiActive)
            return;

        currentState.UpdateState(this);

    }

    private void OnDrawGizmos()
    {
        if(currentState != null && eyes !=null )
        {
            Gizmos.color = currentState.sceneGizmoColor;
            Gizmos.DrawWireSphere(eyes.position, shipStats.lookSphereCastRadius);
        }
    }

    public void TransitionToState(State nextState)
    {
        if (nextState != remainState)
        {
            currentState = nextState;
            OnExitState();
        }
    }

    public bool CheckIfCountDownElapsed(float duration)
    {
        stateTimeElapsed += Time.deltaTime;
        return (stateTimeElapsed >= duration);
    }


    private void OnExitState()
    {
        stateTimeElapsed = 0;
    }


}
