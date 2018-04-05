
using UnityEngine;
using UnityEngine.Networking;

public class RadarScript : NetworkBehaviour
{
    public float RadarRadius = 10f;
    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update ()
    {
        //Debug.Log("Radar Pos");
        if (!isLocalPlayer)
            return;

        GameObject[] radarObjects = GameObject.FindGameObjectsWithTag("Radar");
        for(int i=0; i< radarObjects.Length; i++)
        {
            radarObjects[i].transform.localPosition = Vector3.zero;
            radarObjects[i].transform.position = transform.position + Vector3.ClampMagnitude(radarObjects[i].transform.position - transform.position, RadarRadius);
            
        }
	}
}
