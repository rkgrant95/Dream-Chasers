using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public static class Statics
{
   public enum DebugMode { DEBUG, WARNING, ERROR }

    public static void CustomDebug(Statics.DebugMode _debugMode, bool _allowDebug, string _debugMsg = "")
    {
        if (_allowDebug)
        {
            switch (_debugMode)
            {
                case Statics.DebugMode.DEBUG:
                    Debug.Log(_debugMsg);
                    break;
                case Statics.DebugMode.WARNING:
                    Debug.LogWarning(_debugMsg);
                    break;
                case Statics.DebugMode.ERROR:
                    Debug.LogError(_debugMsg);
                    break;
                default:
                    break;
            }
        }
    }

    public static Vector3 GetAverageCollisionPoint(Collision _collision)
    {
        List<Vector3> contacts = new List<Vector3>();

        for (int i = 0; i < _collision.contactCount; i++)
        {
            contacts.Add(_collision.GetContact(i).point);
        }

        return GetAverageVector(contacts);
    }

    public static Vector3 GetAverageVector(List<Vector3> _list)
    {
        if (_list.Count > 1)
        {
            Vector3 newVector = new Vector3(
            _list.Average(x => x.x),
            _list.Average(x => x.y),
            _list.Average(x => x.z));

            return newVector;
        }
        else
        {
            if (_list.Count > 0)
            {
                return _list[0];
            }
        }

        return Vector3.zero;
    }
}
