using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CarePackageFlightIndicatorUtility
{
    public enum IndicatorState { Backdrop, AirDrop, }
    public IndicatorState indicatorState;

    public string name;

    public CarePackageFlightIndicatorUtility(string _name)
    {
        name = _name;
        indicatorState = IndicatorState.Backdrop;
    }

    /// <summary>
    /// Updates indicator position to default settings
    /// </summary>
    /// <param name="_thisTransform"></param>
    public void UpdateSpawnDefault(Transform _thisTransform)
    {
        switch (indicatorState)
        {
            case IndicatorState.Backdrop:
                _thisTransform.position = GameManager.Instance.AirDropManager.utility.ReturnRandPosInBackdropBounds(GameManager.Instance.AirDropManager.utility.backDropExtentsCollider, 0.75f, (int)Random.Range(15, 40));
                break;
            case IndicatorState.AirDrop:
                _thisTransform.position = GameManager.Instance.AirDropManager.utility.ReturnRandPosInAirdropBounds(GameManager.Instance.AirDropManager.utility.airDropExtentsCollider, 0.75f, (int)Random.Range(15, 40));
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Updates indicator postion on contact with air drop flyer
    /// </summary>
    /// <param name="_thisTransform"></param>
    public void UpdateSpawnOnTrigger(Transform _thisTransform)
    {

        switch (indicatorState)
        {
            case IndicatorState.Backdrop:
                _thisTransform.position = GameManager.Instance.AirDropManager.utility.ReturnRandPosInBackdropBounds(GameManager.Instance.AirDropManager.utility.backDropExtentsCollider, 0.75f, (int)Random.Range(15, 40));
                break;
            case IndicatorState.AirDrop:
                GameManager.Instance.AirDropManager.utility.SpawnAirDrop(_thisTransform);
                _thisTransform.position = GameManager.Instance.AirDropManager.utility.ReturnRandPosInAirdropBounds(GameManager.Instance.AirDropManager.utility.backDropExtentsCollider, 0.75f, (int)Random.Range(15, 40));
                indicatorState = IndicatorState.Backdrop;
                break;
            default:
                break;
        }
    }



}
