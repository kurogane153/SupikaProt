using UnityEngine;
using UnityEngine.UI;

public class AlertFlush : MonoBehaviour
{
    Image img;
    Animator anim;

    void Start()
    {
        img = GetComponent<Image>();
        anim = GetComponent<Animator>();
        img.color = Color.clear;
        anim.SetBool("AretKey", false);
    }

    void Update()
    {
        if (AlertLine._alertflg)
        {
            OnAlert();
        }
        else
        {
            OffAlert();
        }
    }

    public void OnAlert()
    {
        anim.SetBool("AretKey", true);
    }
    public void OffAlert()
    {
        anim.SetBool("AretKey", false);
    }
}