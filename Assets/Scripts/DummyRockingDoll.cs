using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class DummyRockingDoll : MonoBehaviour
{
    [Header("Component References")]
    private Rigidbody rb;

    [Header("Rocking Doll Physics Settings")]
    [Tooltip("The mass of the dummy. Higher values make it heavier and harder to launch into the air.")]
    public float dummyMass = 10f;

    [Tooltip("The spring force pushing the dummy back to an upright position.")]
    public float uprightSpringForce = 1500f;
    
    [Tooltip("Damping to prevent the dummy from wobbling forever. Higher values make it settle faster.")]
    public float uprightDamping = 10f;
    
    [Tooltip("Lowers the center of mass to act like a real Roly-Poly toy. Negative values make it bottom-heavy.")]
    public float centerOfMassYOffset = -1.5f;

    private Quaternion initialRotation;
    private Vector3 originalCenterOfMass;
    private Vector3 bottomHeavyCenterOfMass;

    [Header("Debug Settings")]
    public bool debugMode = true;

    [Header("Independent Input Settings")]
    public bool reactToInputDirectly = true;
    public float punchForce = 100f;
    public float reactionDelay = 0.2f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // Apply our heavier mass so it doesn't fly away easily
        rb.mass = dummyMass;

        // Remember the original standing rotation
        initialRotation = transform.rotation;

        // Save center of mass states
        originalCenterOfMass = rb.centerOfMass;
        
        // Find the exact bottom of the collider to use as the pivot point
        float bottomY = centerOfMassYOffset;
        BoxCollider box = GetComponentInChildren<BoxCollider>();
        CapsuleCollider cap = GetComponentInChildren<CapsuleCollider>();
        if (box != null) bottomY = box.center.y - (box.size.y / 2f);
        else if (cap != null) bottomY = cap.center.y - (cap.height / 2f);
        else {
            Collider col = GetComponentInChildren<Collider>();
            if (col != null && transform.lossyScale.y > 0.001f) 
                bottomY = (col.bounds.min.y - transform.position.y) / transform.lossyScale.y;
        }

        // Instead of freezing positions and fighting physics manually,
        // we create an invisible anchor and use a ConfigurableJoint.
        // This is the native, completely unbreakable way to make a punching bag in Unity.
        
        GameObject anchorGo = new GameObject(gameObject.name + "_Anchor");
        anchorGo.transform.position = transform.position;
        anchorGo.transform.rotation = transform.rotation;
        
        Rigidbody anchorRb = anchorGo.AddComponent<Rigidbody>();
        anchorRb.isKinematic = true;

        ConfigurableJoint joint = gameObject.AddComponent<ConfigurableJoint>();
        joint.connectedBody = anchorRb;
        joint.autoConfigureConnectedAnchor = false;
        
        // Pivot around the bottom center
        // Pivot around the bottom center
        joint.anchor = new Vector3(0, bottomY, 0);

// FIX: Just use the local offset! Do not add the world position.
        joint.connectedAnchor = new Vector3(0, bottomY, 0);

        // Lock all linear movement
        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;

        // Lock twisting around the Y axis
        joint.angularYMotion = ConfigurableJointMotion.Locked;

        // Limit the tilt angle so it physically CANNOT touch the floor
        joint.angularXMotion = ConfigurableJointMotion.Limited;
        joint.angularZMotion = ConfigurableJointMotion.Limited;
        
        SoftJointLimit limit = new SoftJointLimit();
        limit.limit = 50f; // Maximum tilt is 50 degrees
        
        joint.lowAngularXLimit = new SoftJointLimit() { limit = -50f };
        joint.highAngularXLimit = limit;
        joint.angularZLimit = limit;

        // Configure spring for a natural, weighty sway instead of a rigid snap
        JointDrive drive = new JointDrive();
        drive.positionSpring = uprightSpringForce * 0.5f; // Massively scaled down so it rocks naturally
        drive.positionDamper = uprightDamping * 1.5f; // Slightly increased damping to settle smoothly
        drive.maximumForce = float.MaxValue;

        joint.angularXDrive = drive;
        joint.angularYZDrive = drive;
    }

    void FixedUpdate()
    {
        // The ConfigurableJoint now natively handles all spring and upright logic.
        // We no longer need to manually calculate and AddTorque in FixedUpdate.
    }

    void Update()
    {
        if (!reactToInputDirectly) return;

        // Determine "forward" direction relative to the camera to push the dummy away from the view
        Vector3 pushDirection = Camera.main != null ? Camera.main.transform.forward : -transform.forward;
        pushDirection.y = 0;
        pushDirection.Normalize();
        
        Vector3 rightDirection = Camera.main != null ? Camera.main.transform.right : -transform.right;
        rightDirection.y = 0;
        rightDirection.Normalize();

        if (Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.M)) // Jab / Left Jab
        {
            StartCoroutine(ApplyDelayedPunch(pushDirection, reactionDelay));
        }
        else if (Input.GetKeyDown(KeyCode.K)) // Left Hook
        {
            // Left hook hits from the left, pushes dummy right and back
            Vector3 hookDir = (pushDirection + rightDirection * 0.5f).normalized;
            StartCoroutine(ApplyDelayedPunch(hookDir, reactionDelay));
        }
        else if (Input.GetKeyDown(KeyCode.H)) // Right Hook
        {
            // Right hook hits from the right, pushes dummy left and back
            Vector3 hookDir = (pushDirection - rightDirection * 0.5f).normalized;
            StartCoroutine(ApplyDelayedPunch(hookDir, reactionDelay));
        }
        else if (Input.GetKeyDown(KeyCode.P)) // Special
        {
            StartCoroutine(ApplyDelayedPunch(pushDirection, reactionDelay));
        }
    }

    IEnumerator ApplyDelayedPunch(Vector3 direction, float delay)
    {
        yield return new WaitForSeconds(delay);
        // Apply force slightly upwards for a better visual impact
        Vector3 forceDir = (direction + Vector3.up * 0.2f).normalized;
        TakePunch(forceDir * punchForce, transform.position + Vector3.up * 1.5f);
    }



    /// <summary>
    /// Call this method from your player punch/attack script when the dummy is hit.
    /// </summary>
    /// <param name="force">The directional force of the punch (e.g. punchDirection * punchPower).</param>
    /// <param name="hitPoint">The world space point where the punch landed.</param>
    public void TakePunch(Vector3 force, Vector3 hitPoint)
    {
        rb.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
    }
}