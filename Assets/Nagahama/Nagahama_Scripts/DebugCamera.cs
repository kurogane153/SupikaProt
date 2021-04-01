using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCamera : MonoBehaviour
{
    #region Singleton
    private static DebugCamera instance;

    public static DebugCamera Instance
    {
        get
        {
            if (instance == null) {
                instance = (DebugCamera)FindObjectOfType(typeof(DebugCamera));

                if (instance == null) {
                    Debug.LogError(typeof(DebugCamera) + "is nothing");
                }
            }

            return instance;
        }
    }
    #endregion

    public float moveSpeed = 10f;
    public float rotateSpeed = 10f;
    private Camera cam;
    private bool isenable;

    public bool IsEnable
    {
        get { return isenable; }
        set
        {
            isenable = value;
            if (value) {
                cam.enabled = true;
            } else {
                cam.enabled = false;
            }
        }
    }


    void Start()
    {
        cam = GetComponent<Camera>();
        cam.enabled = false;
    }

    float sight_x = 0;
    float sight_y = 0;

    void controller()
    {
        float x = Input.GetAxis("Horizontal") * moveSpeed;
        float z = Input.GetAxis("Vertical") * moveSpeed;
        float angleH = Input.GetAxis("RStickHor") * rotateSpeed;
        float angleV = Input.GetAxis("RStickVer") * rotateSpeed;

        if (sight_x >= 360) {
            sight_x = sight_x - 360;
        } else if (sight_x < 0) {
            sight_x = 360 - sight_x;
        }
        sight_x = sight_x + angleH;

        if (sight_y > 80) {
            if (angleV < 0) {
                sight_y = sight_y + angleV;
            }
        } else if (sight_y < -90) {
            if (angleV > 0) {
                sight_y = sight_y + angleV;
            }
        } else {
            sight_y = sight_y + angleV;
        }

        float dx = Mathf.Sin(sight_x * Mathf.Deg2Rad) * z + Mathf.Sin((sight_x + 90f) * Mathf.Deg2Rad) * x;

        float dz = Mathf.Cos(sight_x * Mathf.Deg2Rad) * z + Mathf.Cos((sight_x + 90f) * Mathf.Deg2Rad) * x;

        transform.Translate(dx, 0, dz, 0.0F);

        transform.localRotation = Quaternion.Euler(-sight_y, sight_x, 0);

    }

    void UpDown()
    {
        transform.Translate(0, Input.GetAxis("D_Pad_V") * moveSpeed, 0);
    }

    void Update()
    {
        if (!isenable) return;
        controller();
        UpDown();
    }
}
