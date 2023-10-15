using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaybeSpawn : MonoBehaviour
{
    [SerializeField] private GameObject Prefab;

    void Start()
    {
        if(Random.value < 0.1f) {
            GameObject item = Instantiate(Prefab);
            item.transform.SetParent(transform);
            item.transform.localPosition = new Vector3(0, 0, 0);
            item.transform.localScale = new Vector3(transform.localScale.x / 3.0f * item.transform.localScale.x, transform.localScale.x / 3.0f * item.transform.localScale.y, 1);
        }
    }
}
