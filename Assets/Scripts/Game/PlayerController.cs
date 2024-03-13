using Cinemachine;
using GameFramework.Network.Movement;
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
    [SerializeField]
    private NetworkMovementComponent playerMovement;

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

        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }
    private void Update()
    {
        Vector2 moveInput = pControl.Player.Move.ReadValue<Vector2>();
        Vector2 lookInput = pControl.Player.Look.ReadValue<Vector2>();


        if (IsClient && IsLocalPlayer)
        {
            playerMovement.ProcessLocalPlayerMovement(moveInput, lookInput);
        }
        else
        {
            playerMovement.ProcessSimulatedPlayerMovement();
        }
    }
    //private void Move(Vector2 moveInput)
    //{
    //    Vector3 movement = moveInput.x * trfCam.right + moveInput.y * trfCam.forward;

    //    movement.y = 0;

    //    cc.Move(movement * speed * Time.deltaTime);
    //}
    //private void Rotate(Vector2 lookInput)
    //{
    //    transform.RotateAround(transform.position, transform.up, lookInput.x * turnSpeed * Time.deltaTime);
    //}
    //private void RotateCamera(float lookY)
    //{
    //    cameraAngle = Vector3.SignedAngle(transform.forward, trfCam.forward, trfCam.right);
    //    float cameraRotationAmount = lookY * turnSpeed * Time.deltaTime;
    //    float newCameraAngle = cameraAngle - cameraRotationAmount;
    //    if (newCameraAngle <= minMaxRotationX.x && newCameraAngle >= minMaxRotationX.y)
    //    {
    //        trfCam.RotateAround(trfCam.position, trfCam.right, lookY * turnSpeed * Time.deltaTime);
    //    }
    //}
    //[ServerRpc]
    //private void MoveServerRPC(Vector2 moveInput, Vector2 lookInput)
    //{
    //    Move(moveInput);
    //    Rotate(lookInput);
    //}
}
