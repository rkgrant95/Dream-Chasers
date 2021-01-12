using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CarePackageCarrierUtility 
{

    public enum FlyingState { RandomFlight, Airdrop, }
    public FlyingState currentState;

    private string name;

    [Tooltip("The base travel speed of the flyer")]
    public float idleSpeed;
    [Tooltip("The turn speed of the flyer")]
    public float turnSpeed;
    public float switchSeconds;
    public float idleRatio;

    public Vector2 animSpeedMinMax;
    public Vector2 moveSpeedMinMax;
    public Vector2 changeAnimEveryFromTo;
    public Vector2 changeTargetEveryFromTo;

    [Tooltip("The transform the flyer should travel to if return to base")]
    public Transform homeTarget;
    [Tooltip("The target transform to fly around")]
    public Transform flyingTarget;
    
    [Tooltip("The min & max radius around the flying target that the flyer can travel X & Z")]
    public Vector2 radiusMinMax;
    [Tooltip("The min & max flying height")]
    public Vector2 yMinMax;

    [Tooltip("Should the flyer return to the home target?")]
    public bool returnToBase = false;
    public float randomBaseOffset = 5;
    public float delayStart = 0f;

    [SerializeField]
    private Animator animator;
    [SerializeField]
    private Rigidbody body;

    [System.NonSerialized]
    public float changeAnim = 0f, timeSinceAnim = 0f, prevAnim, currentAnim = 0f, prevSpeed, speed, zturn, prevz, turnSpeedBackup;
    public Vector3 rotateTarget, position, direction, velocity, randomizedBase;
    private Quaternion lookRotation;
    [System.NonSerialized] 
    public float distanceFromBase;

    public CarePackageFlightIndicator targetPosIndicator;
    public bool changeFlightOnTimer;
    public CarePackageCarrierUtility(string _name)
    {
        name = _name;
        currentState = FlyingState.RandomFlight;

        idleSpeed = 10;
        turnSpeed = 100;
        switchSeconds = 3;
        idleRatio = 0.3f;

        animSpeedMinMax = new Vector2(0.5f, 2);
        moveSpeedMinMax = new Vector2(10, 20);
        changeAnimEveryFromTo = new Vector2(2, 4);
        changeTargetEveryFromTo = new Vector2(3, 8);

        radiusMinMax = new Vector2(-60, 60);
        yMinMax = new Vector2(0, 50);

        delayStart = 0;
        randomBaseOffset = 2.5f;

        returnToBase = true;

        /*
        if (targetPosIndicator)
        {
            homeTarget = targetPosIndicator.transform;
            flyingTarget = targetPosIndicator.transform;
        }
        */

    }

    public CarePackageCarrierUtility(CarePackageCarrierUtility _cloneUtility)
    {
        name = _cloneUtility.name;

        idleSpeed = _cloneUtility.idleSpeed;
        turnSpeed = _cloneUtility.turnSpeed;
        switchSeconds = _cloneUtility.switchSeconds;
        idleRatio = _cloneUtility.idleRatio;

        animSpeedMinMax = _cloneUtility.animSpeedMinMax;
        moveSpeedMinMax = _cloneUtility.moveSpeedMinMax;
        changeAnimEveryFromTo = _cloneUtility.changeAnimEveryFromTo;
        changeTargetEveryFromTo = _cloneUtility.changeTargetEveryFromTo;

        radiusMinMax = _cloneUtility.radiusMinMax;
        yMinMax = _cloneUtility.yMinMax;

        delayStart = _cloneUtility.delayStart;
        randomBaseOffset = _cloneUtility.randomBaseOffset;
        returnToBase = _cloneUtility.returnToBase;

        homeTarget = _cloneUtility.homeTarget;
        flyingTarget = _cloneUtility.flyingTarget;
    }

    /// <summary>
    /// Fixed update for this game object
    /// </summary>
    /// <param name="_thisGameObject"></param>
    public void FixedTick(GameObject _thisGameObject)
    {
        // Wait if start should be delayed (useful to add small differences in large flocks)
        if (delayStart > 0f)
        {
            delayStart -= Time.fixedDeltaTime;
            return;
        }

        // Calculate distances
        distanceFromBase = Vector3.Magnitude(randomizedBase - body.position);

        AllowDrasticTurns();

        UpdateAnimationSpeed();

        UpdateFlyTarget();

        AdjustFlightHeight();

        UpdateAnimTimes();

        UpdateRotation();

        UpdateFlyerMovement(_thisGameObject);


    }

    /// <summary>
    /// Initialize this game object
    /// </summary>
    /// <param name="_thisGameObject"></param>
    /// <param name="_targetPosIndicator"></param>
    public void Initialize(GameObject _thisGameObject, CarePackageFlightIndicator _targetPosIndicator)
    {
        // Inititalize
        _thisGameObject.name = name;

        animator = _thisGameObject.GetComponentInChildren<Animator>();
        body = _thisGameObject.GetComponent<Rigidbody>();

        targetPosIndicator = _targetPosIndicator;
        homeTarget = targetPosIndicator.transform;
        flyingTarget = targetPosIndicator.transform;

        turnSpeedBackup = turnSpeed;
        direction = Quaternion.Euler(_thisGameObject.transform.eulerAngles) * (Vector3.forward);

        if (delayStart < 0f) 
            body.velocity = idleSpeed * direction;

        changeAnim = 10;
    }

    /// <summary>
    /// Doubles the turn speed of the flyer in cases where they may exceed bounds if their turn speed is too slow
    /// </summary>
    private void AllowDrasticTurns()
    {
        // Allow drastic turns close to base to ensure target can be reached
        if (returnToBase && distanceFromBase < 10f)
        {
            if (turnSpeed != 200f && body.velocity.magnitude != 0f)
            {
                turnSpeedBackup = turnSpeed;
                turnSpeed = 200f;
            }
            else if (distanceFromBase <= 2f)
            {
                body.velocity = Vector3.zero;
                turnSpeed = turnSpeedBackup;
                return;
            }
        }
    }

    private void UpdateAnimationSpeed()
    {
        prevSpeed = speed;
        speed = idleSpeed;

        if (changeAnim < 0)
        {
            int rand = Random.Range(0, 5);
            bool reverse = Random.value > 0.5f;

            animator.SetBool(Statics.ReverseAnim, reverse);

            if (rand < 1)
                animator.SetTrigger(Statics.BarrelRollAnim);
            else
                animator.SetTrigger(Statics.TurbulenceAnim);

            changeAnim = 5;
        }
    }

    /// <summary>
    /// Updates the flyer target
    /// </summary>
    private void UpdateFlyTarget()
    {
        rotateTarget = ChangeDirection(body.transform.position);

        /*
        if (returnToBase)

        */
    }

    private void AdjustFlightHeight()
    {
        // Turn when approaching height limits
        // ToDo: Adjust limit and "exit direction" by object's direction and velocity, instead of the 10f and 1f - this works in my current scenario/scale
        if (body.transform.position.y < yMinMax.x + 10f || body.transform.position.y > yMinMax.y - 10f)
        {
            if (body.transform.position.y < yMinMax.x + 10f)
                rotateTarget.y = 1f;
            else
                rotateTarget.y = -1f;
        }

        //body.transform.Rotate(0f, 0f, -prevz, Space.Self); // If required to make Quaternion.LookRotation work correctly, but it seems to be fine
        zturn = Mathf.Clamp(Vector3.SignedAngle(rotateTarget, direction, Vector3.up), -45f, 45f);

    }

    private void UpdateAnimTimes()
    {
        // Update times
        changeAnim -= Time.fixedDeltaTime;
        timeSinceAnim += Time.fixedDeltaTime;
    }

    /// <summary>
    /// Updates the flyer rotation 
    /// </summary>
    private void UpdateRotation()
    {
        // Rotate towards target
        if (rotateTarget != Vector3.zero)
            lookRotation = Quaternion.LookRotation(rotateTarget, Vector3.up);

        Vector3 rotation = Quaternion.RotateTowards(body.transform.rotation, lookRotation, turnSpeed * Time.fixedDeltaTime).eulerAngles;
        body.transform.eulerAngles = rotation;

        // Rotate on z-axis to tilt body towards turn direction
        float temp = prevz;

        if (prevz < zturn)
            prevz += Mathf.Min(turnSpeed * Time.fixedDeltaTime, zturn - prevz);
        else if
            (prevz >= zturn) prevz -= Mathf.Min(turnSpeed * Time.fixedDeltaTime, prevz - zturn);

        // Min and max rotation on z-axis - can also be parameterized
        prevz = Mathf.Clamp(prevz, -45f, 45f);

        // Remove temp if transform is rotated back earlier in FixedUpdate
        body.transform.Rotate(0f, 0f, prevz - temp, Space.Self);
    }

    /// <summary>
    /// Updates flyer movement velocity with speed & direction
    /// </summary>
    /// <param name="_thisGameObject"></param>
    private void UpdateFlyerMovement(GameObject _thisGameObject)
    {
        // Move flyer
        direction = Quaternion.Euler(_thisGameObject.transform.eulerAngles) * Vector3.forward;

        if (returnToBase && distanceFromBase < idleSpeed)
            body.velocity = Mathf.Min(idleSpeed, distanceFromBase) * direction;
        else
            body.velocity = Mathf.Lerp(prevSpeed, speed, Mathf.Clamp(timeSinceAnim / switchSeconds, 0f, 1f)) * direction;

        // Hard-limit the height, in case the limit is breached despite of the turnaround attempt
        if (body.transform.position.y < yMinMax.x || body.transform.position.y > yMinMax.y)
        {
            position = body.transform.position;
            position.y = Mathf.Clamp(position.y, yMinMax.x, yMinMax.y);
            body.transform.position = position;
        }
    }

    /// <summary>
    /// Returns a new direction for the flyer to travel in 
    /// </summary>
    /// <param name="_currentPosition"></param>
    /// <returns></returns>
    private Vector3 ChangeDirection(Vector3 _currentPosition)
    {
        Vector3 newDir = Vector3.zero;

        switch (currentState)
        {
            case FlyingState.RandomFlight:
                newDir = targetPosIndicator.transform.position - _currentPosition;
                break;
            case FlyingState.Airdrop:
                newDir = targetPosIndicator.transform.position - _currentPosition;
                break;
            default:
                break;
        }

        return newDir.normalized;
    }
}
