using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float zoomSpeed = 10.0f; // Adjust the zoom speed
    public float minFOV = 20.0f; // Minimum field of view
    public float maxFOV = 90.0f; // Maximum field of view

    void Update()
    {
        float fov = Camera.main.fieldOfView;

        // Use the + and - keys for zooming in and out
        if (Input.GetKey(KeyCode.Equals) || Input.GetKey(KeyCode.KeypadPlus))
        {
            fov -= zoomSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.Minus) || Input.GetKey(KeyCode.KeypadMinus))
        {
            fov += zoomSpeed * Time.deltaTime;
        }

        fov = Mathf.Clamp(fov, minFOV, maxFOV);
        Camera.main.fieldOfView = fov;
    }
}
