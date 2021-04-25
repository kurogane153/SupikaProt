using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingCameraScript : MonoBehaviour
{
    public bool rotflg = false;
    public Transform target;
    public float speed = 2f;
    float step;

    void Start()
    {
        StartCoroutine(nameof(TimeScaleDown));
    }

    void Update()
    {
        if (rotflg) {
            if(target == null) {
                rotflg = false;
                return;
            }
            Vector3 targetDir = target.position - transform.position;
            targetDir.y = transform.position.y; //targetと高さが異なると体ごと上下を向いてしまうので制御
            float step = speed * Time.deltaTime;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 10.0F);
            transform.rotation = Quaternion.LookRotation(newDir);
        }
    }

    private IEnumerator TimeScaleDown()
    {
        yield return new WaitForSeconds(1.75f);
        Time.timeScale = 0.2f;
        rotflg = true;
        yield return new WaitForSeconds(2 * 0.2f);
        Time.timeScale = 1f;
    }
}
