using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Destroy GO After Time", menuName = "Assets/ScriptableObject/Utility/Destroy After Time")]
public class DestroyAfterTime_SO : ScriptableObject
{
    public void Destroy(MonoBehaviour _mono, GameObject _toDestroy, int _activeTime)
    {
        _mono.StartCoroutine(DestroyCo(_mono, _toDestroy, _activeTime));
    }
    public IEnumerator DestroyCo(MonoBehaviour _mono, GameObject _toDestroy, int _activeTime)
    {
        yield return new WaitForSecondsRealtime(_activeTime);

        Destroy(_toDestroy);
    }
}
