using UnityEngine;

public class AirplaneController : MonoBehaviour
{
    [Header("Speed")]
    public float acceleration = 20f;
    public float maxSpeed = 100f;
    public float speedDecay = 10f;      // how fast speed drops when not accelerating
    public float brakeForce = 40f;      // how fast speed drops while braking
    public float minSpeed = 10f;        // floor while flying (not braking)
    public float startingSpeed = 30f;

    [Header("Movement")]
    public float pitchSpeed = 50f;      // deg/sec while W/S held
    public float rollSpeed = 70f;       // deg/sec while A/D held
    public float yawSpeed = 40f;        // max turn deg/sec at full bank

    [Header("Limits (this is what stops the flipping)")]
    public float maxPitchAngle = 60f;   // how far nose can point up/down
    public float maxRollAngle = 45f;    // how far the plane can bank left/right
    public float autoLevelSpeed = 30f;  // how fast pitch/roll drift back to 0 with no input

    [Header("Flight")]
    public bool engineAlwaysOn = true;

    private float currentSpeed;

    // Tracked orientation, set directly each frame instead of compounded with
    // transform.Rotate — accumulating local rotations frame after frame is
    // what causes a plane to slowly tumble/flip out of control.
    private float pitchAngle = 0f;
    private float rollAngle = 0f;
    private float yawAngle = 0f;

    void Start()
    {
        currentSpeed = startingSpeed;
        Vector3 startEuler = transform.eulerAngles;
        pitchAngle = NormalizeAngle(startEuler.x);
        yawAngle = startEuler.y;
        rollAngle = NormalizeAngle(startEuler.z);
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
        bool braking = Input.GetKey(KeyCode.Space);

        if (braking)
        {
            // Space = brake, can bring the plane all the way to a stop for landing
            currentSpeed -= brakeForce * Time.deltaTime;
            currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);
            return;
        }

        // Shift = accelerate
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            currentSpeed += acceleration * Time.deltaTime;
        }
        else
        {
            // Gradually decay back toward min speed when not accelerating
            currentSpeed -= speedDecay * Time.deltaTime;
        }

        // Keep speed within limits (minSpeed floor only applies when not braking)
        currentSpeed = Mathf.Clamp(currentSpeed, minSpeed, maxSpeed);
    }

    void HandleRotation()
    {
        float pitchInput = 0f;
        float rollInput = 0f;

        // W = nose up (head up)
        if (Input.GetKey(KeyCode.W))
            pitchInput = -1f;

        // S = nose down (head down)
        if (Input.GetKey(KeyCode.S))
            pitchInput = 1f;

        // A = roll/turn left
        if (Input.GetKey(KeyCode.A))
            rollInput = 1f;

        // D = roll/turn right
        if (Input.GetKey(KeyCode.D))
            rollInput = -1f;

        // --- Pitch: move toward input, clamp to a max angle, auto-level when released ---
        if (pitchInput != 0f)
        {
            pitchAngle += pitchInput * pitchSpeed * Time.deltaTime;
            pitchAngle = Mathf.Clamp(pitchAngle, -maxPitchAngle, maxPitchAngle);
        }
        else
        {
            pitchAngle = Mathf.MoveTowards(pitchAngle, 0f, autoLevelSpeed * Time.deltaTime);
        }

        // --- Roll: same idea, capped so it banks instead of barrel-rolling ---
        if (rollInput != 0f)
        {
            rollAngle += rollInput * rollSpeed * Time.deltaTime;
            rollAngle = Mathf.Clamp(rollAngle, -maxRollAngle, maxRollAngle);
        }
        else
        {
            rollAngle = Mathf.MoveTowards(rollAngle, 0f, autoLevelSpeed * Time.deltaTime);
        }

        // --- Yaw: turning is driven by how much the plane is banked, like a real coordinated turn ---
        float bankFraction = rollAngle / maxRollAngle; // -1..1
        yawAngle += bankFraction * yawSpeed * Time.deltaTime;

        // Apply the orientation directly instead of stacking Rotate() calls
        transform.rotation = Quaternion.Euler(pitchAngle, yawAngle, rollAngle);
    }

    void FlyForward()
    {
        transform.position += transform.forward * currentSpeed * Time.fixedDeltaTime;
    }

    // Converts a 0-360 Unity euler angle into a -180..180 range so clamping works as expected
    private float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle > 180f) angle -= 360f;
        return angle;
    }
}