using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveAgent : MonoBehaviour
{

    public int PlayerId { get; set; }

    NavMeshAgent myNavMeshAgent;
    public bool followMouse = false;
    Vector3 loc;
    public float stopDist = 5f;
    public float speed = 8;
    public LayerMask layerMask;

    void Start ()
    {
        myNavMeshAgent = GetComponent<NavMeshAgent>();
        Protocol.HandleProtocol.Inst.Bind( 'M', MovePlayer );
    }

    void Update ()
    {

//        print( Vector3.Distance( transform.position, loc ) + " < " + stopDist );

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

        if ( !SocketClient.ActiveGameData.IsPlayerAndActive( PlayerId ) ) 
            return; // its not this players turn.

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );

        if ( Physics.Raycast( ray, out hit, 300, layerMask ) ) 
        {
            Protocol.MovePlayerProtocol movePlayer = new Protocol.MovePlayerProtocol()
            {
                player_id = PlayerId,
                position = hit.point
            };

            // send the move to position to the server and update the local player :)
            SocketClient.ActiveSocket.QueueMessage( movePlayer );
            MoveAgentToPosition( hit.point );

        }

        loc = new Vector3((int)hit.point.x, hit.point.y, (int)hit.point.z );

        Debug.DrawRay( ray.origin, ray.direction * 500, Color.red );

    }

    private void MovePlayer( Protocol.BaseProtocol protocol)
    {
        Protocol.MovePlayerProtocol movePlayer = protocol as Protocol.MovePlayerProtocol;

        if ( movePlayer.player_id == PlayerId )
            MoveAgentToPosition( movePlayer.position );

    }

    private void MoveAgentToPosition( Vector3 position )
    {
        myNavMeshAgent.SetDestination( position );

    }

    private void OnDestroy ()
    {
        Protocol.HandleProtocol.Inst.Unbind( 'M', MovePlayer );

    }
}
