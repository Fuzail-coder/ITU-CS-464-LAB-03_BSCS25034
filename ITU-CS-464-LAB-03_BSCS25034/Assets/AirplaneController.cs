using UnityEngine;

public class AirplaneController : MonoBehaviour
{
    [Header("Speed")]
    public float acceleration = 20f;
    public float maxSpeed = 100f;
    public float brakeForce = 40f;
    public float startingSpeed = 30f;

    [Header("Movement")]
    public float pitchSpeed = 50f;
    public float rollSpeed = 70f;
    public float yawSpeed = 30f;

    [Header("Flight")]
    public bool engineAlwaysOn = true;

    private float currentSpeed;

    void Start()
    {
        currentSpeed = startingSpeed;
    }

    void Update()
    {
        HandleSpeed();
        HandleRotation();
    }

    void FixedUpdate()
    {
        FlyForward();
    }

    void HandleSpeed()
    {
        // W = accelerate
        if (Input.GetKey(KeyCode.W))
        {
            currentSpeed += acceleration * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            // S = brake
            currentSpeed -= brakeForce * Time.deltaTime;
        }

        // Keep speed within limits
        currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);
    }

    void HandleRotation()
    {
        float pitch = 0f;
        float roll = 0f;

        // W = nose down
        if (Input.GetKey(KeyCode.W))
            pitch = 1f;

        // S = nose up
        if (Input.GetKey(KeyCode.S))
            pitch = -1f;

        // A = roll left
        if (Input.GetKey(KeyCode.A))
            roll = 1f;

        // D = roll right
        if (Input.GetKey(KeyCode.D))
            roll = -1f;

        float pitchAmount = pitch * pitchSpeed * Time.deltaTime;
        float rollAmount = roll * rollSpeed * Time.deltaTime;

        transform.Rotate(
            pitchAmount,
            0f,
            rollAmount,
            Space.Self
        );
    }

    void FlyForward()
    {
        transform.position += transform.forward * currentSpeed * Time.fixedDeltaTime;
    }
}