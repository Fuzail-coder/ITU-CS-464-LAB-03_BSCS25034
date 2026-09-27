using UnityEngine;

public class AirplaneCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform airplane;

    [Header("Camera Position")]
    public float distance = 12f;
    public float height = 5f;

    [Header("Smoothness")]
    public float positionSmoothness = 5f;
    public float rotationSmoothness = 5f;

    void LateUpdate()
    {
        if (airplane == null)
            return;

        // Position behind and above the airplane
        Vector3 targetPosition =
            airplane.position
            - airplane.forward * distance
            + Vector3.up * height;

        // Smoothly move camera
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            positionSmoothness * Time.deltaTime
        );

        // Look at the airplane
        Vector3 direction = airplane.position - transform.position;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSmoothness * Time.deltaTime
            );
        }
    }
}