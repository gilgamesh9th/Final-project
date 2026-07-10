using UnityEngine;

public class CharController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private float verticalLookMin = -80f;
    [SerializeField] private float verticalLookMax = 80f;
    private float xRotation = 0f;

    private CharacterController _controller;
    private Vector3 _velocity;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        CameraRot();

        bool isGrounded = _controller.isGrounded;

        if (isGrounded && _velocity.y < 0f)
            _velocity.y = -2f;

        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector3 move = transform.right * input.x + transform.forward * input.y;
        _controller.Move(move * moveSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
            _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        _velocity.y += gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }

    void CameraRot()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, verticalLookMin, verticalLookMax);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}