using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    [SerializeField] float delayTime = 5f;
    
    private void Start()
    {
        Destroy(gameObject, delayTime);
    }
}
