using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnItem : MonoBehaviour
{
    [SerializeField] private GameObject[] optionsPrefabs;

    void Start()
    {
        GameObject item = Instantiate(optionsPrefabs[Random.Range(0, optionsPrefabs.Length)]);
        item.transform.SetParent(transform);
        item.transform.localPosition = new Vector3(Random.Range(-0.15f, 0.15f), Random.Range(-0.15f, 0.15f), 0);
        item.transform.localScale = new Vector3(transform.localScale.x / 3.0f * item.transform.localScale.x, transform.localScale.x / 3.0f * item.transform.localScale.y, 1);
    }
}
