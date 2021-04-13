using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameClearOverSpika_Process : MonoBehaviour
{

    void Start()
    {
    }

    void Update()
    {
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Asteroid")
        {
        }
    }
}
