using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    [Header("Movement")]
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float speedAdjustmentFactor = 2.0f;
    public float jumpSpeed = 8.0f;

    [Header("Physics")]
    public float gravity = 20.0f;
    public float pushPower = 2.0f;

    [Header("Camera")]
    public Transform playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;

    CharacterController characterController;

    Vector3 velocity = Vector3.zero;
    float rotationX = 0;
    float moveX;
    float moveY;
    float speedControl = 1.0f;


    void Start() {
        characterController = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    void Update() {
        if (Input.GetKey(KeyCode.LeftControl)) {
            speedControl = Mathf.Max(0, speedControl += Input.mouseScrollDelta.y * speedAdjustmentFactor);
        }

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float speed = Mathf.Pow((isRunning ? runningSpeed : walkingSpeed), speedControl);

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float movementDirectionY = velocity.y;
        moveX = Input.GetAxis("Vertical");
        moveY = Input.GetAxis("Horizontal");
        velocity = Vector3.ClampMagnitude(((forward * moveX) + (right * moveY)), 1.0f) * speed;

        if (Input.GetButton("Jump") && characterController.isGrounded) {
            velocity.y = jumpSpeed;
        } else {
            velocity.y = movementDirectionY;
        }

        if (!characterController.isGrounded) {
            velocity.y -= gravity * Time.deltaTime;
        }

        characterController.Move(velocity * Time.deltaTime);

        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
    }


    void OnControllerColliderHit(ControllerColliderHit hit) {
        Rigidbody body = hit.collider.attachedRigidbody;

        if (body == null || body.isKinematic) { 
            return; 
        }

        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

        body.AddForceAtPosition(pushDir * pushPower, hit.point, ForceMode.Force);
    }
}
