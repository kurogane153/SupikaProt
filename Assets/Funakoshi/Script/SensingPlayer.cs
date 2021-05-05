using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensingPlayer : MonoBehaviour
{

    public GameObject PlayerOff;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("触ったよ");
        if (collision.gameObject.tag == "Player")
        {
            Destroy(PlayerOff);
            
        }
    }
}
