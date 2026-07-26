using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Movement")]
    public float forwardSpeed = 8f;
    public float maxSpeed = 20f;
    public float speedIncreaseRate = 0.2f;

    [Header("Turning")]
    public float turnDuration = 0.3f;   // time to complete 90°
    public float driftFactor = 0.92f;   // lower = more drift

    private Rigidbody rb;
    private Vector3 moveDirection;

    private bool isHolding = false;
    private bool isTurning = false;

    private Quaternion fromRotation;
    private Quaternion toRotation;
    private float turnProgress = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        moveDirection = transform.forward;
    }

    void Update()
    {
        bool pressing = Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space);

       
        if (pressing && !isHolding && !isTurning)
        {
            isHolding = true;
            StartTurn(90f);
        }

       
        if (!pressing && isHolding && !isTurning)
        {
            isHolding = false;
            StartTurn(-90f);
        }

       
        if (forwardSpeed < maxSpeed)
            forwardSpeed += speedIncreaseRate * Time.deltaTime;
    }

    void StartTurn(float angle)
    {
        fromRotation = rb.rotation;
        toRotation = fromRotation * Quaternion.Euler(0f, angle, 0f);
        turnProgress = 0f;
        isTurning = true;
    }

    void FixedUpdate()
    {
        // Smooth 90° turn
        if (isTurning)
        {
            turnProgress += Time.fixedDeltaTime / turnDuration;
            float t = Mathf.Clamp01(turnProgress);

            float eased = EaseInOutCubic(t);
            Quaternion newRot = Quaternion.Slerp(fromRotation, toRotation, eased);
            rb.MoveRotation(newRot);

            if (t >= 1f)
                isTurning = false;
        }
        
        moveDirection = Vector3.Lerp(moveDirection, transform.forward, 1f - driftFactor);

        rb.MovePosition(rb.position + moveDirection * forwardSpeed * Time.fixedDeltaTime);
    }

    float EaseInOutCubic(float t)
    {
        return t < 0.5f 
            ? 4f * t * t * t 
            : 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;
    }
}