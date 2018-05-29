using Assets.Scripts.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class CameraControllercs : MonoBehaviour {

    POINT mouse;
    public UnityEngine.GameObject target;                  // Target to follow
    public float targetHeight = 1.7f;          // Vertical offset adjustment
    public float distance = 12.0f;             // Default Distance
    public float offsetFromWall = 0.1f;        // Bring camera away from any colliding objects
    public float maxDistance = 20f;            // Maximum zoom Distance
    public float minDistance = 0.6f;           // Minimum zoom Distance
    public float xSpeed = 200.0f;              // Orbit speed (Left/Right)
    public float ySpeed = 200.0f;              // Orbit speed (Up/Down)
    public float yMinLimit = -80f;             // Looking up limit
    public float yMaxLimit = 80f;              // Looking down limit
    public float zoomRate = 40f;               // Zoom Speed
    public float rotationDampening = 3.0f;     // Auto Rotation speed (higher = faster)
    public float zoomDampening = 5.0f;         // Auto Zoom speed (Higher = faster)
    public LayerMask collisionLayers = -1;     // What the camera will collide with
    public bool lockToRearOfTarget = false;    // Lock camera to rear of target
    public bool allowMouseInputX = true;       // Allow player to control camera angle on the X axis (Left/Right)
    public bool allowMouseInputY = true;       // Allow player to control camera angle on the Y axis (Up/Down)

    private float xDeg = 0.0f;
    private float yDeg = 0.0f;
    private float currentDistance;
    private float desiredDistance;
    private float correctedDistance;
    private bool rotateBehind = false;
    private bool mouseSideButton = false;
    private float pbuffer = 0.0f;              //Cooldownpuffer for SideButtons
    private float coolDown = 0.5f;             //Cooldowntime for SideButtons 

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        xDeg = angles.x;
        yDeg = angles.y;
        currentDistance = distance;
        desiredDistance = distance;
        correctedDistance = distance;

        // Make the rigid body not change rotation
        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().freezeRotation = true;

        if (lockToRearOfTarget)
            rotateBehind = true;
    }
    void Update()
    {
        if (target == null)
        {
            target = UnityEngine.GameObject.Find(Exchange.authClient.Player.GUID.ToString()) as UnityEngine.GameObject; 
            Debug.Log("Looking for Player");
        }

        if (Input.GetMouseButtonDown(0) && Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            Cursor.lockState = CursorLockMode.Confined;
            mouse = GetCursorPosition();
            Cursor.visible = false;
        }

        if (Input.GetMouseButtonUp(0) && Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
        {
            POINT p = new POINT();
            p.x = Convert.ToInt16(mouse.x);
            p.y = Convert.ToInt16(mouse.y);
            
            SetCursorPos(p.x, p.y);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;            
        }

    }

    [DllImport("user32.dll")]
    public static extern bool GetCursorPos(out POINT lpPoint);

    public static POINT GetCursorPosition()
    {
        POINT lpPoint;
        GetCursorPos(out lpPoint);

        return lpPoint;
    }

    [DllImport("User32.Dll")]
    public static extern long SetCursorPos(int x, int y);
    
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int x;
        public int y;
    }
    

    //Only Move camera after everything else has been updated
    void LateUpdate()
    {
        // Don't do anything if target is not defined
        if (target == null)
            return;
        //pushbuffer
        if (pbuffer > 0)
            pbuffer -= Time.deltaTime;
        if (pbuffer < 0) pbuffer = 0;

        //Sidebuttonmovement
        if ((Input.GetAxis("Toggle Move") != 0) && (pbuffer == 0))
        {
            pbuffer = coolDown;
            mouseSideButton = !mouseSideButton;
        }
        if (mouseSideButton && Input.GetAxis("Vertical") != 0)
            mouseSideButton = false;

        Vector3 vTargetOffset;

        // If either mouse buttons are down, let the mouse govern camera position
        if (GUIUtility.hotControl == 0)
        {
            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                //Check to see if mouse input is allowed on the axis
                if (allowMouseInputX)
                    xDeg += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
                else
                    RotateBehindTarget();
                if (allowMouseInputY)
                    yDeg -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

                //Interrupt rotating behind if mouse wants to control rotation
                if (!lockToRearOfTarget)
                    rotateBehind = false;
            }

            // otherwise, ease behind the target if any of the directional keys are pressed
            else if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0 || rotateBehind || mouseSideButton)
            {
                RotateBehindTarget();
            }
        }
        yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);

        // Set camera rotation
        Quaternion rotation = Quaternion.Euler(yDeg, xDeg, 0);

        // Calculate the desired distance
        desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate * Mathf.Abs(desiredDistance);
        desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
        correctedDistance = desiredDistance;

        // Calculate desired camera position
        vTargetOffset = new Vector3(0, -targetHeight, 0);
        Vector3 position = target.transform.position - (rotation * Vector3.forward * desiredDistance + vTargetOffset);

        // Check for collision using the true target's desired registration point as set by user using height
        RaycastHit collisionHit;
        Vector3 trueTargetPosition = new Vector3(target.transform.position.x, target.transform.position.y + targetHeight, target.transform.position.z);

        // If there was a collision, correct the camera position and calculate the corrected distance
        var isCorrected = false;
        if (Physics.Linecast(trueTargetPosition, position, out collisionHit, collisionLayers))
        {
            // Calculate the distance from the original estimated position to the collision location,
            // subtracting out a safety "offset" distance from the object we hit.  The offset will help
            // keep the camera from being right on top of the surface we hit, which usually shows up as
            // the surface geometry getting partially clipped by the camera's front clipping plane.
            correctedDistance = Vector3.Distance(trueTargetPosition, collisionHit.point) - offsetFromWall;
            isCorrected = true;
        }

        // For smoothing, lerp distance only if either distance wasn't corrected, or correctedDistance is more than currentDistance
        currentDistance = !isCorrected || correctedDistance > currentDistance ? Mathf.Lerp(currentDistance, correctedDistance, Time.deltaTime * zoomDampening) : correctedDistance;

        // Keep within limits
        currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);

        // Recalculate position based on the new currentDistance
        position = target.transform.position - (rotation * Vector3.forward * currentDistance + vTargetOffset);

        //Finally Set rotation and position of camera
        transform.rotation = rotation;
        transform.position = position;
    }

    private void RotateBehindTarget()
    {
        float targetRotationAngle = target.transform.eulerAngles.y;
        float currentRotationAngle = transform.eulerAngles.y;
        xDeg = Mathf.LerpAngle(currentRotationAngle, targetRotationAngle, rotationDampening * Time.deltaTime);

        // Stop rotating behind if not completed
        if (targetRotationAngle == currentRotationAngle)
        {
            if (!lockToRearOfTarget)
                rotateBehind = false;
        }
        else
            rotateBehind = true;

    }


    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f)
            angle += 360f;
        if (angle > 360f)
            angle -= 360f;
        return Mathf.Clamp(angle, min, max);
    }

}