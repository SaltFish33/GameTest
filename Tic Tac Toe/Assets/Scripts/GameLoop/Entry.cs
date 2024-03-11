using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Entry : MonoBehaviour
{
    private void Awake()
    {
        GameManager.Instance.Init();

        
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnDestroy();
    }
}
