using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Care Package", menuName = "Assets/ScriptableObject/Carepackage")]
public class CarePackage_SO : ScriptableObject
{
    public enum CarePackageType { WEAPON, EQUIPMENT, TACTICAL }
    public CarePackageType carePackageType;

    public enum CarePackageTier { COMMON, RARE, EXOTIC, SECRET }
    public CarePackageTier carePackageTier;

    [EnumToggleButtons]
    public enum LockMoveAxis { X, Y, Z }
    public LockMoveAxis lockMoveAxis;

    //public List<LayerMask> collisionLayers = new List<LayerMask>();
    public List<LayerMask> interactionLayers = new List<LayerMask>();

    public int carePackageID;
    public string carePackageName;

    // These belong in a manager script that will asign these values to all care packages
    public Vector3 carePackageTriggerSize = new Vector3(1.25f, 1.25f, 1.25f);

    public Transform carePackageHolder;
    public Transform spawnPoint;

    [PropertyRange(2, 10)]
    public int fracturedActiveTime = 10;
    public bool useFracturedRandActiveTime = false;

    [PropertyRange(5, 30)]
    public int activeTime = 10;
    public bool useRandActiveTime = false;



    [AssetsOnly]
    public Image carePackageImage;
    [AssetsOnly]
    public List<Material> carePackageMaterials = new List<Material>();
    [AssetsOnly]
    public List<GameObject> carePackageModels = new List<GameObject>();
    [AssetsOnly]
    public List<GameObject> fracturedModels = new List<GameObject>();

    #region Effects Scriptable Objects
    // Consider making an audioclip_SO. Will allow us to independently set EQ values for SFX;
    [Space(10)]
    [TitleGroup("Effects")]

    public DestroyAfterTime_SO destroyAfterTime;
    #region SFX
    [Title("Sound Effects")]
    [Title("Spawn/Despawn")]
    [AssetsOnly]
    public SoundEffect_SO spawnSFX;
    [AssetsOnly]
    public SoundEffect_SO despawnSFX;
    [AssetsOnly]
    public SoundEffect_SO destroySFX;


    [Space(5)] [Title("Collision/Collection")]
    [AssetsOnly]
    public SoundEffect_SO collisionEnterSFX;
    [AssetsOnly]
    public SoundEffect_SO collisionExitSFX;
    [AssetsOnly]
    public SoundEffect_SO collectSFX;
    #endregion SFX

    #region VFX
    [Space(20)] [Title("Visual Effects")]
    [Title("Spawn/Despawn")]
    // Consider making an particleSystem_SO. Will allow us to independently control particles and reuse them;
    [AssetsOnly]
    public VisualEffect_SO spawnVFX;
    [AssetsOnly]
    public VisualEffect_SO despawnVFX;
    [AssetsOnly]
    public VisualEffect_SO destroyVFX;

    [Space(5)]  [Title("Collision/Collection")]
    [AssetsOnly]
    public VisualEffect_SO collisionEnterVFX;
    [AssetsOnly]
    public VisualEffect_SO collisionExitVFX;
    [AssetsOnly]
    public VisualEffect_SO collectVFX;
    #endregion
#endregion

    [Space(10)] [TitleGroup("Debug Settings")]
    public bool cpManagerOverrideDebugs = false;
    [Space(5)]  [Title("Debug Collision")]
    public bool debugCollision = false;

    public IEnumerator Interaction = null;
    public bool interactionRunning = false;

    public bool IsGrounded(CarePackage _carePackage)
    {
        return false;
    }

    #region Initialization
    private void InitializeComponents(CarePackage _carePackage)
    {
        InitializeAudioSource(_carePackage);
        InitializeRigidBody(_carePackage);
        InitializeCollider(_carePackage);
        InitializeModel(_carePackage);
    }

    /// <summary>
    /// Initialize audio source component
    /// </summary>
    /// <param name="_carePackage"></param>
    private void InitializeAudioSource(CarePackage _carePackage)
    {
        if (_carePackage.audioSource == null)
        {
            _carePackage.audioSource = GetAudioSource(_carePackage);
        }
    }

    /// <summary>
    /// Initialize rigidbody component
    /// </summary>
    /// <param name="_carePackage"></param>
    private void InitializeRigidBody(CarePackage _carePackage)
    {
        if (_carePackage.rBody == null)
        {
            _carePackage.rBody = _carePackage.GetComponent<Rigidbody>();
        }

        switch (lockMoveAxis)
        {
            case LockMoveAxis.X:
                _carePackage.rBody.constraints = RigidbodyConstraints.FreezePositionX;
                break;
            case LockMoveAxis.Y:
                _carePackage.rBody.constraints = RigidbodyConstraints.FreezePositionY;
                break;
            case LockMoveAxis.Z:
                _carePackage.rBody.constraints = RigidbodyConstraints.FreezePositionZ;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Initialize visual model
    /// </summary>
    /// <param name="_carePackage"></param>
    private void InitializeModel(CarePackage _carePackage)
    {
        if (_carePackage.transform.childCount > 0)
        {
            _carePackage.model = _carePackage.transform.GetChild(0).gameObject;
        }
    }

    /// <summary>
    /// Initialize collider compenent
    /// </summary>
    /// <param name="_carePackage"></param>
    private void InitializeCollider(CarePackage _carePackage)
    {
        if (_carePackage.bCollider == null)
        {
            _carePackage.bCollider = _carePackage.GetComponent<BoxCollider>();
            _carePackage.bCollider.isTrigger = true;
            _carePackage.bCollider.size = carePackageTriggerSize;
        }
    }

    /// <summary>
    /// Generate a new care package
    /// </summary>
    /// <returns></returns>
    public CarePackage GenerateCarePackage()
    {
        GameObject cp = new GameObject(name);
        cp.transform.parent = carePackageHolder;
        cp.transform.localPosition = Vector3.zero;
        cp.transform.localRotation = Quaternion.Euler(Vector3.zero);

        GameObject cpModel = GameObject.Instantiate(carePackageModels[(int)carePackageTier]);
        //cpModel.GetComponent<Renderer>().material = carePackageMaterials[(int)carePackageTier];

        cpModel.transform.parent = cp.transform;
        cpModel.transform.localPosition = Vector3.zero;
        cpModel.transform.localRotation = Quaternion.Euler(Vector3.zero);

        if (cp.GetComponent<CarePackage>() == null)
        {
            cp.AddComponent<CarePackage>();
            cp.GetComponent<CarePackage>().cpData = this;
            InitializeComponents(cp.GetComponent<CarePackage>());
        }

        return cp.GetComponent<CarePackage>();
    }

    /// <summary>
    /// Generate a new fractured care package model
    /// </summary>
    /// <param name="_carePackage"></param>
    public void GenerateFracturedModel(CarePackage _carePackage)
    {
        int rand = Random.Range(0, 0);

        GameObject temp = GameObject.Instantiate(fracturedModels[rand]);
        temp.name = fracturedModels[rand].name;
        temp.transform.position = _carePackage.transform.position;
        temp.transform.rotation = _carePackage.transform.rotation;
        _carePackage.destroyedModel = temp;

        destroyAfterTime.Destroy(GetMonoBehaviour(_carePackage), temp, fracturedActiveTime);
    }
    #endregion

    #region Collision / Trigger Detection

    // Valid collision check not being used atm. May want to revisist this change it so it checks what layer the collision is on and 
    // produce a unique effect based on the result. An example could be, play a different sfx depending on collision layer. 
    ///// <summary>
    ///// Query whether collision is valid or not.
    ///// </summary>
    ///// <param name="_collision"></param>
    ///// <returns></returns>
    //public bool IsValidCollision(Collision _collision)
    //{
    //    Statics.CustomDebug(Statics.DebugMode.DEBUG, debugCollision, "Collision obj layer is: " + _collision.gameObject.layer);

    //    if (collisionLayers.Count > 0)
    //    {
    //        for (int i = 0; i < collisionLayers.Count; i++)
    //        {
    //            if (collisionLayers[i] == (collisionLayers[i] | (1 << _collision.gameObject.layer)))
    //            {
    //                Statics.CustomDebug(Statics.DebugMode.DEBUG, debugCollision, "Valid collision detected with: " + _collision.gameObject.name);

    //                return true;
    //            }
    //        }
    //    }
    //    else
    //    {
    //        Statics.CustomDebug(Statics.DebugMode.DEBUG, debugCollision, "No validation layer defined, will collide with any object");
    //        Statics.CustomDebug(Statics.DebugMode.DEBUG, debugCollision, "Valid collision detected with: " + _collision.gameObject.name);

    //        return true;
    //    }

    //    Statics.CustomDebug(Statics.DebugMode.DEBUG, debugCollision, "No validation layer defined, will collide with any object");
    //    Statics.CustomDebug(Statics.DebugMode.DEBUG, debugCollision, "Valid collision detected with: " + _collision.gameObject.name);

    //    return false;
    //}

    /// <summary>
    /// Query whether trigger is valid or not
    /// </summary>
    /// <param name="_collider"></param>
    /// <returns></returns>
    public bool IsValidTrigger(Collider _collider)
    {
        Statics.CustomDebug(Statics.DebugMode.DEBUG, debugCollision, "Trigger obj layer is: " + _collider.gameObject.layer);

        if (interactionLayers.Count > 0)
        {
            for (int i = 0; i < interactionLayers.Count; i++)
            {
                if (interactionLayers[i] == (interactionLayers[i] | (1 << _collider.gameObject.layer)))
                {
                    Statics.CustomDebug(Statics.DebugMode.DEBUG, debugCollision, "Valid trigger detected with: " + _collider.gameObject.name);

                    return true;
                }
            }
        }
        else
        {
            Statics.CustomDebug(Statics.DebugMode.WARNING, debugCollision, "No validation layer defined, will not trigger");
            return false;
        }

        Statics.CustomDebug(Statics.DebugMode.WARNING, debugCollision, "No validation layer defined, will not trigger");
        return false;
    }

    #endregion


    public void SpawnCarePackage(CarePackage _carePackage)
    {
        spawnSFX.PlaySFX(GetAudioSource(_carePackage));
       // spawnVFX.Play();
    }

    public void CollisionEnterCarePackage(CarePackage _carePackage, Vector3 _collisionPoint)
    {
        PlayVFX(collisionEnterVFX, _carePackage, _collisionPoint);
        PlaySFX(collisionEnterSFX, _carePackage);

        if (_carePackage.interactable == false)
        {
            DespawnCarePackage(_carePackage);
        }
    }

    public void CollisionExitCarePackage(CarePackage _carePackage, Vector3 _collisionPoint)
    {
        PlaySFX(collisionExitSFX, _carePackage);
        PlayVFX(collisionExitVFX, _carePackage, _collisionPoint);
    }

    public void DestroyCarePackage(CarePackage _carePackage)
    {
        destroySFX.PlaySFX(GetAudioSource(_carePackage));
        //destroyVFX.Play();
    }

    public void CollectCarePackage(CarePackage _carePackage)
    {
        StopCoroutine(_carePackage);

        GenerateFracturedModel(_carePackage);
        DeActivateCarePackage(_carePackage);

        PlaySFX(destroySFX, _carePackage);
        PlaySFX(collectSFX, _carePackage);
        PlayVFX(collectVFX, _carePackage, GetMeshRenderer(_carePackage).bounds.center);

        TransformCarePackage(_carePackage, carePackageHolder.position);
    }


    public void DespawnCarePackage(CarePackage _carePackage)
    {
        StopCoroutine(_carePackage);

        Interaction = DespawnCarePackageAfterDelay(_carePackage);
        GetMonoBehaviour(_carePackage).StartCoroutine(Interaction);
    }

    public IEnumerator DespawnCarePackageAfterDelay(CarePackage _carePackage)
    {
        interactionRunning = true;

        if (useRandActiveTime)
        {
            int rand = Random.Range(5, 30);
            yield return new WaitForSecondsRealtime(rand);
        }
        else
        {
            yield return new WaitForSecondsRealtime(activeTime - 0.25f);
        }

        GenerateFracturedModel(_carePackage);
        DeActivateCarePackage(_carePackage);

        PlaySFX(destroySFX, _carePackage);
        PlaySFX(despawnSFX, _carePackage);

        PlayVFX(destroyVFX, _carePackage, GetMeshRenderer(_carePackage).bounds.center);

        //despawnVFX.Play();

        TransformCarePackage(_carePackage, carePackageHolder.position);

        interactionRunning = false;
    }

    #region SFX/VFX Functions
    public void PlaySFX(SoundEffect_SO _sfx, CarePackage _carePackage)
    {
        _sfx.PlaySFX(GetAudioSource(_carePackage));
    }

    public void PlayVFX(VisualEffect_SO _vfx, CarePackage _carePackage, Vector3 _spawnPoint)
    {
        // Need to have a look at this, shouldnt need an if statement. 
        int rand = Random.Range(0, _vfx.particleSystems.Count);

        if (_carePackage.destroyedModel != null)
        {
            _vfx.Activate(_vfx.GenerateParticle(rand, _carePackage.destroyedModel.transform, _spawnPoint));
        }
        else
        {
            _vfx.Activate(_vfx.GenerateParticle(rand, _carePackage.model.transform, _spawnPoint));
        }
    }
    #endregion

    #region Utility Functions
    /// <summary>
    /// Enables colliders, mesh renderers & disables kinematic rigidbody
    /// </summary>
    /// <param name="_carePackage"></param>
    public void ActivateCarePackage(CarePackage _carePackage)
    {
        GetCollisionCollider(_carePackage).enabled = true;
        GetTriggerCollider(_carePackage).enabled = true;
        GetRigidBody(_carePackage).isKinematic = false;
        GetMeshRenderer(_carePackage).enabled = true;
    }

    /// <summary>
    /// Disables colliders, mesh renderers & enables kinematic rigidbody
    /// </summary>
    /// <param name="_carePackage"></param>
    public void DeActivateCarePackage(CarePackage _carePackage)
    {
        GetCollisionCollider(_carePackage).enabled = false;
        GetTriggerCollider(_carePackage).enabled = false;
        GetRigidBody(_carePackage).isKinematic = true;
        GetMeshRenderer(_carePackage).enabled = false;
    }

    public void StopCoroutine(CarePackage _carePackage)
    {
        if (Interaction != null)
        {
            if (interactionRunning)
            {
                GetMonoBehaviour(_carePackage).StopCoroutine(Interaction);
                interactionRunning = false;
            }
        }
    }
    #endregion

    /// <summary>
    /// Moves the Care Package to a target Vector 3 Position
    /// </summary>
    /// <param name="_carePackage"></param>
    /// <param name="_targetPos"></param>
    public void TransformCarePackage(CarePackage _carePackage, Vector3 _targetPos)
    {
        _carePackage.transform.position = _targetPos;
    }

    #region GetComponents Functions

    /// <summary>
    /// Returns the rigid body component of the care package
    /// </summary>
    /// <param name="_carePackage"></param>
    /// <returns></returns>
    public Rigidbody GetRigidBody(CarePackage _carePackage)
    {
        return _carePackage.GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Returns the collider component to the care package
    /// </summary>
    /// <param name="_carePackage"></param>
    /// <returns></returns>
    public Collider GetTriggerCollider(CarePackage _carePackage)
    {
        return _carePackage.GetComponent<Collider>();
    }

    /// <summary>
    /// Returns the collider component of the care package model
    /// </summary>
    /// <param name="_carePackage"></param>
    /// <returns></returns>
    public Collider GetCollisionCollider(CarePackage _carePackage)
    {
        return _carePackage.model.GetComponent<Collider>();
    }

    /// <summary>
    /// Returns the collider component of the care package
    /// </summary>
    /// <param name="_carePackage"></param>
    /// <returns></returns>
    public MonoBehaviour GetMonoBehaviour(CarePackage _carePackage)
    {
        return _carePackage.GetComponent<MonoBehaviour>();
    }

    /// <summary>
    /// Returns the audio source component of the care package
    /// </summary>
    /// <param name="_carePackage"></param>
    /// <returns></returns>
    public AudioSource GetAudioSource(CarePackage _carePackage)
    {
        return _carePackage.GetComponent<AudioSource>();
    }

    /// <summary>
    /// Returns the mesh renderer component of the care package model
    /// </summary>
    /// <param name="_carePackage"></param>
    /// <returns></returns>
    public MeshRenderer GetMeshRenderer(CarePackage _carePackage)
    {
        return _carePackage.model.GetComponent<MeshRenderer>();
    }
    #endregion
}
