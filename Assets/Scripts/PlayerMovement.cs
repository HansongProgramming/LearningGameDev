using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 5f;
    private bool isGrounded;

    private Rigidbody rb;
    private Transform cam;
    private float rotationSpeed = 700f;  // Rotation speed

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main.transform; // Reference to the main camera
    }

    void Update()
    {
        Move();
        Jump();
    }

    void Move()
    {
        float moveX = Input.GetAxis("Horizontal"); // A, D (or Left/Right Arrows)
        float moveZ = Input.GetAxis("Vertical");   // W, S (or Up/Down Arrows)

        // Calculate movement direction relative to the camera's orientation
        Vector3 forward = cam.forward;
        forward.y = 0; // Ensure movement is only along the XZ plane
        forward.Normalize();

        Vector3 right = cam.right;
        right.y = 0; // Ensure movement is only along the XZ plane
        right.Normalize();

        // Compute the desired movement direction
        Vector3 moveDirection = (forward * moveZ + right * moveX).normalized;

        // Apply movement
        Vector3 velocity = moveDirection * speed;
        velocity.y = rb.linearVelocity.y; // Preserve the existing vertical velocity
        rb.linearVelocity = velocity;

        // Only rotate if there is movement input and the movement direction is significant
        if (moveDirection != Vector3.zero)
        {
            // Calculate the desired rotation (only around the Y-axis)
            Quaternion toRotation = Quaternion.LookRotation(moveDirection);
            Vector3 eulerRotation = toRotation.eulerAngles;
            eulerRotation.x = 0; // Prevent rotation around X-axis
            eulerRotation.z = 0; // Prevent rotation around Z-axis
            toRotation = Quaternion.Euler(eulerRotation);

            // Smooth rotation (limit to Y-axis rotation)
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false; // Prevents double jumps
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
