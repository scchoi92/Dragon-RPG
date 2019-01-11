using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    [SerializeField] float delayTime = 5f;
    // Update is called once per frame
    private void Start()
    {
        Destroy(gameObject, delayTime);
    }
}
