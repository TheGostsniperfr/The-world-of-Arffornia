using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    private Vector3 velocity;
    private Vector3 rotation;
    private Vector3 cameraRotation;

    private Rigidbody rb;

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private float jumpForce = 10f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

    }



    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }

    public void Rotate(Vector3 _rotation)
    {
        rotation = _rotation;
    }

    public void CameraRotate(Vector3 _cameraRotation)
    {
        cameraRotation = _cameraRotation;
    }

  


    private void Update()
    {
        PerformMovement();
        PerformRotation();
    }

    private void PerformMovement()
    {
        if(velocity != Vector3.zero)
        {
            rb.MovePosition(rb.position + velocity * Time.deltaTime);
        }

        


    }

    public void jump()
    {
        rb.AddForce(new Vector3(0f, jumpForce, 0f), ForceMode.Acceleration);
    }


    private void PerformRotation()
    {
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
        cam.transform.Rotate(-cameraRotation);

    }





}
