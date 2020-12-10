//This script handles moving the player. As the player doesn't move using a navmesh agent, some calculations have to be done to
//get the appropriate level of control.

using UnityEngine;
using System.Collections;
public class PlayerMovement : MonoBehaviour
{
	[HideInInspector] public Vector3 MoveDirection = Vector3.zero;		//The direction the player should move
	[HideInInspector] public Vector3 LookDirection = Vector3.forward;   //The direction the player should face

	[SerializeField] float slowedSpeed = 3f;                              //The slowed speed that the player moves at wehen struck by an enemy
	[SerializeField] float slowedSpeedTimer = 0.5f;						  //The amount of time the player will be slowed when hit by an enemy
	[SerializeField] float baseSpeed = 6f;								  //The default speed that the player moves
	[SerializeField] float currentSpeed = 6f;                             //The current speed that the player moves

	[SerializeField] Animator animator;									//Reference to the animator component
	[SerializeField] Rigidbody rigidBody;								//Reference to the rigidbody component

	bool canMove = true;                                                //Can the player move?

	public IEnumerator defaultHitEffect;
	private void Awake()
	{
		// Place the player on the 'Player' layer and disable collision between players and enemies
		gameObject.layer = Statics.playerLayer;
		Physics.IgnoreLayerCollision(Statics.playerLayer, Statics.enemyLayer);

		// Assign coroutine to a variable
		defaultHitEffect = DefaultHitEffect();
	}

	//Reset() defines the default values for properties in the inspector
	void Reset ()
	{
		//Grab the needed component references
		animator = GetComponent <Animator> ();
		rigidBody = GetComponent <Rigidbody> ();
	}

	//Move with physics so the movement code goes in FixedUpdate()
	void FixedUpdate ()
	{
		//If the player cannot move, leave
		if (!canMove)
			return;

		//Remove any Y value from the desired move direction
		MoveDirection.Set (MoveDirection.x, 0, 0);
		//Move the player using the MovePosition() method of its rigidbody component. This moves the player is a more
		//physically accurate way than transform.Translate() does
		rigidBody.MovePosition (transform.position + MoveDirection.normalized * currentSpeed * Time.deltaTime);

		//Remove any Y & X value from the desired look direction so the player can only look on a 2D plane
		//If the player is not moving, do not update look direction
		if (MoveDirection.x != 0)
			LookDirection.Set(MoveDirection.x, 0, 0);

		//Rotate the player using the MoveRotation() method of its rigidbody component. This rotates the player is a more
		//physically accurate way than transform.Rotate() does. We also use the LookRotation() method of the Quaternion
		//class to help use convert our euler angles into a quaternion
		rigidBody.MoveRotation (Quaternion.LookRotation (LookDirection));
		//Set the IsWalking paramter of the animator. If the move direction has any magnitude (amount), then the player is walking
		animator.SetBool ("IsWalking", MoveDirection.sqrMagnitude > 0);
    }

	// Modifies the player speed by multiplying base speed by parsed value for x seconds
	public IEnumerator DefaultHitEffect()
	{
		// Reset player speed to 0
		currentSpeed = 0;

		//Wait for small delay 
		yield return new WaitForSeconds(0.005f);

		// Modify our current speed variable
		currentSpeed = slowedSpeed;

		// Wait for delay to finish
		yield return new WaitForSeconds(slowedSpeedTimer);
		// Reset the current speed to base speed
		currentSpeed = baseSpeed;
	}

	public void RunHitEffect()
	{
		StopCoroutine(defaultHitEffect);
		defaultHitEffect = DefaultHitEffect();
		StartCoroutine(defaultHitEffect);
	}

	//Called when the player is defeated
	public void Defeated()
	{
		//Player can no longer move
		canMove = false;
	}
}

