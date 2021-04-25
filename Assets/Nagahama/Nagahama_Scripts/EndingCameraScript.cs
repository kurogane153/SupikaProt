using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingCameraScript : MonoBehaviour
{
    public bool rotflg = false;
    public bool followflg = false;
    public Transform target;
    public Transform explosionViwePos;
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

        if(followflg) {
            Vector3 desiredPosition = target.position + new Vector3(0, 10, -10);
            transform.position = Vector3.Lerp(transform.position, desiredPosition, 0.319f);
        }
    }

    private IEnumerator TimeScaleDown()
    {
        yield return new WaitForSeconds(1.75f);
        Time.timeScale = 0.2f;
        rotflg = true;
        yield return new WaitForSeconds(2 * 0.2f);
        Time.timeScale = 1f;
        rotflg = false;
        transform.rotation = target.rotation;
        transform.Rotate(15, 180, 0);

        Vector3 desiredPosition = target.position + new Vector3(0, 10, -5);
        transform.position = desiredPosition;
        followflg = true;

        yield return new WaitForSeconds(2f);
        followflg = false;
        transform.position = explosionViwePos.position;
        transform.rotation = explosionViwePos.rotation;
    }
}
