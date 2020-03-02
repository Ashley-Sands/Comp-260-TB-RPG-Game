using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CamControles : MonoBehaviour
{

    public float rotateSpeed = 12;
    public float moveSpeed = 12;
    public float zoomSpeed = 6;

    void Update()
    {
        Camera cam = GetComponent<Camera>();

        // left right
        float mSpeed = moveSpeed * Time.deltaTime;
        float rSpeed = rotateSpeed * Time.deltaTime;
        float zSpeed = zoomSpeed * Time.deltaTime;

        Vector3 currentPosition = transform.position;
        Vector3 currentRotation = transform.eulerAngles;

        if ( Input.GetKey( "a" ) )
            currentPosition -= transform.right * mSpeed;

        if ( Input.GetKey( "d" ) )
            currentPosition += transform.right * mSpeed;

        if ( Input.GetKey( "w" ) )
            cam.orthographicSize += zSpeed;

        if ( Input.GetKey( "s" ) )
            cam.orthographicSize -= zSpeed;

        if ( Input.GetKey( "r" ) )
            currentPosition += transform.up * mSpeed;

        if ( Input.GetKey( "f" ) )
            currentPosition -= transform.up * mSpeed;

        if ( Input.GetKey( "q" ) )
            currentRotation.y -= rSpeed;

        if ( Input.GetKey( "e" ) )
            currentRotation.y += rSpeed;

        transform.position = currentPosition;
        transform.eulerAngles = currentRotation;

    }
}
