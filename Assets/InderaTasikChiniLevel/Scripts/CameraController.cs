using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector3 offset = new Vector3(0f, 0f, -10f);
    private float smoothTime = 0.25f;
    private Vector3 velocity = Vector3.zero;

    [SerializeField] private Transform target;
    [SerializeField] private BoxCollider2D cameraBounds;

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition = target.position + offset;
        Vector3 smoothPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        Vector3 clampedPosition = ClampPositionToBounds(smoothPosition);
        transform.position = clampedPosition;
    }

    private Vector3 ClampPositionToBounds(Vector3 position)
    {
        // Get the bounds of the BoxCollider2D
        Bounds bounds = cameraBounds.bounds;

        // Clamp the camera position to the bounds
        float clampedX = Mathf.Clamp(position.x, bounds.min.x, bounds.max.x);
        float clampedY = Mathf.Clamp(position.y, bounds.min.y, bounds.max.y);

        return new Vector3(clampedX, clampedY, position.z);
    }
}
