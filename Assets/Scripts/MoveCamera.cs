using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public GameObject cameraObject;
    public float rateOfRotation;
    public float rateOfMovement;

    // Update is called once per frame
    void Update()
    {

        if (GameManager.instance.isPaused) // check if game is paused
        {
            return; // we don't update 
        }

        MoveCameraPosition(); // move camera position
        MoveCameraRotation(); // rotate camera
    }

    // move camera position based on WASD keyboard.
    void MoveCameraPosition()
    {
        if (Input.GetKey(KeyCode.W)) // checks if user presses W on keyboard.
        {
            cameraObject.transform.position = cameraObject.transform.position + new Vector3(0, rateOfMovement, 0); // move camera on y axis
        }

        if (Input.GetKey(KeyCode.A)) // checks if user presses A on keyboard.
        {
            cameraObject.transform.position = cameraObject.transform.position + new Vector3(-rateOfMovement, 0, rateOfMovement); // move camera on x-z axis
        }

        if (Input.GetKey(KeyCode.S)) // checks if user presses S on keyboard. 
        {
            cameraObject.transform.position = cameraObject.transform.position + new Vector3(0, -rateOfMovement, 0); // move camera on y axis
        }

        if (Input.GetKey(KeyCode.D)) // checks if user presses D on keyboard.
        {
            cameraObject.transform.position = cameraObject.transform.position + new Vector3(rateOfMovement, 0, -rateOfMovement); // move camera on x-z axis
        }
    }

    // move camera raotation mased on left mouse button
    void MoveCameraRotation()
    {
        if (Input.GetMouseButton(0)) // check if user rpesses left mouse button
        {
            cameraObject.transform.eulerAngles += rateOfRotation * new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0); // rotate camera
        }
    }
}
