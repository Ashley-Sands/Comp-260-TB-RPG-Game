using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveAgent : MonoBehaviour
{
    NavMeshAgent myNavMeshAgent;
    public bool followMouse = false;
    Vector3 loc;
    public float stopDist = 5f;
    public float speed = 8;
    public LayerMask layerMask;

    void Start ()
    {
        myNavMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Update ()
    {

        print( Vector3.Distance( transform.position, loc ) + " < " + stopDist );

        if ( followMouse || Input.GetMouseButtonDown( 0 ) )
        {
            SetDestinationToMousePosition();
        }

        if ( Vector3.Distance(transform.position, loc) < stopDist )
        {
            myNavMeshAgent.speed = 0;
            transform.position = new Vector3(loc.x, transform.position.y, loc.z);
        }
        else
        {
            myNavMeshAgent.speed = speed;
        }

        transform.rotation = Quaternion.identity;


    }

    void SetDestinationToMousePosition ()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
        if ( Physics.Raycast( ray, out hit, 300, layerMask ) ) 
        {
            myNavMeshAgent.SetDestination( hit.point );
        }

        loc = new Vector3((int)hit.point.x, hit.point.y, (int)hit.point.z );

        Debug.DrawRay( ray.origin, ray.direction * 500, Color.red );

    }
}
