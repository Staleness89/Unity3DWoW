using Assets.Scripts.Shared;
using Client.World;
using Client.World.Definitions;
using Client.World.Network;
using System.Diagnostics;
using System.Timers;
using UnityEngine;

public enum MovementStatus
{
    Stand, Run, StrafeLeft, StrafeRight, Jumping
}

public class GroundChecker : MonoBehaviour
{
    public int GroundCollisions { get; set; }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            GroundCollisions++;
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            GroundCollisions--;
    }
}


public class WarcraftController : MonoBehaviour
{

    private CapsuleCollider unitCollider;
    private Animator animator;
    private Stopwatch fallTime;

    [SerializeField]
    bool isPlayerControlled = false;
    [SerializeField]
    float jumpSpeed = 4.0f;
    [SerializeField]
    float rotateSpeed = 50.0f;
    [SerializeField]
    float baseGroundCheckDistance = 0.2f;
    float groundCheckDistance = 0.2f;

    UnityEngine.Vector3 groundNormal = UnityEngine.Vector3.up;
    UnityEngine.Vector3 inputVelocity = UnityEngine.Vector3.zero;
    
    Rigidbody unitRigidbody;
    GroundChecker groundChecker;

    MovementStatus movementStatus;
    bool jumping = false;
    bool grounded = false;
    bool wasGrounded = false;


    public bool OnEdge
    {
        get
        {
            return !grounded && TouchingGround;
        }
    }

    public bool TooSteep
    {
        get
        {
            return (groundNormal.y <= Mathf.Cos(45 * Mathf.Deg2Rad));
        }
    }

    public bool OnSlope
    {
        get
        {
            return (groundNormal.y >= Mathf.Cos(5 * Mathf.Deg2Rad));
        }
    }

    public bool TouchingGround
    {
        get
        {
            return groundChecker.GroundCollisions > 0;
        }
    }


    void Awake()
    {
        unitRigidbody = GetComponent<Rigidbody>();
        fallTime = new Stopwatch();
        animator = UnityEngine.GameObject.Find("Mage").GetComponent<Animator>();
        unitCollider = GetComponent<CapsuleCollider>();
        groundChecker = GetComponentInChildren<GroundChecker>();

        groundCheckDistance = baseGroundCheckDistance;
    }

    void Update()
    {

        if (!isPlayerControlled)
        {
            if (grounded)
                ApplyGroundedAnimations();
            else
                ApplyFlyingAnimations();

            return;
        }
        // Only allow movement and jumps while grounded
        ApplyInputVelocity();

        // Allow turning at anytime. Keep the character facing in the same direction as the Camera if the right mouse button is down.
        ApplyInputRotation();
    }

    void FixedUpdate()
    {
        unitCollider.radius = 0.2f;

        if (jumping)
        {
            unitRigidbody.velocity = inputVelocity;
            groundCheckDistance = 0.05f;
            jumping = false;
        }
        else if (grounded)
        {
            unitRigidbody.velocity = new UnityEngine.Vector3(inputVelocity.x, unitRigidbody.velocity.y, inputVelocity.z);
           
            if (wasGrounded)
                groundCheckDistance = baseGroundCheckDistance;
        }
        else if (groundCheckDistance < baseGroundCheckDistance)
            groundCheckDistance = unitRigidbody.velocity.y < 0 ? baseGroundCheckDistance : groundCheckDistance + 0.01f;

        CheckGroundStatus();
    }


    void ApplyInputVelocity()
    {
        if (grounded)
        {
            //movedirection
            inputVelocity = new UnityEngine.Vector3((Input.GetMouseButton(1) ? Input.GetAxis("Horizontal") : 0), 0, Input.GetAxis("Vertical"));

            //L+R MouseButton Movement
            if (Input.GetMouseButton(0) && Input.GetMouseButton(1) && (Input.GetAxis("Vertical") == 0))
            {
                inputVelocity.z += 1;
            }

            if (inputVelocity.z > 1)
            {
                inputVelocity.z = 1;
            }

            //Strafing move (like Q/E movement    
            inputVelocity.x -= Input.GetAxis("Strafing");

            // if moving forward and to the side at the same time, compensate for distance
            if (Input.GetMouseButton(1) && (Input.GetAxis("Horizontal") != 0) && (Input.GetAxis("Vertical") != 0))
            {
                inputVelocity *= 0.7f;
            }

            // Check roots and apply final move speed
            inputVelocity *= Exchange.authClient.Player.Movement.RunSpeed;

            // Jump!
            if (Input.GetButton("Jump") || jumping)
            {
                jumping = true;
                inputVelocity.y = jumpSpeed;
                fallTime.Start();
            }

            if (Input.GetKeyDown(KeyCode.Space)) // && isnt doing other activities
            {
                Exchange.authClient.movementMgr.Flags.SetMoveFlag(MovementFlags.MOVEMENTFLAG_FALLING);
                Exchange.authClient.movementMgr.SendMoveJump(transform.position, transform.rotation);
                Exchange.authClient.movementMgr.SendHeartBeat(transform.position, transform.rotation);
            }

                ApplyGroundedAnimations();
            
            inputVelocity = transform.TransformDirection(inputVelocity);            
        }
        else
        {
            inputVelocity = UnityEngine.Vector3.zero;
            ApplyFlyingAnimations();
            
        }

    }

    void ApplyInputRotation()
    {
        if (Input.GetMouseButton(1))
        {
            transform.rotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
            stopping = true;
        }
        else
        {
            transform.Rotate(0, Input.GetAxis("Horizontal") * rotateSpeed * Time.deltaTime, 0);
            if (Input.GetKeyDown(KeyCode.A)) // && isnt doing other activities
            {
                Exchange.authClient.movementMgr.SendMoveLeft(transform.position, transform.rotation);
            }

            transform.Rotate(0, Input.GetAxis("Horizontal") * rotateSpeed * Time.deltaTime, 0);
            if (Input.GetKeyDown(KeyCode.D)) // && isnt doing other activities
            {
                Exchange.authClient.movementMgr.SendMoveRight(transform.position, transform.rotation);
            }

            if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.A)) // && isnt doing other activities
            {
                Exchange.authClient.movementMgr.SendStopTurn(transform.position, transform.rotation);
            }
        }
    }
    
    bool stopping = false;
    void ApplyGroundedAnimations()
    {
        var lastStatus = movementStatus;

        if (inputVelocity.x > 0)
        {
            movementStatus = MovementStatus.StrafeRight;
        }
        else if (inputVelocity.x < 0)
        {
            movementStatus = MovementStatus.StrafeLeft;
        }
        else if (inputVelocity.magnitude > 0)
        {
            movementStatus = MovementStatus.Run;
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) // && isnt doing other activities
            {
                Exchange.authClient.movementMgr.Flags.SetMoveFlag(MovementFlags.MOVEMENTFLAG_FORWARD);
                Exchange.authClient.movementMgr.MoveForward(transform.position, transform.rotation);               
            }

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))// && isnt doing other activities
            {
                Exchange.authClient.movementMgr.Flags.SetMoveFlag(MovementFlags.MOVEMENTFLAG_FORWARD);
                Exchange.authClient.movementMgr.SendHeartBeat(transform.position, transform.rotation);
            }

            if (Input.GetKeyDown(KeyCode.Mouse0) && Input.GetMouseButton(1) || Input.GetMouseButton(0) && Input.GetKeyDown(KeyCode.Mouse1)) // && isnt doing other activities
            {
                Exchange.authClient.movementMgr.Flags.SetMoveFlag(MovementFlags.MOVEMENTFLAG_FORWARD);
                Exchange.authClient.movementMgr.MoveForward(transform.position, transform.rotation);                
            }

            if (Input.GetMouseButton(0) && Input.GetMouseButton(1))
            {
                Exchange.authClient.movementMgr.Flags.SetMoveFlag(MovementFlags.MOVEMENTFLAG_FORWARD);
                Exchange.authClient.movementMgr.SendHeartBeat(transform.position, transform.rotation);
            }

            if (inputVelocity.magnitude < 1)
                stopping = true;
        }
        else
        {
            if (stopping)
            {
                stopping = false;
                Exchange.authClient.movementMgr.SendMoveStop(transform.position, transform.rotation);
            }
            movementStatus = MovementStatus.Stand;
        }

        animator.SetBool("Grounded", true);

        float strafeTarget = (UnityEngine.Vector3.Normalize(inputVelocity).x + 1) / 2;
        float currentStrafe = animator.GetFloat("Strafe");
        float strafeDelta = Time.deltaTime * 2 * Mathf.Sign(strafeTarget - currentStrafe);
        float resultStrafe = Mathf.Clamp(currentStrafe + strafeDelta, 0.0f, 1.0f);

        if (Mathf.Abs(strafeTarget - currentStrafe) > Mathf.Abs(strafeDelta))
            animator.SetFloat("Strafe", resultStrafe);


        if (lastStatus == MovementStatus.StrafeLeft || lastStatus == MovementStatus.StrafeRight)
            animator.SetFloat("Speed", 1);
        else
            animator.SetFloat("Speed", movementStatus == MovementStatus.Stand ? 0 : 1);
    }

    void ApplyFlyingAnimations()
    {
        movementStatus = MovementStatus.Jumping;

        animator.SetBool("Grounded", false);
    }

    bool falling = false;
    void CheckGroundStatus()
    {
        wasGrounded = grounded;
        RaycastHit hitInfo;

        if (Physics.Raycast(unitCollider.transform.position, -UnityEngine.Vector3.up, out hitInfo, baseGroundCheckDistance + 0.1f))
        {
            var distanceToGround = hitInfo.distance;

            if (distanceToGround > unitCollider.bounds.extents.y + groundCheckDistance)
            {
                if (grounded && inputVelocity.y <= 0)
                {
                    unitRigidbody.AddForce(UnityEngine.Vector3.down * unitRigidbody.velocity.magnitude, ForceMode.VelocityChange);
                    groundNormal = hitInfo.normal;                    
                    grounded = true;
                    Exchange.authClient.movementMgr.SendFallLand(transform.position, transform.rotation, (uint)fallTime.Elapsed.Milliseconds);
                    fallTime.Stop();
                    fallTime.Reset();
                }
                else
                {
                    grounded = false;
                    groundNormal = UnityEngine.Vector3.up;
                    falling = true;
                }
            }
            else
            {
                if (falling)
                {
                    falling = false;                    
                    Exchange.authClient.movementMgr.SendFallLand(transform.position, transform.rotation, (uint)fallTime.Elapsed.Milliseconds);
                    fallTime.Stop();
                    fallTime.Reset();
                }

                groundNormal = hitInfo.normal;
                grounded = true;
            }
        }
        else
        {
            grounded = false;
            falling = true;
            groundNormal = UnityEngine.Vector3.up;
        }

        if(Exchange.authClient.Player != null)
            Exchange.authClient.Player.IsGrounded = grounded;

        //if (TooSteep || OnEdge)
        //    unitCollider.material = GameManager.SlidingMaterial;
        //else
        //    unitCollider.material = GameManager.GroundedMaterial;
    }
}