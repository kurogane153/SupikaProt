using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void SetTrigger(string name)
    {
        animator.SetTrigger(name);
    }

    public void AcrobatLoop()
    {
        animator.SetTrigger("AcrobatLoop");
    }

    public void Spiral()
    {
        animator.SetTrigger("Spiral");
    }

    public void ChangeOrbit()
    {
        animator.SetTrigger("ChangeOrbit");
    }
}
