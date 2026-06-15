using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class DummyRockingDoll : MonoBehaviour
{
    [Header("Component References")]
    private Rigidbody rb;

    [Header("Rocking Doll Physics Settings")]
    [Tooltip("The spring force pushing the dummy back to an upright position.")]
    public float uprightSpringForce = 150f;
    
    [Tooltip("Damping to prevent the dummy from wobbling forever. Higher values make it settle faster.")]
    public float uprightDamping = 10f;
    
    [Tooltip("Lowers the center of mass to act like a real Roly-Poly toy. Negative values make it bottom-heavy.")]
    public float centerOfMassYOffset = -1.5f;

    [Header("Knockdown Settings")]
    [Tooltip("Angle in degrees before the dummy is considered 'knocked down' and stops trying to stand.")]
    public float knockdownAngleThreshold = 60f;
    
    [Tooltip("Time in seconds the dummy stays flat on the floor before starting to stand back up.")]
    public float recoveryDelay = 3.0f;

    private Quaternion initialRotation;
    private Vector3 originalCenterOfMass;
    private Vector3 bottomHeavyCenterOfMass;
    private bool isKnockedDown = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // Remember the original standing rotation
        initialRotation = transform.rotation;

        // Save center of mass states
        originalCenterOfMass = rb.centerOfMass;
        bottomHeavyCenterOfMass = originalCenterOfMass + new Vector3(0, centerOfMassYOffset, 0);

        // Start off bottom-heavy to be stable
        rb.centerOfMass = bottomHeavyCenterOfMass;

        // Freeze X and Z position so it doesn't slide, but let Y be free so it can fall to the floor
        rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
    }

    void FixedUpdate()
    {
        // 1. Calculate the rotation needed to return to the initial rotation
        Quaternion deltaRot = initialRotation * Quaternion.Inverse(rb.rotation);
        deltaRot.ToAngleAxis(out float angle, out Vector3 axis);

        // Adjust angle to find the shortest path back
        if (angle > 180f)
            angle -= 360f;

        float absAngle = Mathf.Abs(angle);

        // 2. Check if the dummy was tipped over far enough to be "knocked down"
        if (!isKnockedDown && absAngle > knockdownAngleThreshold)
        {
            StartCoroutine(KnockdownRoutine());
        }

        // 3. If knocked down, we do NOT apply the upright spring force. Let it lie there.
        if (isKnockedDown)
            return;

        // 4. If standing or recovering, apply spring forces to keep it upright
        float angleInRadians = angle * Mathf.Deg2Rad;

        // Skip if perfectly upright or if the axis is mathematically invalid
        if (Mathf.Abs(angleInRadians) < 0.001f || float.IsNaN(axis.x) || float.IsInfinity(axis.x))
            return;

        // Apply a spring formula: Torque = (SpringForce * Error) - (Damping * AngularVelocity)
        Vector3 restoringTorque = axis * (angleInRadians * uprightSpringForce) - (rb.angularVelocity * uprightDamping);
        
        rb.AddTorque(restoringTorque, ForceMode.Acceleration);
    }

    private IEnumerator KnockdownRoutine()
    {
        isKnockedDown = true;
        
        // Reset the center of mass to normal so it acts like a lifeless body on the floor
        // (If we leave it bottom-heavy, it might naturally try to stand up on its own)
        rb.centerOfMass = originalCenterOfMass;

        // Wait for the recovery delay while lying on the ground
        yield return new WaitForSeconds(recoveryDelay);

        // After the delay, restore bottom-heavy physics and let FixedUpdate spring it back up
        rb.centerOfMass = bottomHeavyCenterOfMass;
        isKnockedDown = false;
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
