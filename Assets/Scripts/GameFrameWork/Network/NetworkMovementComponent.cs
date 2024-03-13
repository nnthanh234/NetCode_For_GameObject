using System;
using Unity.Netcode;
using UnityEngine;

namespace GameFramework.Network.Movement
{
    public class NetworkMovementComponent : NetworkBehaviour
    {
        [SerializeField]
        private CharacterController cc;
        [SerializeField]
        private float speed;
        [SerializeField]
        private float turnSpeed;
        [SerializeField]
        private Transform camSocket;
        [SerializeField]
        private GameObject vcam;

        private Transform vcamTransform;
        private int tick = 0;
        private float tickRate = 1/60f;
        private float tickDeltaTime = 0f;

        private const int BUFFER_SIZE = 1024;
        private InputState[] inputState = new InputState[BUFFER_SIZE];
        private TransformState[] transformStates = new TransformState[BUFFER_SIZE];

        public NetworkVariable<TransformState> ServerTransformState = new NetworkVariable<TransformState>();
        public TransformState previousTransformState;

        private void OnEnable()
        {
            ServerTransformState.OnValueChanged += OnServerStateChanged;
        }
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            vcamTransform = vcam.transform;
        }
        private void OnServerStateChanged(TransformState previousValue, TransformState newValue)
        {
            previousTransformState = previousValue;
        }
        public void ProcessLocalPlayerMovement(Vector2 movementInput, Vector2 lookInput)
        {
            tickDeltaTime += Time.deltaTime;
            if (tickDeltaTime > tickRate)
            {
                int buffer = tick % BUFFER_SIZE;

                if (!IsServer)
                {
                    MovePlayerServerRPC(tick, movementInput, lookInput);
                    MovePlayer(movementInput);
                    RotatePlayer(lookInput);
                }
                else
                {
                    MovePlayer(movementInput);
                    RotatePlayer(lookInput);

                    TransformState state = new TransformState()
                    {
                        Tick = tick,
                        Position = transform.position,
                        Rotation = transform.rotation,
                        HasStartedMoving = true
                    };

                    previousTransformState = ServerTransformState.Value;
                    ServerTransformState.Value = state;
                }

                InputState ipState = new InputState()
                {
                    Tick = tick,
                    movementInput = movementInput,
                    lookInput = lookInput
                };

                TransformState trfState = new TransformState()
                {
                    Tick = tick,
                    Position = transform.position,
                    Rotation = transform.rotation,
                    HasStartedMoving = true
                };

                inputState[buffer] = ipState;
                transformStates[buffer] = trfState;

                tickDeltaTime -= tickRate;
                tick++;
            }
        }
        public void ProcessSimulatedPlayerMovement()
        {
            tickDeltaTime += Time.deltaTime;
            if (tickDeltaTime > tickRate)
            {
                if (ServerTransformState.Value.HasStartedMoving)
                {
                    transform.position = ServerTransformState.Value.Position;
                    transform.rotation = ServerTransformState.Value.Rotation;
                }
            }

            tickDeltaTime -= tickRate;
            tick++;
        }
        [ServerRpc]
        private void MovePlayerServerRPC(int tick, Vector2 movementInput, Vector2 lookInput)
        {
            MovePlayer(movementInput);
            RotatePlayer(lookInput);

            TransformState state = new TransformState()
            {
                Tick = tick,
                Position = transform.position,
                Rotation = transform.rotation,
                HasStartedMoving = true
            };

            previousTransformState = ServerTransformState.Value;
            ServerTransformState.Value = state;
        }        
        private void MovePlayer(Vector2 movementInput)
        {
            Vector3 movement = movementInput.x * vcamTransform.right + movementInput.y * vcamTransform.forward;
            movement.y = 0;

            //if (!cc.isGrounded)
            //{
            //    movement.y = -9.81f;
            //}

            cc.Move(movement * speed * tickRate);
        }
        private void RotatePlayer(Vector2 lookInput)
        {
            vcamTransform.RotateAround(vcamTransform.position, vcamTransform.right, lookInput.y * turnSpeed * tickRate);
            transform.RotateAround(transform.position, transform.up, lookInput.x * turnSpeed * tickRate);
        }

    }
}
