using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeLookCamera : MonoBehaviour
{
    public float rotateSpeed = 5.0f;
    public float zoomSpeed = 20.0f;

    private float distance;
    private Vector3 offset;
    public float panSpeed = 5f; 
    private Vector3 dragOrigin;

    private void Start()
    {
        offset = transform.position;
        distance = offset.magnitude;
    }

    private void Update()
    {
        if (Input.GetMouseButton(1)) // Right mouse button
        {
            float h = rotateSpeed * Input.GetAxis("Mouse X");
            float v = rotateSpeed * Input.GetAxis("Mouse Y");

            transform.RotateAround(Vector3.zero, Vector3.up, h);
            transform.RotateAround(Vector3.zero, transform.right, -v);
        }

        distance -= Input.mouseScrollDelta.y * zoomSpeed * Time.deltaTime;
        distance = Mathf.Max(distance, 0.1f); // Prevent the camera from going inside the object

        transform.position = transform.rotation * new Vector3(0, 0, -distance);

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0.0f)
        {
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - scroll * zoomSpeed, 10, 20);
        }

        if (Input.GetMouseButtonDown(2))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(2)) return;

        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition) - Camera.main.ScreenToViewportPoint(dragOrigin);
        Vector3 move = new Vector3(pos.x * panSpeed, 0, pos.y * panSpeed);

        transform.Translate(move, Space.World);
        dragOrigin = Input.mousePosition;
    }
}
