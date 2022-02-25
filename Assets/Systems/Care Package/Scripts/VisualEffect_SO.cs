using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "VFX - NEW NAME", menuName = "Assets/ScriptableObject/VisualEffect")]
public class VisualEffect_SO : ScriptableObject
{
    public string particleSystemName = "New Particle system";
    [Title("Particle Systems")]
    public List<GameObject> particleSystems;

    public ParticleSystemScalingMode scalingMode = ParticleSystemScalingMode.Hierarchy;

    public int activeTime = 2;
    public Vector3 particleScale = new Vector3(0.05f, 0.05f, 0.05f);
    public Vector3 particleOffset = new Vector3(0, 0, 0);

    private void ChangeScalingMode()
    {
        
    }

    /// <summary>
    /// Generate a holder for the vfx
    /// </summary>
    /// <param name="_parentObj"></param>
    /// <returns></returns>
    public GameObject GenerateHolder(Transform _parentObj = null, Vector3 _spawnPos = default(Vector3))
    {
        GameObject holder = new GameObject();
        holder.name = particleSystemName;

        SetParent(holder.transform, _parentObj);
        SetTransformParticleGlobal(holder.transform, _spawnPos + particleOffset);
        ResetRotationParticleLocal(holder.transform);
        Deactivate(holder);
        return holder;
    }

    /// <summary>
    /// Generate a vfx and parent it to a holder
    /// </summary>
    /// <param name="_index"></param>
    /// <param name="_parentObj"></param>
    /// <returns></returns>
    public GameObject GenerateParticle(int _index, Transform _parentObj, Vector3 _spawnPoint = default(Vector3))
    {
        GameObject holder = GenerateHolder(_parentObj, _spawnPoint);

        GameObject temp = GameObject.Instantiate(particleSystems[_index]);
        SetParent(temp.transform, holder.transform);
        ResetScale(temp.transform);
        //ResetTransformParticleLocal(temp.transform);

        SetTransformParticleGlobal(temp.transform, _spawnPoint);
        ResetRotationParticleLocal(temp.transform);

        return holder;
    }

    /// <summary>
    /// Generate multiple vfx and parent them to a holder
    /// </summary>
    /// <param name="_parentObj"></param>
    /// <returns></returns>
    public GameObject GenerateParticles(Transform _parentObj)
    {
        GameObject holder = GenerateHolder(_parentObj);

        for (int i = 0; i < particleSystems.Count; i++)
        {
            GameObject temp = GameObject.Instantiate(particleSystems[i]);
            SetParent(temp.transform, holder.transform);
            ResetScale(temp.transform);
            ResetTransformParticleLocal(temp.transform);
            ResetRotationParticleLocal(temp.transform);
        }


        return holder;
    }

    #region Activate/Deactivate
    public void Activate(GameObject _obj)
    {
        _obj.SetActive(true);
    }

    public void Deactivate(GameObject _obj)
    {
        _obj.SetActive(false);
    }
    #endregion

    #region Transform
    public void SetParent(Transform _particle, Transform _parent)
    {
        _particle.parent = _parent;
    }

    #region Global Transform
    public void SetTransformParticleGlobal(Transform _particle, Vector3 _toPos)
    {
        _particle.position = _toPos;
    }

    public void ResetTransformParticleGlobal(Transform _particle)
    {
        _particle.localPosition = Vector3.zero;
    }
    #endregion

    #region Local Transform
    public void SetTransformParticleLocal(Transform _particle, Vector3 _toPos)
    {
        _particle.localPosition = _toPos;
    }

    public void ResetTransformParticleLocal(Transform _particle)
    {
        _particle.localPosition = Vector3.zero;
    }
    #endregion
    #endregion

    #region Scale
    public void SetScale(Transform _transform, Vector3 _toScale)
    {
        _transform.localScale = _toScale;
    }

    public void ResetScale(Transform _transform)
    {
        _transform.localScale = particleScale;
    }
    #endregion

    #region Rotation
    #region Global Rotation
    public void SetRotationParticleGlobal(Transform _particle, Vector3 _toRot)
    {
        _particle.rotation = Quaternion.Euler(_toRot);
    }

    public void ResetRotationParticleGlobal(Transform _particle)
    {
        _particle.rotation = Quaternion.Euler(Vector3.zero);
    }
    #endregion

    #region LocalRotation
    public void SetRotationParticleLocal(Transform _particle, Vector3 _toRot)
    {
        _particle.localRotation = Quaternion.Euler(_toRot); 
    }

    public void ResetRotationParticleLocal(Transform _particle)
    {
        _particle.localRotation = Quaternion.Euler(Vector3.zero);
    }
    #endregion
    #endregion
}
