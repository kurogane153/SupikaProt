using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotation : MonoBehaviour
{
    void FixedUpdate()
    {
        float x = 0.2f;
        this.transform.Rotate(0, x, 0);
    }
}
