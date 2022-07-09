
using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 3f;

    [SerializeField]
    private float mouseSensitivityX = 10f;

    [SerializeField]
    private float mouseSensitivityY = 10f;



    private PlayerMotor motor;

    private void Start()
    {
        motor = GetComponent<PlayerMotor>();
    }

    private void Update()
    {
        //Player velocity calcul

        float xMov = Input.GetAxisRaw("Horizontal");
        float zMov = Input.GetAxisRaw("Vertical");

        Vector3 moveHorizontal = transform.right * xMov;
        Vector3 moveVertical = transform.forward * zMov;

        Vector3 velocity = (moveHorizontal + moveVertical).normalized * speed;

        motor.Move(velocity);


        //Player rotation calcul
        float yRot = Input.GetAxisRaw("Mouse X");
        float xRot = Input.GetAxisRaw("Mouse Y");

        Vector3 Playerrotation = new Vector3(0, yRot, 0) * mouseSensitivityX;
        motor.Rotate(Playerrotation);

        Vector3 cameraRotation = new Vector3(xRot, 0, 0) * mouseSensitivityY;
        motor.CameraRotate(cameraRotation);
       

    }
}
