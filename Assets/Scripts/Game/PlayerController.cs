using Cinemachine;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : NetworkBehaviour
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private float turnSpeed;
    [SerializeField]
    private Vector2 minMaxRotationX;
    [SerializeField]
    private Transform trfCam;

    private CharacterController cc;
    private PlayControl pControl;
    private float cameraAngle;


    public override void OnNetworkSpawn()
    {
        CinemachineVirtualCamera cvCam = trfCam.gameObject.GetComponent<CinemachineVirtualCamera>();

        cvCam.Priority = IsHost ? 1 : 0;
    }
    private void Start()
    {
        cc = GetComponent<CharacterController>();
        pControl = new PlayControl();
        pControl.Enable();

        Cursor.lockState = CursorLockMode.Locked;
    }
    private void Update()
    {
        if (IsLocalPlayer)
        {
            if (pControl.Player.Move.inProgress)
            {
                Vector2 moveInput = pControl.Player.Move.ReadValue<Vector2>();
                Vector3 movement = moveInput.x * trfCam.right + moveInput.y * trfCam.forward;

                movement.y = 0;

                cc.Move(movement * speed * Time.deltaTime);
            }

            if (pControl.Player.Look.inProgress)
            {
                Vector2 lookInput = pControl.Player.Look.ReadValue<Vector2>();
                transform.RotateAround(transform.position, transform.up, lookInput.x * turnSpeed * Time.deltaTime);
                RotateCamera(lookInput.y);
            }
        }
    }

    private void RotateCamera(float lookY)
    {
        cameraAngle = Vector3.SignedAngle(transform.forward, trfCam.forward, trfCam.right);
        float cameraRotationAmount = lookY * turnSpeed * Time.deltaTime;
        float newCameraAngle = cameraAngle - cameraRotationAmount;
        if (newCameraAngle <= minMaxRotationX.x && newCameraAngle >= minMaxRotationX.y)
        {
            trfCam.RotateAround(trfCam.position, trfCam.right, lookY * turnSpeed * Time.deltaTime);
        }
    }
}
