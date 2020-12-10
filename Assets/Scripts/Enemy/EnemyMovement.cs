//This script controls the movement of the enemies. The enemy uses a navmesh agent to navigate the scene. 
//The target of the enemy's navigation is received from the GameManager and that allows the enemy to chase 
//whatever the GameManager wants (which happens when allies are spawned). Additionally, the enemy movement
//can be affected by a frost debuff which freezes it in place

using System.Collections;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
	//We want this to be public, but not in the inspector, so we use [HideInInspector]
	[HideInInspector] public FrostDebuff FrostDebuff;               //Reference to a frost debuff that may be attached to the enemy

	[Header("Components")]
	[SerializeField] UnityEngine.AI.NavMeshAgent navMeshAgent;                  //Reference to the navmesh agent component
	[SerializeField] Rigidbody rigidBody;                                       //Reference to the rigidbody component

	[SerializeField] Animator animator;                             //Reference to the animator component
	[Header("Stink Hit Properties")]
	[SerializeField] float runAwayDistance = 10f;                   //How far the enemy runs when hit by a stink attack

	float originalSpeed;                                            //Original movement speed of the enemy (in case they get frozen)
	bool isRunningAway;                                             //Is the enemy running away?
	Vector3 runAwayPosition;                                        //Where the enemy runs if they are hit with a stink attack

	public IEnumerator hitReaction;
	public IEnumerator chasePlayer;
	EnemyHealth myHealth;
	static WaitForSeconds updateDelay = new WaitForSeconds(.5f);    //The delay between updating the navmesh agent (for efficiency). Since
																	//all enemies have the same delay, this is declared as 'static' so all
																	//enemies share the same one (saves on memory)

	public enum EnemyType {ZomBunny, ZomBear, ZomClown, ZomDuck, Hellephant }
	public EnemyType enemyType;

	KillData killData = new KillData();


	void TesterFunctionLogPoints()
	{
		Debug.Log(DataManager.Instance.killDataContainer.killDataList.Count);

		switch (enemyType)
		{
			case EnemyType.ZomBunny:
				killData = new KillData(DataManager.Instance.killDataContainer.killDataList[0]);
					killData.enemyName = Statics.zomBunnyName;
					killData.currentValue += 1;
					killData.expValue = 10;
					killData.totalValue += 10;
					DataManager.Instance.killDataContainer.killDataList[0] = new KillData(killData);
				break;
			case EnemyType.ZomBear:
				killData = new KillData(DataManager.Instance.killDataContainer.killDataList[1]);
				killData.enemyName = Statics.zomBearName;
					killData.currentValue += 1;
					killData.expValue = 10;
					killData.totalValue += 10;
					DataManager.Instance.killDataContainer.killDataList[1] = new KillData(killData);
				
				break;
			case EnemyType.ZomClown:
				killData = new KillData(DataManager.Instance.killDataContainer.killDataList[2]);
				killData.enemyName = Statics.zomClownName;
					killData.currentValue += 1;
					killData.expValue = 10;
					killData.totalValue += 10;
					DataManager.Instance.killDataContainer.killDataList[2] = new KillData(killData);

				break;
			case EnemyType.ZomDuck:
				killData = new KillData(DataManager.Instance.killDataContainer.killDataList[3]);

				killData.enemyName = Statics.zomDuckName;
					killData.currentValue += 1;
					killData.expValue = 10;
					killData.totalValue += 10;
					DataManager.Instance.killDataContainer.killDataList[3] = new KillData(killData);
				break;
			case EnemyType.Hellephant:
				killData = new KillData(DataManager.Instance.killDataContainer.killDataList[4]);

				killData.enemyName = Statics.hellephantName;
					killData.currentValue += 1;
					killData.expValue = 10;
					killData.totalValue += 10;
					DataManager.Instance.killDataContainer.killDataList[4] = new KillData(killData);
				break;
			default:
				break;
		}
	}

	//Reset() defines the default values for properties in the inspector
	void Reset ()
	{
		//Grab references to the needed components
		navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
		animator = GetComponent<Animator> ();
		rigidBody = GetComponent<Rigidbody>();
		myHealth = GetComponent<EnemyHealth>();
		hitReaction = HitReaction(new Vector3(),0, 0);
		chasePlayer = ChasePlayer();
	}

	private void Awake()
	{
		Reset();
	}

	//When this game object is enabled...
	void OnEnable()
	{
		//Enabled the nav mesh agent
		navMeshAgent.enabled = true;
		isRunningAway = false;
		//Start the ChasePlayer coroutine
		StartCoroutine(chasePlayer);
	}

	//This coroutine updates the navmesh agent to chase the player
	public IEnumerator ChasePlayer ()
	{
		//Start by waiting a single frame to give the game a chance to initialize.
		//This is usefull if you start with an enemy in the scene (instead of spawning it)
		yield return null;

		//If there is no GameManager, leave this coroutine permanently (that's 
		//what 'yield break' does
		if (GameManager.Instance == null)
			yield break;

		//While the navmesh agent is enabled...
		while (navMeshAgent.enabled)
		{
			//...get the target from the GameManager...
			Transform target = GameManager.Instance.EnemyTarget;
			//...and if the enemy is running away, head towards the run away position...
			if (isRunningAway)
				navMeshAgent.SetDestination(runAwayPosition);
			//...otherwise, if the target exists, head towards it...
			else if (target != null)
				navMeshAgent.SetDestination(target.position);
			//...finally, wait a time interval before looping
			yield return updateDelay;
		}
	}

	public IEnumerator HitReaction(Vector3 _forceDirection, float _force, float _delay)
	{
		// Remove navmesh target 
		StopCoroutine(chasePlayer);

		// Disable kinematics and navmesh agent
		navMeshAgent.enabled = false;
		rigidBody.isKinematic = false;
		// Add force to the rigidbody to simulate push back 
		rigidBody.AddForce(_forceDirection * _force, ForceMode.Impulse);

		// Wait for period of time
		yield return new WaitForSeconds(_delay);

		// Enable kinematics
		rigidBody.isKinematic = true;

		// If this entity is not dead
		if (!myHealth.isDefeated)
		{
			// Enable navmesh agent
			navMeshAgent.enabled = true;

			// Reassign the navmesh target
			StartCoroutine(chasePlayer);
		}
	}

	//Called when the enemy is defeated and can no longer move
	public void Defeated()
	{
		TesterFunctionLogPoints();


		//Stop coroutines
		StopCoroutine(chasePlayer);
		StopCoroutine(hitReaction);

		//Disable the navmesh agent
		navMeshAgent.enabled = false;

		//If there is a frost debuff attached, remove it
		if (FrostDebuff != null)
			FrostDebuff.gameObject.SetActive (false);
	}

	//This method is called by a frost debuff when it get's frozen in place
	public void Freeze()
	{
		//Stop animating
		animator.enabled = false;
		//Record the navmesh agent's speed (will be needed later)
		originalSpeed = navMeshAgent.speed;
		//Stop the navmesh agent
		navMeshAgent.speed = 	0f;
	}

	//This method is called by a frost debuff when it wears off
	public void UnFreeze()
	{
		//Start animating again
		animator.enabled = true;
		//Set the speed back to it's original value
		navMeshAgent.speed = originalSpeed;
	}

	//This method is called when the enemy is hit by a stink attack
	public void Runaway()
	{
		//The enemy is now running away
		isRunningAway = true;
		//Get a vector from the player's position to the enemy's position
		Vector3 runVector = transform.localPosition - GameManager.Instance.EnemyTarget.position;
		//Use the runVector to run directly away from the player
		runAwayPosition = runVector.normalized * runAwayDistance;
	}

	//This method is called by the StinkHit script when the stink cloud wears off
	public void ComeBack()
	{
		//No longer running away
		isRunningAway = false;
	}
}

