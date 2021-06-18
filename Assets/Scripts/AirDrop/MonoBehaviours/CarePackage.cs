using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarePackage : CarePackageUtility
{
    public CarePackage(CarePackageType _type)
    {
        airDropType = _type;
    }

    public CarePackage(CarePackage _airDropUtility)
    {
        dropId = _airDropUtility.dropId;
        activeTime = _airDropUtility.activeTime;
        airDropType = _airDropUtility.airDropType;
        useRandomActiveTime = _airDropUtility.useRandomActiveTime;
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    public override int AirDropCollected()
    {
        return base.AirDropCollected();
    }

    protected override IEnumerator DespawnAirDrop(float _delay)
    {
        return base.DespawnAirDrop(_delay);
    }

    public override void TriggerEnter()
    {
        base.TriggerEnter();
    }

}
