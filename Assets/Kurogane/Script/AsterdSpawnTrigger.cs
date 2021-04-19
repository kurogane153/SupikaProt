using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsterdSpawnTrigger : MonoBehaviour
{

    private bool _areaflg;

    void Start()
    {
        _areaflg = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            _areaflg = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            _areaflg = false;
        }
    }

    public bool GetAreaTrigger()
    {
        return _areaflg;
    }

}
