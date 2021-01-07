using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AirDropRandomFlyerUtility 
{

    public enum FlyingState { RandomFlight, TransitionToAirDrop, Airdrop, }
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
    public float changeTarget = 0f, changeAnim = 0f, timeSinceTarget = 0f, timeSinceAnim = 0f, prevAnim, currentAnim = 0f, prevSpeed, speed, zturn, prevz, turnSpeedBackup;
    public Vector3 rotateTarget, position, direction, velocity, randomizedBase;
    private Quaternion lookRotation;
    [System.NonSerialized] 
    public float distanceFromBase, distanceFromTarget;

    public GameObject targetPosIndicator;

    public AirDropRandomFlyerUtility(string _name)
    {
        name = _name;
        currentState = FlyingState.RandomFlight;

        idleSpeed = 10;
        turnSpeed = 75;
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

    public AirDropRandomFlyerUtility(AirDropRandomFlyerUtility _cloneUtility)
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
        //distanceFromTarget = Vector3.Magnitude(flyingTarget.position - body.position);


        distanceFromTarget = Vector3.Magnitude(targetPosIndicator.transform.position - body.position);

        AllowDrasticTurns();

        UpdateAnimationSpeed();

        UpdateFlyTarget();

        AdjustFlightHeight();

        UpdateAnimTimes();

        UpdateRotation();

        UpdateFlyerMovement(_thisGameObject);


    }

    public void Initialize(GameObject _thisGameObject, GameObject _targetPosIndicator)
    {
        // Inititalize
        _thisGameObject.name = name;

        animator = _thisGameObject.GetComponent<Animator>();
        body = _thisGameObject.GetComponent<Rigidbody>();

        targetPosIndicator = _targetPosIndicator;
        homeTarget = targetPosIndicator.transform;
        flyingTarget = targetPosIndicator.transform;

        turnSpeedBackup = turnSpeed;
        direction = Quaternion.Euler(_thisGameObject.transform.eulerAngles) * (Vector3.forward);

        if (delayStart < 0f) 
            body.velocity = idleSpeed * direction;
    }

    private void AllowDrasticTurns()
    {
        // Allow drastic turns close to base to ensure target can be reached
        if (returnToBase && distanceFromBase < 10f)
        {
            if (turnSpeed != 150f && body.velocity.magnitude != 0f)
            {
                turnSpeedBackup = turnSpeed;
                turnSpeed = 150f;
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
        // Time for a new animation speed
        if (changeAnim < 0f)
        {
            prevAnim = currentAnim;
            currentAnim = ChangeAnim(currentAnim);
            changeAnim = Random.Range(changeAnimEveryFromTo.x, changeAnimEveryFromTo.y);
            timeSinceAnim = 0f;
            prevSpeed = speed;

            if (currentAnim == 0)
                speed = idleSpeed;
            else
                speed = Mathf.Lerp(moveSpeedMinMax.x, moveSpeedMinMax.y, (currentAnim - animSpeedMinMax.x) / (animSpeedMinMax.y - animSpeedMinMax.x));
        }
    }

    private void UpdateFlyTarget()
    {
        // Time for a new target position
        if (changeTarget < 0f)
        {
            rotateTarget = ChangeDirection(body.transform.position);

            if (returnToBase)
                changeTarget = 0.2f;
            else
                changeTarget = Random.Range(changeTargetEveryFromTo.x, changeTargetEveryFromTo.y);

            timeSinceTarget = 0f;
        }
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
        changeTarget -= Time.fixedDeltaTime;
        timeSinceTarget += Time.fixedDeltaTime;
        timeSinceAnim += Time.fixedDeltaTime;
    }

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
    // Select a new animation speed randomly
    private float ChangeAnim(float _currentAnim)
    {
        float newState;
        if (Random.Range(0f, 1f) < idleRatio) newState = 0f;
        else
        {
            newState = Random.Range(animSpeedMinMax.x, animSpeedMinMax.y);
        }
        if (newState != _currentAnim)
        {
            animator.SetFloat("flySpeed", newState);
            if (newState == 0) animator.speed = 1f; else animator.speed = newState;
        }
        return newState;
    }

    // Select a new direction to fly in randomly
    private Vector3 ChangeDirection(Vector3 _currentPosition)
    {
        Vector3 newDir = Vector3.zero;

        switch (currentState)
        {
            case FlyingState.RandomFlight:
                newDir = targetPosIndicator.transform.position - _currentPosition;
                break;
            case FlyingState.TransitionToAirDrop:
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
