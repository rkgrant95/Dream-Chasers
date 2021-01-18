using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLocatorUtility : MonoBehaviour
{
    public int enemiesPresent;
    public GameObject centralPointIndicator;
    public List<Transform> enemyTransforms;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<EnemyHealth>() && !enemyTransforms.Contains(other.transform))
            enemyTransforms.Add(other.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<EnemyHealth>() && enemyTransforms.Contains(other.transform))
            enemyTransforms.Remove(other.transform);
    }

    private void Start()
    {
        StartCoroutine(UpdateAveragePosition());
    }

    private IEnumerator UpdateAveragePosition()
    {
        while (gameObject.activeInHierarchy)
        {
            yield return new WaitForSeconds(1);

            centralPointIndicator.transform.position = GetAverageEnemyPosition();
        }

    }

    private Vector3 GetAverageEnemyPosition()
    {
        Vector3 retVal = Vector3.zero;

        if (enemyTransforms.Count > 0)
        {
            for (int i = 0; i < enemyTransforms.Count; i++)
                retVal += enemyTransforms[i].transform.position;

            retVal /= enemyTransforms.Count;
        }
        else
        {
            retVal = transform.position;
        }

        retVal.y = 12.5f;

        return retVal;
    }

}
