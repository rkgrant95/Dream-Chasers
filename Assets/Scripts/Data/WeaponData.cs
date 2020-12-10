using SaveSystem;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

[System.Serializable]
public class WeaponData
{
	public WeaponData() { }

	public WeaponData(WeaponData _weaponData)
	{
		// Weapon Type
		this.type = _weaponData.type;                                               // Which weapon system should be used
		this.gameObject = _weaponData.gameObject;                                   // This weapons game object

		// Auto
		this.auto = _weaponData.auto;                                               // How does this weapon fire - semi-auto or full-auto

		this.shooterAIEnabled = _weaponData.shooterAIEnabled;                       // Enable features compatible with Shooter AI by Gateway Games
		this.bloodyMessEnabled = _weaponData.bloodyMessEnabled;                     // Enable features compatible with Bloody Mess by Heavy Diesel Softworks
		this.weaponType = _weaponData.weaponType;                                   // Bloody mess property

		// General
		this.playerWeapon = _weaponData.playerWeapon;                               // Whether or not this is a player's weapon as opposed to an AI's weapon
		this.weaponModel = _weaponData.weaponModel;                                 // The actual mesh for this weapon
		this.raycastStartSpot = _weaponData.raycastStartSpot;                       // The spot that the raycasting weapon system should use as an origin for rays
		this.delayBeforeFire = _weaponData.delayBeforeFire;                         // An optional delay that causes the weapon to fire a specified amount of time after it normally would (0 for no delay)

		// Warmup
		this.warmup = _weaponData.warmup;                                           // Whether or not the shot will be allowed to "warmup" before firing - allows power to increase when the player holds down fire button longer
																					// Only works on semi-automatic raycast and projectile weapons
		this.maxWarmup = _weaponData.maxWarmup;                                     // The maximum amount of time a warmup can have any effect on power, etc.
		this.multiplyForce = _weaponData.multiplyForce;                             // Whether or not the initial force of the projectile should be multiplied based on warmup heat value - only for projectiles
		this.multiplyPower = _weaponData.multiplyPower;                             // Whether or not the damage of the projectile should be multiplied based on warmup heat value - only for projectiles
																					// Also only has an effect on projectiles using the direct damage system - an example would be an arrow that causes more damage the longer you pull back your bow
		this.powerMultiplier = _weaponData.powerMultiplier;                         // The multiplier by which the warmup can affect weapon power; power = power * (heat * powerMultiplier)
		this.initialForceMultiplier = _weaponData.initialForceMultiplier;           // The multiplier by which the warmup can affect the initial force, assuming a projectile system
		this.allowCancel = _weaponData.allowCancel;                                 // If true, the player can cancel this warmed up shot by pressing the "Cancel" input button, otherwise a shot will be fired when the player releases the fire key
		this.heat = _weaponData.heat;                                               // The amount of time the weapon has been warming up, can be in the range of (0, maxWarmup)

		// Beam
		this.reflect = _weaponData.reflect;                                         // Whether or not the laser beam should reflect off of certain surfaces
		this.reflectionMaterial = _weaponData.reflectionMaterial;                   // The material that reflects the laser.  If this is null, the laser will reflect off all surfaces
		this.maxReflections = _weaponData.maxReflections;                           // The maximum number of times the laser beam can reflect off surfaces.  Without this limit, the system can possibly become stuck in an infinite loop
		this.beamTypeName = _weaponData.beamTypeName;                               // This is the name that will be used as the name of the instantiated beam effect.  It is not necessary.
		this.maxBeamHeat = _weaponData.maxBeamHeat;                                 // The time, in seconds, that the beam will last
		this.infiniteBeam = _weaponData.infiniteBeam;                               // If this is true the beam will never overheat (equivalent to infinite ammo)
		this.beamMaterial = _weaponData.beamMaterial;                               // The material that will be used in the beam (line renderer.)  This should be a particle material
		this.beamColor = _weaponData.beamColor;                                     // The color that will be used to tint the beam material
		this.startBeamWidth = _weaponData.startBeamWidth;                           // The width of the beam on the starting side
		this.endBeamWidth = _weaponData.endBeamWidth;                               // The width of the beam on the ending side
		this.beamHeat = _weaponData.beamHeat;                                       // Timer to keep track of beam warmup and cooldown
		this.coolingDown = _weaponData.coolingDown;                                 // Whether or not the beam weapon is currently cooling off.  This is used to make sure the weapon isn't fired when it's too close to the maximum heat level
		this.beamGO = _weaponData.beamGO;                                           // The reference to the instantiated beam GameObject
		this.beaming = _weaponData.beaming;                                         // Whether or not the weapon is currently firing a beam - used to make sure StopBeam() is called after the beam is no longer being fired

		// Power
		this.power = _weaponData.power;                                             // The amount of power this weapon has (how much damage it can cause) (if the type is raycast or beam)
		this.forceMultiplier = _weaponData.forceMultiplier;                         // Multiplier used to change the amount of force applied to rigid bodies that are shot
		this.beamPower = _weaponData.beamPower;                                     // Used to determine damage caused by beam weapons.  This will be much lower because this amount is applied to the target every frame while firing
		this.hitReactionTime = _weaponData.hitReactionTime;                         // Used to determine how long the target's rigid body will be active for to apply force. Should generally be low to avoid strange interactions

		// Range
		this.range = _weaponData.range;                                             // How far this weapon can shoot (for raycast and beam)

		// Rate of Fire
		this.rateOfFire = _weaponData.rateOfFire;                                   // The number of rounds this weapon fires per second
		this.actualROF = _weaponData.actualROF;                                     // The frequency between shots based on the rateOfFire
		this.fireTimer = _weaponData.fireTimer;                                     // Timer used to fire at a set frequency

		// Ammo
		this.infiniteAmmo = _weaponData.infiniteAmmo;                               // Whether or not this weapon should have unlimited ammo
		this.ammoCapacity = _weaponData.ammoCapacity;                               // The number of rounds this weapon can fire before it has to reload
		this.shotPerRound = _weaponData.shotPerRound;                               // The number of "bullets" that will be fired on each round.  Usually this will be 1, but set to a higher number for things like shotguns with spread
		this.currentAmmo = _weaponData.currentAmmo;                                 // How much ammo the weapon currently has
		this.reloadTime = _weaponData.reloadTime;                                   // How much time it takes to reload the weapon
		this.showCurrentAmmo = _weaponData.showCurrentAmmo;                         // Whether or not the current ammo should be displayed in the GUI
		this.reloadAutomatically = _weaponData.reloadAutomatically;                 // Whether or not the weapon should reload automatically when out of ammo

		// Accuracy
		this.accuracy = _weaponData.accuracy;                                       // How accurate this weapon is on a scale of 0 to 100
		this.currentAccuracy = _weaponData.currentAccuracy;                         // Holds the current accuracy.  Used for varying accuracy based on speed, etc.
		this.accuracyDropPerShot = _weaponData.accuracyDropPerShot;                 // How much the accuracy will decrease on each shot
		this.accuracyRecoverRate = _weaponData.accuracyRecoverRate;                 // How quickly the accuracy recovers after each shot (value between 0 and 1)

		// Burst
		this.burstRate = _weaponData.burstRate;                                     // The number of shots fired per each burst
		this.burstPause = _weaponData.burstPause;                                   // The pause time between bursts
		this.burstCounter = _weaponData.burstCounter;                               // Counter to keep track of how many shots have been fired per burst
		this.burstTimer = _weaponData.burstTimer;                                   // Timer to keep track of how long the weapon has paused between bursts

		// Recoil
		this.recoil = _weaponData.recoil;                                           // Whether or not this weapon should have recoil
		this.recoilKickBackMin = _weaponData.recoilKickBackMin;                     // The minimum distance the weapon will kick backward when fired
		this.recoilKickBackMax = _weaponData.recoilKickBackMax;                     // The maximum distance the weapon will kick backward when fired
		this.recoilRotationMin = _weaponData.recoilRotationMin;                     // The minimum rotation the weapon will kick when fired
		this.recoilRotationMax = _weaponData.recoilRotationMax;                     // The maximum rotation the weapon will kick when fired
		this.recoilRecoveryRate = _weaponData.recoilRecoveryRate;                   // The rate at which the weapon recovers from the recoil displacement

		// Effects
		this.spitShells = _weaponData.spitShells;                                   // Whether or not this weapon should spit shells out of the side
		this.shell = _weaponData.shell;                                             // A shell prop to spit out the side of the weapon
		this.shellSpitForce = _weaponData.shellSpitForce;                           // The force with which shells will be spit out of the weapon
		this.shellForceRandom = _weaponData.shellForceRandom;                       // The variant by which the spit force can change + or - for each shot
		this.shellSpitTorqueX = _weaponData.shellSpitTorqueX;                       // The torque with which the shells will rotate on the x axis
		this.shellSpitTorqueY = _weaponData.shellSpitTorqueY;                       // The torque with which the shells will rotate on the y axis
		this.shellTorqueRandom = _weaponData.shellTorqueRandom;                     // The variant by which the spit torque can change + or - for each shot
		this.shellSpitPosition = _weaponData.shellSpitPosition;                     // The spot where the weapon should spit shells from
		this.makeMuzzleEffects = _weaponData.makeMuzzleEffects;                     // Whether or not the weapon should make muzzle effects
		this.muzzleEffects = _weaponData.muzzleEffects;                             // Effects to appear at the muzzle of the gun (muzzle flash, smoke, etc.)
		this.muzzleEffectsPosition = _weaponData.muzzleEffectsPosition;             // The spot where the muzzle effects should appear from
		this.makeHitEffects = _weaponData.makeHitEffects;                           // Whether or not the weapon should make hit effects
		this.hitEffects = _weaponData.hitEffects;                                   // Effects to be displayed where the "bullet" hit

		// Bullet Holes
		this.makeBulletHoles = _weaponData.makeBulletHoles;                         // Whether or not bullet holes should be made
		this.bhSystem = _weaponData.bhSystem;                                       // What condition the dynamic bullet holes should be based off
		this.bulletHolePoolNames = _weaponData.bulletHolePoolNames;                 // A list of strings holding the names of bullet hole pools in the scene
		this.defaultBulletHolePoolNames = _weaponData.defaultBulletHolePoolNames;   // A list of strings holding the names of default bullet hole pools in the scene
		this.bulletHoleGroups = _weaponData.bulletHoleGroups;                       // A list of bullet hole groups.  Each one holds a tag for GameObjects that might be hit, as well as a corresponding bullet hole
		this.defaultBulletHoles = _weaponData.defaultBulletHoles;                   // A list of default bullet holes to be instantiated when none of the custom parameters are met
		this.bulletHoleExceptions = _weaponData.bulletHoleExceptions;               // A list of SmartBulletHoleGroup objects that defines conditions for when no bullet hole will be instantiated.
																					// In other words, the bullet holes in the defaultBulletHoles list will be instantiated on any surface except for
																					// the ones specified in this list.

		// Crosshairs
		this.showCrosshair = _weaponData.showCrosshair;                             // Whether or not the crosshair should be displayed
		this.crosshairTexture = _weaponData.crosshairTexture;                       // The texture used to draw the crosshair
		this.crosshairLength = _weaponData.crosshairLength;                         // The length of each crosshair line
		this.crosshairWidth = _weaponData.crosshairWidth;                           // The width of each crosshair line
		this.startingCrosshairSize = _weaponData.startingCrosshairSize;             // The gap of space (in pixels) between the crosshair lines (for weapon inaccuracy)
		this.currentCrosshairSize = _weaponData.currentCrosshairSize;               // The gap of space between crosshair lines that is updated based on weapon accuracy in realtime

		// Audio
		this.fireSound = _weaponData.fireSound;                                     // Sound to play when the weapon is fired
		this.reloadSound = _weaponData.reloadSound;                                 // Sound to play when the weapon is reloading
		this.dryFireSound = _weaponData.dryFireSound;                               // Sound to play when the user tries to fire but is out of ammo

		// Other
		this.canFire = _weaponData.canFire;                                         // Whether or not the weapon can currently fire (used for semi-auto weapons)
		this.hitEnemy = _weaponData.hitEnemy;                                       // Logs the latest enemy that was hit by this weapon
		this.killDataContainer = _weaponData.killDataContainer;                     // Tracks the enemies that were killed by this weapon
	}

	#region Weapon Data Variables
	//[XmlEnum]
	public WeaponType type = WeaponType.Projectile;     // Which weapon system should be used

	[XmlIgnore]
	public GameObject gameObject;                      // This weapons game object
	[XmlIgnore]
	public MonoBehaviour monoBehaviour;				// This weapons monobehaviour component
	// External Tools
	public bool shooterAIEnabled = false;               // Enable features compatible with Shooter AI by Gateway Games
	public bool bloodyMessEnabled = false;              // Enable features compatible with Bloody Mess by Heavy Diesel Softworks
	public int weaponType = 0;                          // Bloody mess property

	// Auto
	public Auto auto = Auto.Full;                       // How does this weapon fire - semi-auto or full-auto

	// General
	public bool playerWeapon = true;                    // Whether or not this is a player's weapon as opposed to an AI's weapon
	[XmlIgnore]
	public GameObject weaponModel;                      // The actual mesh for this weapon
	[XmlIgnore]
	public Transform raycastStartSpot;                  // The spot that the raycasting weapon system should use as an origin for rays
	public float delayBeforeFire = 0.0f;                // An optional delay that causes the weapon to fire a specified amount of time after it normally would (0 for no delay)

	// Warmup
	public bool warmup = false;                         // Whether or not the shot will be allowed to "warmup" before firing - allows power to increase when the player holds down fire button longer
														// Only works on semi-automatic raycast and projectile weapons
	public float maxWarmup = 2.0f;                      // The maximum amount of time a warmup can have any effect on power, etc.
	public bool multiplyForce = true;                   // Whether or not the initial force of the projectile should be multiplied based on warmup heat value - only for projectiles
	public bool multiplyPower = false;                  // Whether or not the damage of the projectile should be multiplied based on warmup heat value - only for projectiles
														// Also only has an effect on projectiles using the direct damage system - an example would be an arrow that causes more damage the longer you pull back your bow
	public float powerMultiplier = 1.0f;                // The multiplier by which the warmup can affect weapon power; power = power * (heat * powerMultiplier)
	public float initialForceMultiplier = 1.0f;         // The multiplier by which the warmup can affect the initial force, assuming a projectile system
	public bool allowCancel = false;                    // If true, the player can cancel this warmed up shot by pressing the "Cancel" input button, otherwise a shot will be fired when the player releases the fire key
	private float heat = 0.0f;                          // The amount of time the weapon has been warming up, can be in the range of (0, maxWarmup)

	// Projectile
	[XmlIgnore]
	public GameObject projectile;                       // The projectile to be launched (if the type is projectile)
	[XmlIgnore]
	public Transform projectileSpawnSpot;               // The spot where the projectile should be instantiated

	// Beam
	public bool reflect = true;                         // Whether or not the laser beam should reflect off of certain surfaces
	[XmlIgnore]
	public Material reflectionMaterial;                 // The material that reflects the laser.  If this is null, the laser will reflect off all surfaces
	public int maxReflections = 5;                      // The maximum number of times the laser beam can reflect off surfaces.  Without this limit, the system can possibly become stuck in an infinite loop
	public string beamTypeName = "laser_beam";          // This is the name that will be used as the name of the instantiated beam effect.  It is not necessary.
	public float maxBeamHeat = 1.0f;                    // The time, in seconds, that the beam will last
	public bool infiniteBeam = false;                   // If this is true the beam will never overheat (equivalent to infinite ammo)
	[XmlIgnore]
	public Material beamMaterial;                       // The material that will be used in the beam (line renderer.)  This should be a particle material
	public Color beamColor = Color.red;                 // The color that will be used to tint the beam material
	public float startBeamWidth = 0.5f;                 // The width of the beam on the starting side
	public float endBeamWidth = 1.0f;                   // The width of the beam on the ending side
	private float beamHeat = 0.0f;                      // Timer to keep track of beam warmup and cooldown
	private bool coolingDown = false;                   // Whether or not the beam weapon is currently cooling off.  This is used to make sure the weapon isn't fired when it's too close to the maximum heat level
	[XmlIgnore]
	private GameObject beamGO;                          // The reference to the instantiated beam GameObject
	private bool beaming = false;                       // Whether or not the weapon is currently firing a beam - used to make sure StopBeam() is called after the beam is no longer being fired

	// Power
	public float power = 80.0f;                         // The amount of power this weapon has (how much damage it can cause) (if the type is raycast or beam)
	public float forceMultiplier = 10.0f;               // Multiplier used to change the amount of force applied to rigid bodies that are shot
	public float beamPower = 1.0f;                      // Used to determine damage caused by beam weapons.  This will be much lower because this amount is applied to the target every frame while firing
	public float hitReactionTime = 0.1f;                // Used to determine how long the target's rigid body will be active for to apply force. Should generally be low to avoid strange interactions

	// Range
	public float range = 9999.0f;                       // How far this weapon can shoot (for raycast and beam)

	// Rate of Fire
	public float rateOfFire = 10;                       // The number of rounds this weapon fires per second
	private float actualROF;                            // The frequency between shots based on the rateOfFire
	private float fireTimer;                            // Timer used to fire at a set frequency

	// Ammo
	public bool infiniteAmmo = false;                   // Whether or not this weapon should have unlimited ammo
	public int ammoCapacity = 12;                       // The number of rounds this weapon can fire before it has to reload
	public int shotPerRound = 1;                        // The number of "bullets" that will be fired on each round.  Usually this will be 1, but set to a higher number for things like shotguns with spread
	private int currentAmmo;                            // How much ammo the weapon currently has
	public float reloadTime = 2.0f;                     // How much time it takes to reload the weapon
	public bool showCurrentAmmo = true;                 // Whether or not the current ammo should be displayed in the GUI
	public bool reloadAutomatically = true;             // Whether or not the weapon should reload automatically when out of ammo

	// Accuracy
	public float accuracy = 80.0f;                      // How accurate this weapon is on a scale of 0 to 100
	private float currentAccuracy;                      // Holds the current accuracy.  Used for varying accuracy based on speed, etc.
	public float accuracyDropPerShot = 1.0f;            // How much the accuracy will decrease on each shot
	public float accuracyRecoverRate = 0.1f;            // How quickly the accuracy recovers after each shot (value between 0 and 1)

	// Burst
	public int burstRate = 3;                           // The number of shots fired per each burst
	public float burstPause = 0.0f;                     // The pause time between bursts
	private int burstCounter = 0;                       // Counter to keep track of how many shots have been fired per burst
	private float burstTimer = 0.0f;                    // Timer to keep track of how long the weapon has paused between bursts

	// Recoil
	public bool recoil = true;                          // Whether or not this weapon should have recoil
	public float recoilKickBackMin = 0.1f;              // The minimum distance the weapon will kick backward when fired
	public float recoilKickBackMax = 0.3f;              // The maximum distance the weapon will kick backward when fired
	public float recoilRotationMin = 0.1f;              // The minimum rotation the weapon will kick when fired
	public float recoilRotationMax = 0.25f;             // The maximum rotation the weapon will kick when fired
	public float recoilRecoveryRate = 0.01f;            // The rate at which the weapon recovers from the recoil displacement

	// Effects
	public bool spitShells = false;                     // Whether or not this weapon should spit shells out of the side
	[XmlIgnore]
	public GameObject shell;                            // A shell prop to spit out the side of the weapon
	public float shellSpitForce = 1.0f;                 // The force with which shells will be spit out of the weapon
	public float shellForceRandom = 0.5f;               // The variant by which the spit force can change + or - for each shot
	public float shellSpitTorqueX = 0.0f;               // The torque with which the shells will rotate on the x axis
	public float shellSpitTorqueY = 0.0f;               // The torque with which the shells will rotate on the y axis
	public float shellTorqueRandom = 1.0f;              // The variant by which the spit torque can change + or - for each shot
	[XmlIgnore]
	public Transform shellSpitPosition;                 // The spot where the weapon should spit shells from
	public bool makeMuzzleEffects = true;               // Whether or not the weapon should make muzzle effects
	[XmlIgnore]
	public GameObject[] muzzleEffects = new GameObject[] { null };                      // Effects to appear at the muzzle of the gun (muzzle flash, smoke, etc.)
	[XmlIgnore]
	public Transform muzzleEffectsPosition;												// The spot where the muzzle effects should appear from
	public bool makeHitEffects = true;													// Whether or not the weapon should make hit effects
	[XmlIgnore]
	public GameObject[] hitEffects = new GameObject[] { null };                         // Effects to be displayed where the "bullet" hit

	// Bullet Holes
	public bool makeBulletHoles = true;                                                             // Whether or not bullet holes should be made
	public BulletHoleSystem bhSystem = BulletHoleSystem.Tag;										// What condition the dynamic bullet holes should be based off
	public List<string> bulletHolePoolNames = new List<string>();									// A list of strings holding the names of bullet hole pools in the scene
	public List<string> defaultBulletHolePoolNames = new List<string>();                            // A list of strings holding the names of default bullet hole pools in the scene
	[XmlIgnore]
	public List<SmartBulletHoleGroup> bulletHoleGroups = new List<SmartBulletHoleGroup>();          // A list of bullet hole groups.  Each one holds a tag for GameObjects that might be hit, as well as a corresponding bullet hole
	[XmlIgnore]
	public List<BulletHolePool> defaultBulletHoles = new List<BulletHolePool>();                    // A list of default bullet holes to be instantiated when none of the custom parameters are met
	[XmlIgnore]
	public List<SmartBulletHoleGroup> bulletHoleExceptions = new List<SmartBulletHoleGroup>();      // A list of SmartBulletHoleGroup objects that defines conditions for when no bullet hole will be instantiated.
																									// In other words, the bullet holes in the defaultBulletHoles list will be instantiated on any surface except for
																									// the ones specified in this list.

	// Crosshairs
	public bool showCrosshair = true;                   // Whether or not the crosshair should be displayed
	[XmlIgnore]
	public Texture2D crosshairTexture;                  // The texture used to draw the crosshair
	public int crosshairLength = 10;                    // The length of each crosshair line
	public int crosshairWidth = 4;                      // The width of each crosshair line
	public float startingCrosshairSize = 10.0f;         // The gap of space (in pixels) between the crosshair lines (for weapon inaccuracy)
	private float currentCrosshairSize;                 // The gap of space between crosshair lines that is updated based on weapon accuracy in realtime

	// Audio
	[XmlIgnore]
	public AudioClip fireSound;                         // Sound to play when the weapon is fired
	[XmlIgnore]
	public AudioClip reloadSound;                       // Sound to play when the weapon is reloading
	[XmlIgnore]
	public AudioClip dryFireSound;                      // Sound to play when the user tries to fire but is out of ammo

	// Other
	private bool canFire = true;                        // Whether or not the weapon can currently fire (used for semi-auto weapons)

	[XmlIgnore]
	EnemyMovement hitEnemy;
	KillDataContainer killDataContainer = new KillDataContainer();                        // Tracks the enemies that were killed by this weapon
    #endregion


    #region Weapon Data Functions

	/// <summary>
	/// Initializes basic variables 
	/// </summary>
	public void Initialize()
	{

		// Calculate the actual ROF to be used in the weapon systems.  The rateOfFire variable is
		// designed to make it easier on the user - it represents the number of rounds to be fired
		// per second.  Here, an actual ROF decimal value is calculated that can be used with timers.
		if (rateOfFire != 0)
			actualROF = 1.0f / rateOfFire;
		else
			actualROF = 0.01f;

		// Initialize the current crosshair size variable to the starting value specified by the user
		currentCrosshairSize = startingCrosshairSize;

		// Make sure the fire timer starts at 0
		fireTimer = 0.0f;

		// Start the weapon off with a full magazine
		currentAmmo = ammoCapacity;

		// Give this weapon an audio source component if it doesn't already have one
		if (gameObject.GetComponent<AudioSource>() == null)
		{
			gameObject.AddComponent(typeof(AudioSource));
		}

		// Make sure raycastStartSpot isn't null
		if (raycastStartSpot == null)
			raycastStartSpot = gameObject.transform;

		// Make sure muzzleEffectsPosition isn't null
		if (muzzleEffectsPosition == null)
			muzzleEffectsPosition = gameObject.transform;

		// Make sure projectileSpawnSpot isn't null
		if (projectileSpawnSpot == null)
			projectileSpawnSpot = gameObject.transform;

		// Make sure weaponModel isn't null
		if (weaponModel == null)
			weaponModel = gameObject;

		// Make sure crosshairTexture isn't null
		if (crosshairTexture == null)
			crosshairTexture = new Texture2D(0, 0);

		// Initialize the bullet hole pools list
		for (int i = 0; i < bulletHolePoolNames.Count; i++)
		{
			GameObject g = GameObject.Find(bulletHolePoolNames[i]);

			if (g != null && g.GetComponent<BulletHolePool>() != null)
				bulletHoleGroups[i].bulletHole = g.GetComponent<BulletHolePool>();
			else
				Debug.LogWarning("Bullet Hole Pool does not exist or does not have a BulletHolePool component.  Please assign GameObjects in the inspector that have the BulletHolePool component.");
		}

		// Initialize the default bullet hole pools list
		for (int i = 0; i < defaultBulletHolePoolNames.Count; i++)
		{
			GameObject g = GameObject.Find(defaultBulletHolePoolNames[i]);

			if (g.GetComponent<BulletHolePool>() != null)
				defaultBulletHoles[i] = g.GetComponent<BulletHolePool>();
			else
				Debug.LogWarning("Default Bullet Hole Pool does not have a BulletHolePool component.  Please assign GameObjects in the inspector that have the BulletHolePool component.");
		}
	}

	/// <summary>
	/// Update Function
	/// </summary>
	public void Tick()
	{
		// Calculate the current accuracy for this weapon
		currentAccuracy = Mathf.Lerp(currentAccuracy, accuracy, accuracyRecoverRate * Time.deltaTime);

		// Calculate the current crosshair size.  This is what causes the crosshairs to grow and shrink dynamically while shooting
		currentCrosshairSize = startingCrosshairSize + (accuracy - currentAccuracy) * 0.8f;

		// Update the fireTimer
		fireTimer += Time.deltaTime;

		// CheckForUserInput() handles the firing based on user input
		if (playerWeapon)
		{
			CheckForUserInput();
		}

		// Reload if the weapon is out of ammo
		if (reloadAutomatically && currentAmmo <= 0)
			Reload();

		RecoilRecovery();

		// Make sure StopBeam() is called when the weapon is no longer firing a beam (calling the Beam() method)
		if (type == WeaponType.Beam)
		{
			if (!beaming)
				StopBeam();
			beaming = false;    // The beaming variable is set to true every frame that the Beam() method is called
		}
	}

	/// <summary>
	/// Lerp weapon back to original position after applying recoil force
	/// </summary>
	public void RecoilRecovery()
	{
		// Recoil Recovery
		if (playerWeapon && recoil && type != WeaponType.Beam)
		{
			weaponModel.transform.position = Vector3.Lerp(weaponModel.transform.position, gameObject.transform.position, recoilRecoveryRate * Time.deltaTime);
			weaponModel.transform.rotation = Quaternion.Lerp(weaponModel.transform.rotation, gameObject.transform.rotation, recoilRecoveryRate * Time.deltaTime);
		}
	}

	/// <summary>
	/// Checks for user input to use the weapons - only if this weapon is player-controlled
	/// </summary>
	public void CheckForUserInput()
	{
		// Fire if this is a raycast type weapon and the user presses the fire button
		if (type == WeaponType.Raycast)
		{
			if (fireTimer >= actualROF && burstCounter < burstRate && canFire)
			{
				if (Input.GetButton("Fire1"))
				{
					if (!warmup)    // Normal firing when the user holds down the fire button
					{
						Fire();
					}
					else if (heat < maxWarmup)  // Otherwise just add to the warmup until the user lets go of the button
					{
						heat += Time.deltaTime;
					}
				}
				if (warmup && Input.GetButtonUp("Fire1"))
				{
					if (allowCancel && Input.GetButton("Cancel"))
					{
						heat = 0.0f;
					}
					else
					{
						Fire();
					}
				}
			}
		}
		// Launch a projectile if this is a projectile type weapon and the user presses the fire button
		if (type == WeaponType.Projectile)
		{
			if (fireTimer >= actualROF && burstCounter < burstRate && canFire)
			{
				if (Input.GetButton("Fire1"))
				{
					if (!warmup)    // Normal firing when the user holds down the fire button
					{
						Launch();
					}
					else if (heat < maxWarmup)  // Otherwise just add to the warmup until the user lets go of the button
					{
						heat += Time.deltaTime;
					}
				}
				if (warmup && Input.GetButtonUp("Fire1"))
				{
					if (allowCancel && Input.GetButton("Cancel"))
					{
						heat = 0.0f;
					}
					else
					{
						Launch();
					}
				}
			}

		}
		// Reset the Burst
		if (burstCounter >= burstRate)
		{
			burstTimer += Time.deltaTime;
			if (burstTimer >= burstPause)
			{
				burstCounter = 0;
				burstTimer = 0.0f;
			}
		}
		// Shoot a beam if this is a beam type weapon and the user presses the fire button
		if (type == WeaponType.Beam)
		{
			if (Input.GetButton("Fire1") && beamHeat <= maxBeamHeat && !coolingDown)
			{
				Beam();
			}
			else
			{
				// Stop the beaming
				StopBeam();
			}
			if (beamHeat >= maxBeamHeat)
			{
				coolingDown = true;
			}
			else if (beamHeat <= maxBeamHeat - (maxBeamHeat / 2))
			{
				coolingDown = false;
			}
		}

		// Reload if the "Reload" button is pressed
		if (Input.GetButtonDown("Reload"))
			Reload();

		// If the weapon is semi-auto and the user lets up on the button, set canFire to true
		if (Input.GetButtonUp("Fire1"))
			canFire = true;
	}

	/// <summary>
	/// A public method that causes the weapon to fire - can be called from other scripts - calls AI Firing for now
	/// </summary>
	public void RemoteFire()
	{
		AIFiring();
	}

	/// <summary>
	/// Determines when the AI can be firing
	/// </summary>
	public void AIFiring()
	{
		// Fire if this is a raycast type weapon
		if (type == WeaponType.Raycast)
		{
			if (fireTimer >= actualROF && burstCounter < burstRate && canFire)
			{
				monoBehaviour.StartCoroutine(DelayFire());    // Fires after the amount of time specified in delayBeforeFire
			}
		}
		// Launch a projectile if this is a projectile type weapon
		if (type == WeaponType.Projectile)
		{
			if (fireTimer >= actualROF && canFire)
			{
				monoBehaviour.StartCoroutine(DelayLaunch());
			}
		}
		// Reset the Burst
		if (burstCounter >= burstRate)
		{
			burstTimer += Time.deltaTime;
			if (burstTimer >= burstPause)
			{
				burstCounter = 0;
				burstTimer = 0.0f;
			}
		}
		// Shoot a beam if this is a beam type weapon
		if (type == WeaponType.Beam)
		{
			if (beamHeat <= maxBeamHeat && !coolingDown)
			{
				Beam();
			}
			else
			{
				// Stop the beaming
				StopBeam();
			}
			if (beamHeat >= maxBeamHeat)
			{
				coolingDown = true;
			}
			else if (beamHeat <= maxBeamHeat - (maxBeamHeat / 2))
			{
				coolingDown = false;
			}
		}
	}

	/// <summary>
	/// Coroutine to delay firing 
	/// </summary>
	/// <returns></returns>
	public IEnumerator DelayFire()
	{
		// Reset the fire timer to 0 (for ROF)
		fireTimer = 0.0f;

		// Increment the burst counter
		burstCounter++;

		// If this is a semi-automatic weapon, set canFire to false (this means the weapon can't fire again until the player lets up on the fire button)
		if (auto == Auto.Semi)
			canFire = false;

		// Send a messsage so that users can do other actions whenever this happens
		gameObject.SendMessageUpwards("OnEasyWeaponsFire", SendMessageOptions.DontRequireReceiver);

		yield return new WaitForSeconds(delayBeforeFire);
		Fire();
	}

	/// <summary>
	/// Coroutine to delay firing a missile
	/// </summary>
	/// <returns></returns>
	public IEnumerator DelayLaunch()
	{
		// Reset the fire timer to 0 (for ROF)
		fireTimer = 0.0f;

		// Increment the burst counter
		burstCounter++;

		// If this is a semi-automatic weapon, set canFire to false (this means the weapon can't fire again until the player lets up on the fire button)
		if (auto == Auto.Semi)
			canFire = false;

		// Send a messsage so that users can do other actions whenever this happens
		gameObject.SendMessageUpwards("OnEasyWeaponsLaunch", SendMessageOptions.DontRequireReceiver);

		yield return new WaitForSeconds(delayBeforeFire);
		Launch();
	}

	/// <summary>
	/// Coroutine to delay a beam
	/// </summary>
	/// <returns></returns>
	public IEnumerator DelayBeam()
	{
		yield return new WaitForSeconds(delayBeforeFire);
		Beam();
	}

	/// <summary>
	/// updates GUI, run in Monobehaviour OnGUI function
	/// </summary>
	public void OnGUI()
	{

		// Crosshairs
		if (type == WeaponType.Projectile || type == WeaponType.Beam)
		{
			currentAccuracy = accuracy;
		}
		if (showCrosshair)
		{
			// Hold the location of the center of the screen in a variable
			Vector2 center = new Vector2(Screen.width / 2, Screen.height / 2);

			// Draw the crosshairs based on the weapon's inaccuracy
			// Left
			Rect leftRect = new Rect(center.x - crosshairLength - currentCrosshairSize, center.y - (crosshairWidth / 2), crosshairLength, crosshairWidth);
			GUI.DrawTexture(leftRect, crosshairTexture, ScaleMode.StretchToFill);
			// Right
			Rect rightRect = new Rect(center.x + currentCrosshairSize, center.y - (crosshairWidth / 2), crosshairLength, crosshairWidth);
			GUI.DrawTexture(rightRect, crosshairTexture, ScaleMode.StretchToFill);
			// Top
			Rect topRect = new Rect(center.x - (crosshairWidth / 2), center.y - crosshairLength - currentCrosshairSize, crosshairWidth, crosshairLength);
			GUI.DrawTexture(topRect, crosshairTexture, ScaleMode.StretchToFill);
			// Bottom
			Rect bottomRect = new Rect(center.x - (crosshairWidth / 2), center.y + currentCrosshairSize, crosshairWidth, crosshairLength);
			GUI.DrawTexture(bottomRect, crosshairTexture, ScaleMode.StretchToFill);
		}

		// Ammo Display
		if (showCurrentAmmo)
		{
			if (type == WeaponType.Raycast || type == WeaponType.Projectile)
				GUI.Label(new Rect(10, Screen.height - 30, 100, 20), "Ammo: " + currentAmmo);
			else if (type == WeaponType.Beam)
				GUI.Label(new Rect(10, Screen.height - 30, 100, 20), "Heat: " + (int)(beamHeat * 100) + "/" + (int)(maxBeamHeat * 100));
		}
	}

	/// <summary>
	/// Fires projectile
	/// </summary>
	public void Fire()
	{
		// Reset the fireTimer to 0 (for ROF)
		fireTimer = 0.0f;

		// Increment the burst counter
		burstCounter++;

		// If this is a semi-automatic weapon, set canFire to false (this means the weapon can't fire again until the player lets up on the fire button)
		if (auto == Auto.Semi)
			canFire = false;

		// First make sure there is ammo
		if (currentAmmo <= 0)
		{
			DryFire();
			return;
		}

		// Subtract 1 from the current ammo
		if (!infiniteAmmo)
			currentAmmo--;


		// Fire once for each shotPerRound value
		for (int i = 0; i < shotPerRound; i++)
		{
			// Calculate accuracy for this shot
			float accuracyVary = (100 - currentAccuracy) / 1000;
			Vector3 direction = raycastStartSpot.forward;
			direction.x += UnityEngine.Random.Range(-accuracyVary, accuracyVary);
			direction.y += UnityEngine.Random.Range(-accuracyVary, accuracyVary);
			direction.z += UnityEngine.Random.Range(-accuracyVary, accuracyVary);
			currentAccuracy -= accuracyDropPerShot;
			if (currentAccuracy <= 0.0f)
				currentAccuracy = 0.0f;

			// The ray that will be used for this shot
			Ray ray = new Ray(raycastStartSpot.position, direction);
			RaycastHit hit;

			// Fire a ray forward, only detect hits on the enemy layer 
			if (Physics.Raycast(ray, out hit, range, 1 << Statics.enemyLayer))
			{
				// Warmup heat
				float damage = power;
				if (warmup)
				{
					damage *= heat * powerMultiplier;
					heat = 0.0f;
				}

				// Damage
				hit.collider.gameObject.SendMessageUpwards("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);

				if (shooterAIEnabled)
				{
					hit.transform.SendMessageUpwards("Damage", damage / 100, SendMessageOptions.DontRequireReceiver);
				}

				if (bloodyMessEnabled)
				{
					//call the ApplyDamage() function on the enenmy CharacterSetup script
					if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Limb"))
					{
						Vector3 directionShot = hit.collider.transform.position - gameObject.transform.position;

						//  Un-comment the following section for Bloody Mess compatibility
						/*
						if (hit.collider.gameObject.GetComponent<Limb>())
						{
							GameObject parent = hit.collider.gameObject.GetComponent<Limb>().parent;
							CharacterSetup character = parent.GetComponent<CharacterSetup>();
							character.ApplyDamage(damage, hit.collider.gameObject, weaponType, directionShot, Camera.main.transform.position);
						}
						*/
					}
				}

				// Bullet Holes

				// Make sure the hit GameObject is not defined as an exception for bullet holes
				bool exception = false;
				if (bhSystem == BulletHoleSystem.Tag)
				{
					foreach (SmartBulletHoleGroup bhg in bulletHoleExceptions)
					{
						if (hit.collider.gameObject.tag == bhg.tag)
						{
							exception = true;
							break;
						}
					}
				}
				else if (bhSystem == BulletHoleSystem.Material)
				{
					foreach (SmartBulletHoleGroup bhg in bulletHoleExceptions)
					{
						MeshRenderer mesh = FindMeshRenderer(hit.collider.gameObject);
						if (mesh != null)
						{
							if (mesh.sharedMaterial == bhg.material)
							{
								exception = true;
								break;
							}
						}
					}
				}
				else if (bhSystem == BulletHoleSystem.Physic_Material)
				{
					foreach (SmartBulletHoleGroup bhg in bulletHoleExceptions)
					{
						if (hit.collider.sharedMaterial == bhg.physicMaterial)
						{
							exception = true;
							break;
						}
					}
				}

				// Select the bullet hole pools if there is no exception
				if (makeBulletHoles && !exception)
				{
					// A list of the bullet hole prefabs to choose from
					List<SmartBulletHoleGroup> holes = new List<SmartBulletHoleGroup>();

					// Display the bullet hole groups based on tags
					if (bhSystem == BulletHoleSystem.Tag)
					{
						foreach (SmartBulletHoleGroup bhg in bulletHoleGroups)
						{
							if (hit.collider.gameObject.tag == bhg.tag)
							{
								holes.Add(bhg);
							}
						}
					}

					// Display the bullet hole groups based on materials
					else if (bhSystem == BulletHoleSystem.Material)
					{
						// Get the mesh that was hit, if any
						MeshRenderer mesh = FindMeshRenderer(hit.collider.gameObject);

						foreach (SmartBulletHoleGroup bhg in bulletHoleGroups)
						{
							if (mesh != null)
							{
								if (mesh.sharedMaterial == bhg.material)
								{
									holes.Add(bhg);
								}
							}
						}
					}

					// Display the bullet hole groups based on physic materials
					else if (bhSystem == BulletHoleSystem.Physic_Material)
					{
						foreach (SmartBulletHoleGroup bhg in bulletHoleGroups)
						{
							if (hit.collider.sharedMaterial == bhg.physicMaterial)
							{
								holes.Add(bhg);
							}
						}
					}


					SmartBulletHoleGroup sbhg = null;

					// If no bullet holes were specified for this parameter, use the default bullet holes
					if (holes.Count == 0)   // If no usable (for this hit GameObject) bullet holes were found...
					{
						List<SmartBulletHoleGroup> defaultsToUse = new List<SmartBulletHoleGroup>();
						foreach (BulletHolePool h in defaultBulletHoles)
						{
							defaultsToUse.Add(new SmartBulletHoleGroup("Default", null, null, h));
						}

						// Choose a bullet hole at random from the list
						sbhg = defaultsToUse[Random.Range(0, defaultsToUse.Count)];
					}

					// Make the actual bullet hole GameObject
					else
					{
						// Choose a bullet hole at random from the list
						sbhg = holes[Random.Range(0, holes.Count)];
					}

					// Place the bullet hole in the scene
					if (sbhg.bulletHole != null)
						sbhg.bulletHole.PlaceBulletHole(hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
				}

				// Hit Effects
				if (makeHitEffects)
				{
					foreach (GameObject hitEffect in hitEffects)
					{
						if (hitEffect != null)
							GameObject.Instantiate(hitEffect, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
					}
				}

				// Apply Hit force to target rigidbody
				ApplyHitForce(hit, ray, forceMultiplier, hitReactionTime);
			}
		}

		// Recoil
		if (recoil)
			Recoil();

		// Muzzle flash effects
		if (makeMuzzleEffects)
		{
			GameObject muzfx = muzzleEffects[Random.Range(0, muzzleEffects.Length)];
			if (muzfx != null)
				GameObject.Instantiate(muzfx, muzzleEffectsPosition.position, muzzleEffectsPosition.rotation);
		}

		// Instantiate shell props
		if (spitShells)
		{
			GameObject shellGO = GameObject.Instantiate(shell, shellSpitPosition.position, shellSpitPosition.rotation) as GameObject;
			shellGO.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(shellSpitForce + Random.Range(0, shellForceRandom), 0, 0), ForceMode.Impulse);
			shellGO.GetComponent<Rigidbody>().AddRelativeTorque(new Vector3(shellSpitTorqueX + Random.Range(-shellTorqueRandom, shellTorqueRandom), shellSpitTorqueY + Random.Range(-shellTorqueRandom, shellTorqueRandom), 0), ForceMode.Impulse);
		}

		// Play the gunshot sound
		gameObject.GetComponent<AudioSource>().PlayOneShot(fireSound);
	}

	/// <summary>
	/// If an enemy has been hit, apply a hit force to the target rigidbody
	/// </summary>
	/// <param name="_hit"></param>
	/// <param name="_ray"></param>
	/// <param name="_forceMultiplier"></param>
	/// <param name="_delay"></param>
	public void ApplyHitForce(RaycastHit _hit, Ray _ray, float _forceMultiplier, float _delay)
	{
		hitEnemy = _hit.collider.GetComponent<EnemyMovement>();

		if (hitEnemy)
		{
			hitEnemy.StopCoroutine(hitEnemy.hitReaction);
			hitEnemy.hitReaction = hitEnemy.HitReaction(_ray.direction, _forceMultiplier, _delay);
			hitEnemy.StartCoroutine(hitEnemy.hitReaction);
		}
	}

	/// <summary>
	/// Projectile system
	/// </summary>
	public void Launch()
	{
		// Reset the fire timer to 0 (for ROF)
		fireTimer = 0.0f;

		// Increment the burst counter
		burstCounter++;

		// If this is a semi-automatic weapon, set canFire to false (this means the weapon can't fire again until the player lets up on the fire button)
		if (auto == Auto.Semi)
			canFire = false;

		// First make sure there is ammo
		if (currentAmmo <= 0)
		{
			DryFire();
			return;
		}

		// Subtract 1 from the current ammo
		if (!infiniteAmmo)
			currentAmmo--;

		// Fire once for each shotPerRound value
		for (int i = 0; i < shotPerRound; i++)
		{
			// Instantiate the projectile
			if (projectile != null)
			{
				GameObject proj = GameObject.Instantiate(projectile, projectileSpawnSpot.position, projectileSpawnSpot.rotation) as GameObject;

				// Warmup heat
				if (warmup)
				{
					if (multiplyPower)
						proj.SendMessage("MultiplyDamage", heat * powerMultiplier, SendMessageOptions.DontRequireReceiver);
					if (multiplyForce)
						proj.SendMessage("MultiplyInitialForce", heat * initialForceMultiplier, SendMessageOptions.DontRequireReceiver);

					heat = 0.0f;
				}
			}
			else
			{
				Debug.Log("Projectile to be instantiated is null.  Make sure to set the Projectile field in the inspector.");
			}
		}

		// Recoil
		if (recoil)
			Recoil();

		// Muzzle flash effects
		if (makeMuzzleEffects)
		{
			GameObject muzfx = muzzleEffects[Random.Range(0, muzzleEffects.Length)];
			if (muzfx != null)
				GameObject.Instantiate(muzfx, muzzleEffectsPosition.position, muzzleEffectsPosition.rotation);
		}

		// Instantiate shell props
		if (spitShells)
		{
			GameObject shellGO = GameObject.Instantiate(shell, shellSpitPosition.position, shellSpitPosition.rotation) as GameObject;
			shellGO.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(shellSpitForce + Random.Range(0, shellForceRandom), 0, 0), ForceMode.Impulse);
			shellGO.GetComponent<Rigidbody>().AddRelativeTorque(new Vector3(shellSpitTorqueX + Random.Range(-shellTorqueRandom, shellTorqueRandom), shellSpitTorqueY + Random.Range(-shellTorqueRandom, shellTorqueRandom), 0), ForceMode.Impulse);
		}

		// Play the gunshot sound
		gameObject.GetComponent<AudioSource>().PlayOneShot(fireSound);
	}

	/// <summary>
	/// Beam system
	/// </summary>
	public void Beam()
	{
		// Send a messsage so that users can do other actions whenever this happens
		gameObject.SendMessageUpwards("OnEasyWeaponsBeaming", SendMessageOptions.DontRequireReceiver);

		// Set the beaming variable to true
		beaming = true;

		// Make the beam weapon heat up as it is being used
		if (!infiniteBeam)
			beamHeat += Time.deltaTime;

		// Make the beam effect if it hasn't already been made.  This system uses a line renderer on an otherwise empty instantiated GameObject
		if (beamGO == null)
		{
			beamGO = new GameObject(beamTypeName, typeof(LineRenderer));
			beamGO.transform.parent = gameObject.transform;        // Make the beam object a child of the weapon object, so that when the weapon is deactivated the beam will be as well	- was beamGO.transform.SetParent(transform), which only works in Unity 4.6 or newer;
		}
		LineRenderer beamLR = beamGO.GetComponent<LineRenderer>();
		beamLR.material = beamMaterial;
		beamLR.material.SetColor("_TintColor", beamColor);
		beamLR.SetWidth(startBeamWidth, endBeamWidth);

		// The number of reflections
		int reflections = 0;

		// All the points at which the laser is reflected
		List<Vector3> reflectionPoints = new List<Vector3>();
		// Add the first point to the list of beam reflection points
		reflectionPoints.Add(raycastStartSpot.position);

		// Hold a variable for the last reflected point
		Vector3 lastPoint = raycastStartSpot.position;

		// Declare variables for calculating rays
		Vector3 incomingDirection;
		Vector3 reflectDirection;

		// Whether or not the beam needs to continue reflecting
		bool keepReflecting = true;

		// Raycasting (damgage, etc)
		Ray ray = new Ray(lastPoint, raycastStartSpot.forward);
		RaycastHit hit;

		do
		{
			// Initialize the next point.  If a raycast hit is not returned, this will be the forward direction * range
			Vector3 nextPoint = ray.direction * range;

			if (Physics.Raycast(ray, out hit, range))
			{
				// Set the next point to the hit location from the raycast
				nextPoint = hit.point;

				// Calculate the next direction in which to shoot a ray
				incomingDirection = nextPoint - lastPoint;
				reflectDirection = Vector3.Reflect(incomingDirection, hit.normal);
				ray = new Ray(nextPoint, reflectDirection);

				// Update the lastPoint variable
				lastPoint = hit.point;

				// Hit Effects
				if (makeHitEffects)
				{
					foreach (GameObject hitEffect in hitEffects)
					{
						if (hitEffect != null)
							GameObject.Instantiate(hitEffect, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
					}
				}

				// Damage
				hit.collider.gameObject.SendMessageUpwards("ChangeHealth", -beamPower, SendMessageOptions.DontRequireReceiver);

				// Shooter AI support
				if (shooterAIEnabled)
				{
					hit.transform.SendMessageUpwards("Damage", beamPower / 100, SendMessageOptions.DontRequireReceiver);
				}

				// Bloody Mess support
				if (bloodyMessEnabled)
				{
					//call the ApplyDamage() function on the enenmy CharacterSetup script
					if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Limb"))
					{
						Vector3 directionShot = hit.collider.transform.position - gameObject.transform.position;

						//  Remove the comment marks from the following section of code for Bloody Mess support
						/*
						if (hit.collider.gameObject.GetComponent<Limb>())
						{
							GameObject parent = hit.collider.gameObject.GetComponent<Limb>().parent;
							
							CharacterSetup character = parent.GetComponent<CharacterSetup>();
							character.ApplyDamage(beamPower, hit.collider.gameObject, weaponType, directionShot, Camera.main.transform.position);
						}
						*/

					}
				}


				// Increment the reflections counter
				reflections++;
			}
			else
			{

				keepReflecting = false;
			}

			// Add the next point to the list of beam reflection points
			reflectionPoints.Add(nextPoint);

		} while (keepReflecting && reflections < maxReflections && reflect && (reflectionMaterial == null || (FindMeshRenderer(hit.collider.gameObject) != null && FindMeshRenderer(hit.collider.gameObject).sharedMaterial == reflectionMaterial)));

		// Set the positions of the vertices of the line renderer beam
		beamLR.SetVertexCount(reflectionPoints.Count);
		for (int i = 0; i < reflectionPoints.Count; i++)
		{
			beamLR.SetPosition(i, reflectionPoints[i]);

			// Muzzle reflection effects
			if (makeMuzzleEffects && i > 0)     // Doesn't make the FX on the first iteration since that is handled later.  This is so that the FX at the muzzle point can be parented to the weapon
			{
				GameObject muzfx = muzzleEffects[Random.Range(0, muzzleEffects.Length)];
				if (muzfx != null)
				{
					GameObject.Instantiate(muzfx, reflectionPoints[i], muzzleEffectsPosition.rotation);
				}
			}
		}

		// Muzzle flash effects
		if (makeMuzzleEffects)
		{
			GameObject muzfx = muzzleEffects[Random.Range(0, muzzleEffects.Length)];
			if (muzfx != null)
			{
				GameObject mfxGO = GameObject.Instantiate(muzfx, muzzleEffectsPosition.position, muzzleEffectsPosition.rotation) as GameObject;
				mfxGO.transform.parent = raycastStartSpot;
			}
		}

		// Play the beam fire sound
		if (!gameObject.GetComponent<AudioSource>().isPlaying)
		{
			gameObject.GetComponent<AudioSource>().clip = fireSound;
			gameObject.GetComponent<AudioSource>().Play();
		}
	}

	/// <summary>
	/// Stop the beam
	/// </summary>
	public void StopBeam()
	{
		// Restart the beam timer
		beamHeat -= Time.deltaTime;
		if (beamHeat < 0)
			beamHeat = 0;

		if (gameObject.GetComponent<AudioSource>())
			gameObject.GetComponent<AudioSource>().Stop();


		// Remove the visible beam effect GameObject
		if (beamGO != null)
		{
			GameObject.Destroy(beamGO);
		}

		// Send a messsage so that users can do other actions whenever this happens
		gameObject.SendMessageUpwards("OnEasyWeaponsStopBeaming", SendMessageOptions.DontRequireReceiver);
	}

	/// <summary>
	/// Reload the weapon
	/// </summary>
	public void Reload()
	{
		currentAmmo = ammoCapacity;
		fireTimer = -reloadTime;
		gameObject.GetComponent<AudioSource>().PlayOneShot(reloadSound);

		// Send a messsage so that users can do other actions whenever this happens
		gameObject.SendMessageUpwards("OnEasyWeaponsReload", SendMessageOptions.DontRequireReceiver);
	}

	/// <summary>
	/// When the weapon tries to fire without any ammo
	/// </summary>
	public void DryFire()
	{
		gameObject.GetComponent<AudioSource>().PlayOneShot(dryFireSound);
	}

	/// <summary>
	/// Recoil FX.  This is the "kick" that you see when the weapon moves back while firing
	/// </summary>
	public void Recoil()
	{
		// No recoil for AIs
		if (!playerWeapon)
			return;

		// Make sure the user didn't leave the weapon model field blank
		if (weaponModel == null)
		{
			Debug.Log("Weapon Model is null.  Make sure to set the Weapon Model field in the inspector.");
			return;
		}

		// Calculate random values for the recoil position and rotation
		float kickBack = Random.Range(recoilKickBackMin, recoilKickBackMax);
		float kickRot = Random.Range(recoilRotationMin, recoilRotationMax);

		// Apply the random values to the weapon's postion and rotation
		weaponModel.transform.Translate(new Vector3(0, 0, -kickBack), Space.Self);
		weaponModel.transform.Rotate(new Vector3(-kickRot, 0, 0), Space.Self);
	}

	/// <summary>
	/// Find a mesh renderer in a specified gameobject, it's children, or its parents
	/// </summary>
	/// <param name="go"></param>
	/// <returns></returns>
	public MeshRenderer FindMeshRenderer(GameObject go)
	{
		MeshRenderer hitMesh;

		// Use the MeshRenderer directly from this GameObject if it has one
		if (go.GetComponent<Renderer>() != null)
		{
			hitMesh = go.GetComponent<MeshRenderer>();
		}

		// Try to find a child or parent GameObject that has a MeshRenderer
		else
		{
			// Look for a renderer in the child GameObjects
			hitMesh = go.GetComponentInChildren<MeshRenderer>();

			// If a renderer is still not found, try the parent GameObjects
			if (hitMesh == null)
			{
				GameObject curGO = go;
				while (hitMesh == null && curGO.transform != curGO.transform.root)
				{
					curGO = curGO.transform.parent.gameObject;
					hitMesh = curGO.GetComponent<MeshRenderer>();
				}
			}
		}

		return hitMesh;
	}

	#endregion
	

}

[System.Serializable]
public class WeaponSystemData
{

	public int startingWeaponIndex = 0;         // The weapon index that the player will start with
	public int weaponIndex;                    // The current index of the active weapon

	public WeaponSystemData() { }

	public WeaponSystemData(WeaponSystemData _weaponSystemData)
	{
		//this.gameObject = _weaponSystemData.gameObject;
		//this.weapons = _weaponSystemData.weapons;
		this.startingWeaponIndex = _weaponSystemData.startingWeaponIndex;
		this.weaponIndex = _weaponSystemData.weaponIndex;
	}

	/// <summary>
	/// Call this on start to initialize variables for weapon system
	/// </summary>
	public void Initialize(GameObject[] _weapons)
	{
		// Make sure the starting active weapon is the one selected by the user in startingWeaponIndex
	//	DataManager.Instance.weaponDataContainer.weaponSystemData = this;
	//	DataManager.Instance.weaponDataContainer.weaponDataList = w
		weaponIndex = startingWeaponIndex;
		SetActiveWeapon(weaponIndex, _weapons);
	}

	/// <summary>
	/// Call this on update to allow for weapon switch
	/// </summary>
	public void Tick(GameObject[] _weapons) 
	{
		// Allow the user to instantly switch to any weapon
		if (Input.GetButtonDown("Weapon 1"))
			SetActiveWeapon(0, _weapons);
		if (Input.GetButtonDown("Weapon 2"))
			SetActiveWeapon(1, _weapons);
		if (Input.GetButtonDown("Weapon 3"))
			SetActiveWeapon(2, _weapons);
		if (Input.GetButtonDown("Weapon 4"))
			SetActiveWeapon(3, _weapons);
		if (Input.GetButtonDown("Weapon 5"))
			SetActiveWeapon(4, _weapons);
		if (Input.GetButtonDown("Weapon 6"))
			SetActiveWeapon(5, _weapons);
		if (Input.GetButtonDown("Weapon 7"))
			SetActiveWeapon(6, _weapons);
		if (Input.GetButtonDown("Weapon 8"))
			SetActiveWeapon(7, _weapons);
		if (Input.GetButtonDown("Weapon 9"))
			SetActiveWeapon(8, _weapons);

		// Allow the user to scroll through the weapons
		if (Input.GetAxis("Mouse ScrollWheel") > 0)
			NextWeapon(_weapons);
		if (Input.GetAxis("Mouse ScrollWheel") < 0)
			PreviousWeapon(_weapons);
	}

	public void SetActiveWeapon(int _index, GameObject[] _weapons)
	{
		// Make sure this weapon exists before trying to switch to it
		if (_index >= _weapons.Length || _index < 0)
		{
			Debug.LogWarning("Tried to switch to a weapon that does not exist.  Make sure you have all the correct weapons in your weapons array.");
			return;
		}

		// Send a messsage so that users can do other actions whenever this happens
		//gameObject.SendMessageUpwards("OnEasyWeaponsSwitch", SendMessageOptions.DontRequireReceiver);

		// Make sure the weaponIndex references the correct weapon
		weaponIndex = _index;

		// Make sure beam game objects aren't left over after weapon switching
			//weapons[_index].GetComponent<Weapon>().weaponData.StopBeam();

		// Start be deactivating all weapons
		for (int i = 0; i < _weapons.Length; i++)
		{
			_weapons[i].SetActive(false);
		}

		// Activate the one weapon that we want
		_weapons[_index].SetActive(true);
	}

	public void NextWeapon(GameObject[] _weapons)
	{
		weaponIndex++;
		if (weaponIndex > _weapons.Length - 1)
			weaponIndex = 0;
		SetActiveWeapon(weaponIndex, _weapons);
	}

	public void PreviousWeapon(GameObject[] _weapons)
	{
		weaponIndex--;
		if (weaponIndex < 0)
			weaponIndex = _weapons.Length - 1;
		SetActiveWeapon(weaponIndex, _weapons);
	}
}


[XmlRoot("WeaponCollection")]
[System.Serializable]
public class WeaponDataContainer
{
	public WeaponSystemData weaponSystemData = new WeaponSystemData();

	[XmlArray("Weapons")]
	[XmlArrayItem("Weapon")]
	public List<WeaponData> weaponDataList = new List<WeaponData>();


	/// <summary>
	/// Saves score data to an XML and Binary file at the persistent data path of the project
	/// </summary>
	public void Save()
    {
        // Create a new list of score data and populate it
        WeaponDataContainer tempWeaponDataContainer = this;

        //The standard file path in unity
        Debug.Log("Your files are located here: " + Application.persistentDataPath);

        //Creates a new FileSave object with the file format XML.
        FileSave fileSave = new FileSave(FileFormat.Xml);

        //Writes an XML file to the path.
        fileSave.WriteToFile(Application.persistentDataPath + "/weaponData.xml", tempWeaponDataContainer);

		// Writing to binary causing errors for this object, in future look foranother option to save weapons 
		/*
        //Changes the file format to Binary file
        fileSave.fileFormat = FileFormat.Binary;

        //Writes a binary file
        fileSave.WriteToFile(Application.persistentDataPath + "/weaponData.bin", tempWeaponDataContainer);
		*/

	}

	/// <summary>
	/// Loads a binary data file from the persistent data path of the project
	/// </summary>
	/// <returns></returns>
	public static WeaponDataContainer Load()
    {
		// Reading from binary causing errors for this object, in future look foranother option to load weapons 
		/*
        //Creates a new FileSave object with the file format XML.
        FileSave fileSave = new FileSave(FileFormat.Binary);

        //...and loads the data from the Binary file
        WeaponDataContainer retVal = fileSave.ReadFromFile<WeaponDataContainer>(Application.persistentDataPath + "/weaponData.bin");
		*/

		//Creates a new FileSave object with the file format XML.
		FileSave fileSave = new FileSave(FileFormat.Xml);

		//...and loads the data from the Binary file
		WeaponDataContainer retVal = fileSave.ReadFromFile<WeaponDataContainer>(Application.persistentDataPath + "/weaponData.xml");

		if (retVal != null)
            return retVal;
        else
            return new WeaponDataContainer();
    }
}