/// <summary>
/// WeaponEditor.cs
/// Author: MutantGopher
/// This script creates a custom inspector for the weapon system in Weapon.cs.
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(Weapon))]
public class WeaponEditor : Editor
{
	private bool showPluginSupport = false;
	private bool showGeneral = false;
	private bool showAmmo = false;
	private bool showROF = false;
	private bool showPower = false;
	private bool showAccuracy = false;
	private bool showWarmup = false;
	private bool showRecoil = false;
	private bool showEffects = false;
	private bool showBulletHoles = false;
	private bool showCrosshairs = false;
	private bool showAudio = false;

	
	public override void OnInspectorGUI()
	{
		// Get a reference to the weapon script
		Weapon weapon = (Weapon)target;

		// Weapon type
		weapon.weaponData.type = (WeaponType)EditorGUILayout.EnumPopup("Weapon Type", weapon.weaponData.type);

		// External tools support
		showPluginSupport = EditorGUILayout.Foldout(showPluginSupport, "3rd Party Plugin Support");
		if (showPluginSupport)
		{
			// Shooter AI support
			weapon.weaponData.shooterAIEnabled = EditorGUILayout.Toggle(new GUIContent("Shooter AI", "Support for Shooter AI by Gateway Games"), weapon.weaponData.shooterAIEnabled);

			// Bloody Mess support
			weapon.weaponData.bloodyMessEnabled = EditorGUILayout.Toggle (new GUIContent("Bloody Mess"), weapon.weaponData.bloodyMessEnabled);
			if (weapon.weaponData.bloodyMessEnabled)
			{
				weapon.weaponData.weaponType = EditorGUILayout.IntField("Weapon Type", weapon.weaponData.weaponType);
			}
		}

		// General
		showGeneral = EditorGUILayout.Foldout(showGeneral, "General");
		if (showGeneral)
		{
			weapon.weaponData.playerWeapon = EditorGUILayout.Toggle("Player's Weapon", weapon.weaponData.playerWeapon);
			if (weapon.weaponData.type == WeaponType.Raycast || weapon.weaponData.type == WeaponType.Projectile)
				weapon.weaponData.auto = (Auto)EditorGUILayout.EnumPopup("Auto Type", weapon.weaponData.auto);
			weapon.weaponData.weaponModel = (GameObject)EditorGUILayout.ObjectField("Weapon Model", weapon.weaponData.weaponModel, typeof(GameObject), true);
			if (weapon.weaponData.type == WeaponType.Raycast || weapon.weaponData.type == WeaponType.Beam)
				weapon.weaponData.raycastStartSpot = (Transform)EditorGUILayout.ObjectField("Raycasting Point", weapon.weaponData.raycastStartSpot, typeof(Transform), true);

			// Projectile
			if (weapon.weaponData.type == WeaponType.Projectile)
			{
				weapon.weaponData.projectile = (GameObject)EditorGUILayout.ObjectField("Projectile", weapon.weaponData.projectile, typeof(GameObject), false);
				weapon.weaponData.projectileSpawnSpot = (Transform)EditorGUILayout.ObjectField("Projectile Spawn Point", weapon.weaponData.projectileSpawnSpot, typeof(Transform), true);
			}

			// Beam
			if (weapon.weaponData.type == WeaponType.Beam)
			{
				weapon.weaponData.reflect = EditorGUILayout.Toggle("Reflect", weapon.weaponData.reflect);
				weapon.weaponData.reflectionMaterial = (Material)EditorGUILayout.ObjectField("Reflection Material", weapon.weaponData.reflectionMaterial, typeof(Material), false);
				weapon.weaponData.maxReflections = EditorGUILayout.IntField("Max Reflections", weapon.weaponData.maxReflections);
				weapon.weaponData.beamTypeName = EditorGUILayout.TextField("Beam Effect Name", weapon.weaponData.beamTypeName);
				weapon.weaponData.maxBeamHeat = EditorGUILayout.FloatField("Max Heat", weapon.weaponData.maxBeamHeat);
				weapon.weaponData.infiniteBeam = EditorGUILayout.Toggle("Infinite", weapon.weaponData.infiniteBeam);
				weapon.weaponData.beamMaterial = (Material)EditorGUILayout.ObjectField("Material", weapon.weaponData.beamMaterial, typeof(Material), false);
				weapon.weaponData.beamColor = EditorGUILayout.ColorField("Color", weapon.weaponData.beamColor);
				weapon.weaponData.startBeamWidth = EditorGUILayout.FloatField("Start Width", weapon.weaponData.startBeamWidth);
				weapon.weaponData.endBeamWidth = EditorGUILayout.FloatField("End Width", weapon.weaponData.endBeamWidth);
			}

			if (weapon.weaponData.type == WeaponType.Beam)
				weapon.weaponData.showCurrentAmmo = EditorGUILayout.Toggle("Show Current Heat", weapon.weaponData.showCurrentAmmo);

		}



		// Power
		if (weapon.weaponData.type == WeaponType.Raycast || weapon.weaponData.type == WeaponType.Beam)
		{
			showPower = EditorGUILayout.Foldout(showPower, "Power");
			if (showPower)
			{
				if (weapon.weaponData.type == WeaponType.Raycast)
					weapon.weaponData.power = EditorGUILayout.FloatField("Power", weapon.weaponData.power);
				else
					weapon.weaponData.beamPower = EditorGUILayout.FloatField("Power", weapon.weaponData.beamPower);

				weapon.weaponData.forceMultiplier = EditorGUILayout.FloatField("Force Multiplier", weapon.weaponData.forceMultiplier);
				weapon.weaponData.range = EditorGUILayout.FloatField("Range", weapon.weaponData.range);
				weapon.weaponData.hitReactionTime = EditorGUILayout.FloatField("Hit Reaction Time", weapon.weaponData.hitReactionTime);
			}
		}


		// ROF
		if (weapon.weaponData.type == WeaponType.Raycast || weapon.weaponData.type == WeaponType.Projectile)
		{
			showROF = EditorGUILayout.Foldout(showROF, "Rate Of Fire");
			if (showROF)
			{
				weapon.weaponData.rateOfFire = EditorGUILayout.FloatField("Rate Of Fire", weapon.weaponData.rateOfFire);
				weapon.weaponData.delayBeforeFire = EditorGUILayout.FloatField("Delay Before Fire", weapon.weaponData.delayBeforeFire);
				// Burst
				weapon.weaponData.burstRate = EditorGUILayout.IntField("Burst Rate", weapon.weaponData.burstRate);
				weapon.weaponData.burstPause = EditorGUILayout.FloatField("Burst Pause", weapon.weaponData.burstPause);
			}
		}


		// Ammo
		if (weapon.weaponData.type == WeaponType.Raycast || weapon.weaponData.type == WeaponType.Projectile)
		{
			showAmmo = EditorGUILayout.Foldout(showAmmo, "Ammunition");
			if (showAmmo)
			{
				weapon.weaponData.infiniteAmmo = EditorGUILayout.Toggle("Infinite Ammo", weapon.weaponData.infiniteAmmo);

				if (!weapon.weaponData.infiniteAmmo)
				{
					weapon.weaponData.ammoCapacity = EditorGUILayout.IntField("Ammo Capacity", weapon.weaponData.ammoCapacity);
					weapon.weaponData.reloadTime = EditorGUILayout.FloatField("Reload Time", weapon.weaponData.reloadTime);
					weapon.weaponData.showCurrentAmmo = EditorGUILayout.Toggle("Show Current Ammo", weapon.weaponData.showCurrentAmmo);
					weapon.weaponData.reloadAutomatically = EditorGUILayout.Toggle("Reload Automatically", weapon.weaponData.reloadAutomatically);
				}
				weapon.weaponData.shotPerRound = EditorGUILayout.IntField("Shots Per Round", weapon.weaponData.shotPerRound);
			}
		}



		// Accuracy
		if (weapon.weaponData.type == WeaponType.Raycast)
		{
			showAccuracy = EditorGUILayout.Foldout(showAccuracy, "Accuracy");
			if (showAccuracy)
			{
				weapon.weaponData.accuracy = EditorGUILayout.FloatField("Accuracy", weapon.weaponData.accuracy);
				weapon.weaponData.accuracyDropPerShot = EditorGUILayout.FloatField("Accuracy Drop Per Shot", weapon.weaponData.accuracyDropPerShot);
				weapon.weaponData.accuracyRecoverRate = EditorGUILayout.FloatField("Accuracy Recover Rate", weapon.weaponData.accuracyRecoverRate);
			}
		}


		// Warmup
		if ((weapon.weaponData.type == WeaponType.Raycast || weapon.weaponData.type == WeaponType.Projectile) && weapon.weaponData.auto == Auto.Semi)
		{
			showWarmup = EditorGUILayout.Foldout(showWarmup, "Warmup");
			if (showWarmup)
			{
				weapon.weaponData.warmup = EditorGUILayout.Toggle("Warmup", weapon.weaponData.warmup);

				if (weapon.weaponData.warmup)
				{
					weapon.weaponData.maxWarmup = EditorGUILayout.FloatField("Max Warmup", weapon.weaponData.maxWarmup);
					
					if (weapon.weaponData.type == WeaponType.Projectile)
					{
						weapon.weaponData.multiplyForce = EditorGUILayout.Toggle("Multiply Force", weapon.weaponData.multiplyForce);
						if (weapon.weaponData.multiplyForce)
							weapon.weaponData.initialForceMultiplier = EditorGUILayout.FloatField("Initial Force Multiplier", weapon.weaponData.initialForceMultiplier);

						weapon.weaponData.multiplyPower = EditorGUILayout.Toggle("Multiply Power", weapon.weaponData.multiplyPower);
						if (weapon.weaponData.multiplyPower)
							weapon.weaponData.powerMultiplier = EditorGUILayout.FloatField("Power Multiplier", weapon.weaponData.powerMultiplier);
					}
					else	// If this is a raycast weapon
					{
						weapon.weaponData.powerMultiplier = EditorGUILayout.FloatField("Power Multiplier", weapon.weaponData.powerMultiplier);
					}
					weapon.weaponData.allowCancel = EditorGUILayout.Toggle("Allow Cancel", weapon.weaponData.allowCancel);
				}
			}
		}


		// Recoil
		if (weapon.weaponData.type == WeaponType.Raycast || weapon.weaponData.type == WeaponType.Projectile)
		{
			showRecoil = EditorGUILayout.Foldout(showRecoil, "Recoil");
			if (showRecoil)
			{
				weapon.weaponData.recoil = EditorGUILayout.Toggle("Recoil", weapon.weaponData.recoil);

				if (weapon.weaponData.recoil)
				{
					weapon.weaponData.recoilKickBackMin = EditorGUILayout.FloatField("Recoil Move Min", weapon.weaponData.recoilKickBackMin);
					weapon.weaponData.recoilKickBackMax = EditorGUILayout.FloatField("Recoil Move Max", weapon.weaponData.recoilKickBackMax);
					weapon.weaponData.recoilRotationMin = EditorGUILayout.FloatField("Recoil Rotation Min", weapon.weaponData.recoilRotationMin);
					weapon.weaponData.recoilRotationMax = EditorGUILayout.FloatField("Recoil Rotation Max", weapon.weaponData.recoilRotationMax);
					weapon.weaponData.recoilRecoveryRate = EditorGUILayout.FloatField("Recoil Recovery Rate", weapon.weaponData.recoilRecoveryRate);
				}
			}
		}


		// Shells
		showEffects = EditorGUILayout.Foldout(showEffects, "Effects");
		if (showEffects)
		{
			weapon.weaponData.spitShells = EditorGUILayout.Toggle("Spit Shells", weapon.weaponData.spitShells);
			if (weapon.weaponData.spitShells)
			{
				weapon.weaponData.shell = (GameObject)EditorGUILayout.ObjectField("Shell", weapon.weaponData.shell, typeof(GameObject), false);
				weapon.weaponData.shellSpitForce = EditorGUILayout.FloatField("Shell Spit Force", weapon.weaponData.shellSpitForce);
				weapon.weaponData.shellForceRandom = EditorGUILayout.FloatField("Force Variant", weapon.weaponData.shellForceRandom);
				weapon.weaponData.shellSpitTorqueX = EditorGUILayout.FloatField("X Torque", weapon.weaponData.shellSpitTorqueX);
				weapon.weaponData.shellSpitTorqueY = EditorGUILayout.FloatField("Y Torque", weapon.weaponData.shellSpitTorqueY);
				weapon.weaponData.shellTorqueRandom = EditorGUILayout.FloatField("Torque Variant", weapon.weaponData.shellTorqueRandom);
				weapon.weaponData.shellSpitPosition = (Transform)EditorGUILayout.ObjectField("Shell Spit Point", weapon.weaponData.shellSpitPosition, typeof(Transform), true);
			}

			// Muzzle FX
			EditorGUILayout.Separator();
			weapon.weaponData.makeMuzzleEffects = EditorGUILayout.Toggle("Muzzle Effects", weapon.weaponData.makeMuzzleEffects);
			if (weapon.weaponData.makeMuzzleEffects)
			{
				weapon.weaponData.muzzleEffectsPosition = (Transform)EditorGUILayout.ObjectField("Muzzle FX Spawn Point", weapon.weaponData.muzzleEffectsPosition, typeof(Transform), true);

				if (GUILayout.Button("Add"))
				{
					List<GameObject> temp = new List<GameObject>(weapon.weaponData.muzzleEffects);
					temp.Add(null);
					weapon.weaponData.muzzleEffects = temp.ToArray();
				}
				EditorGUI.indentLevel++;
				for (int i = 0; i < weapon.weaponData.muzzleEffects.Length; i++)
				{
					EditorGUILayout.BeginHorizontal();
					weapon.weaponData.muzzleEffects[i] = (GameObject)EditorGUILayout.ObjectField("Muzzle FX Prefabs", weapon.weaponData.muzzleEffects[i], typeof(GameObject), false);
					if (GUILayout.Button("Remove"))
					{
						List<GameObject> temp = new List<GameObject>(weapon.weaponData.muzzleEffects);
						temp.Remove(temp[i]);
						weapon.weaponData.muzzleEffects = temp.ToArray();
					}
					EditorGUILayout.EndHorizontal();
				}
				EditorGUI.indentLevel--;
			}

			// Hit FX
			if (weapon.weaponData.type != WeaponType.Projectile)
			{
				EditorGUILayout.Separator();
				weapon.weaponData.makeHitEffects = EditorGUILayout.Toggle("Hit Effects", weapon.weaponData.makeHitEffects);
				if (weapon.weaponData.makeHitEffects)
				{
					if (GUILayout.Button("Add"))
					{
						List<GameObject> temp = new List<GameObject>(weapon.weaponData.hitEffects);
						temp.Add(null);
						weapon.weaponData.hitEffects = temp.ToArray();
					}
					EditorGUI.indentLevel++;
					for (int i = 0; i < weapon.weaponData.hitEffects.Length; i++)
					{
						EditorGUILayout.BeginHorizontal();
						weapon.weaponData.hitEffects[i] = (GameObject)EditorGUILayout.ObjectField("Hit FX Prefabs", weapon.weaponData.hitEffects[i], typeof(GameObject), false);
						if (GUILayout.Button("Remove"))
						{
							List<GameObject> temp = new List<GameObject>(weapon.weaponData.hitEffects);
							temp.Remove(temp[i]);
							weapon.weaponData.hitEffects = temp.ToArray();
						}
						EditorGUILayout.EndHorizontal();
					}
					EditorGUI.indentLevel--;
				}
			}
		}


		// Bullet Holes
		if (weapon.weaponData.type == WeaponType.Raycast)
		{
			showBulletHoles = EditorGUILayout.Foldout(showBulletHoles, "Bullet Holes");
			if (showBulletHoles)
			{

				weapon.weaponData.makeBulletHoles = EditorGUILayout.Toggle("Bullet Holes", weapon.weaponData.makeBulletHoles);

				if (weapon.weaponData.makeBulletHoles)
				{
					weapon.weaponData.bhSystem = (BulletHoleSystem)EditorGUILayout.EnumPopup("Determined By", weapon.weaponData.bhSystem);

					if (GUILayout.Button("Add"))
					{
						weapon.weaponData.bulletHoleGroups.Add(new SmartBulletHoleGroup());
						weapon.weaponData.bulletHolePoolNames.Add("Default");
					}

					EditorGUILayout.BeginVertical();

					for (int i = 0; i < weapon.weaponData.bulletHolePoolNames.Count; i++)
					{
						EditorGUILayout.BeginHorizontal();

						// The tag, material, or physic material by which the bullet hole is determined
						if (weapon.weaponData.bhSystem == BulletHoleSystem.Tag)
						{
							weapon.weaponData.bulletHoleGroups[i].tag = EditorGUILayout.TextField(weapon.weaponData.bulletHoleGroups[i].tag);
						}
						else if (weapon.weaponData.bhSystem == BulletHoleSystem.Material)
						{
							weapon.weaponData.bulletHoleGroups[i].material = (Material)EditorGUILayout.ObjectField(weapon.weaponData.bulletHoleGroups[i].material, typeof(Material), false);
						}
						else if (weapon.weaponData.bhSystem == BulletHoleSystem.Physic_Material)
						{
							weapon.weaponData.bulletHoleGroups[i].physicMaterial = (PhysicMaterial)EditorGUILayout.ObjectField(weapon.weaponData.bulletHoleGroups[i].physicMaterial, typeof(PhysicMaterial), false);
						}

						// The bullet hole to be instantiated for this type
						weapon.weaponData.bulletHolePoolNames[i] = EditorGUILayout.TextField(weapon.weaponData.bulletHolePoolNames[i]);

						if (GUILayout.Button("Remove"))
						{
							weapon.weaponData.bulletHoleGroups.Remove(weapon.weaponData.bulletHoleGroups[i]);
							weapon.weaponData.bulletHolePoolNames.Remove(weapon.weaponData.bulletHolePoolNames[i]);
						}

						EditorGUILayout.EndHorizontal();
					}

					// The default bullet holes to be instantiated when other specifications (above) are not met
					EditorGUILayout.Separator();
					EditorGUILayout.LabelField("Default Bullet Holes");

					if (GUILayout.Button("Add"))
					{
						weapon.weaponData.defaultBulletHoles.Add(null);
						weapon.weaponData.defaultBulletHolePoolNames.Add("Default");
					}

					for (int i = 0; i < weapon.weaponData.defaultBulletHolePoolNames.Count; i++)
					{
						EditorGUILayout.BeginHorizontal();

						weapon.weaponData.defaultBulletHolePoolNames[i] = EditorGUILayout.TextField(weapon.weaponData.defaultBulletHolePoolNames[i]);

						if (GUILayout.Button("Remove"))
						{
							weapon.weaponData.defaultBulletHoles.Remove(weapon.weaponData.defaultBulletHoles[i]);
							weapon.weaponData.defaultBulletHolePoolNames.Remove(weapon.weaponData.defaultBulletHolePoolNames[i]);
							
						}

						EditorGUILayout.EndHorizontal();
					}


					// The exceptions to the bullet hole rules defined in the default bullet holes
					EditorGUILayout.Separator();
					EditorGUILayout.LabelField("Exceptions");

					if (GUILayout.Button("Add"))
					{
						weapon.weaponData.bulletHoleExceptions.Add(new SmartBulletHoleGroup());
					}

					for (int i = 0; i < weapon.weaponData.bulletHoleExceptions.Count; i++)
					{
						EditorGUILayout.BeginHorizontal();

						// The tag, material, or physic material by which the bullet hole is determined
						if (weapon.weaponData.bhSystem == BulletHoleSystem.Tag)
						{
							weapon.weaponData.bulletHoleExceptions[i].tag = EditorGUILayout.TextField(weapon.weaponData.bulletHoleExceptions[i].tag);
						}
						else if (weapon.weaponData.bhSystem == BulletHoleSystem.Material)
						{
							weapon.weaponData.bulletHoleExceptions[i].material = (Material)EditorGUILayout.ObjectField(weapon.weaponData.bulletHoleExceptions[i].material, typeof(Material), false);
						}
						else if (weapon.weaponData.bhSystem == BulletHoleSystem.Physic_Material)
						{
							weapon.weaponData.bulletHoleExceptions[i].physicMaterial = (PhysicMaterial)EditorGUILayout.ObjectField(weapon.weaponData.bulletHoleExceptions[i].physicMaterial, typeof(PhysicMaterial), false);
						}


						if (GUILayout.Button("Remove"))
						{
							weapon.weaponData.bulletHoleExceptions.Remove(weapon.weaponData.bulletHoleExceptions[i]);
						}

						EditorGUILayout.EndHorizontal();
					}


					EditorGUILayout.EndVertical();

					if (weapon.weaponData.bulletHoleGroups.Count > 0)
					{
						EditorGUILayout.HelpBox("Assign bullet hole prefabs corresponding with tags, materials, or physic materials.  If you assign multiple bullet holes to the same parameter, one of them will be chosen at random.  The default bullet hole will be used when something is hit that doesn't match any of the other parameters.  The exceptions define parameters for which no bullet holes will be instantiated.", MessageType.None);
					}
				}

			}
		}


		// Crosshairs
		showCrosshairs = EditorGUILayout.Foldout(showCrosshairs, "Crosshairs");
		if (showCrosshairs)
		{
			weapon.weaponData.showCrosshair = EditorGUILayout.Toggle("Show Crosshairs", weapon.weaponData.showCrosshair);
			if (weapon.weaponData.showCrosshair)
			{
				weapon.weaponData.crosshairTexture = (Texture2D)EditorGUILayout.ObjectField("Crosshair Texture", weapon.weaponData.crosshairTexture, typeof(Texture2D), false);
				weapon.weaponData.crosshairLength = EditorGUILayout.IntField("Crosshair Length", weapon.weaponData.crosshairLength);
				weapon.weaponData.crosshairWidth = EditorGUILayout.IntField("Crosshair Width", weapon.weaponData.crosshairWidth);
				weapon.weaponData.startingCrosshairSize = EditorGUILayout.FloatField("Start Crosshair Size", weapon.weaponData.startingCrosshairSize);
			}
		}
		

		// Audio
		showAudio = EditorGUILayout.Foldout(showAudio, "Audio");
		if (showAudio)
		{
			weapon.weaponData.fireSound = (AudioClip)EditorGUILayout.ObjectField("Fire", weapon.weaponData.fireSound, typeof(AudioClip), false);
			weapon.weaponData.reloadSound = (AudioClip)EditorGUILayout.ObjectField("Reload", weapon.weaponData.reloadSound, typeof(AudioClip), false);
			weapon.weaponData.dryFireSound = (AudioClip)EditorGUILayout.ObjectField("Out of Ammo", weapon.weaponData.dryFireSound, typeof(AudioClip), false);
		}


		// This makes the editor gui re-draw the inspector if values have changed
		if (GUI.changed)
			EditorUtility.SetDirty(target);
	}
	
}

