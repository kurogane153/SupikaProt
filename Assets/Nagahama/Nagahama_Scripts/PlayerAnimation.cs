using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    private float animationWaitTime;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (0f < animationWaitTime) {
            animationWaitTime -= Time.deltaTime;
        }
    }

    public void SetTrigger(string name, float animTime)
    {
        if (animationWaitTime <= 0) {
            animator.SetTrigger(name);
        }
        
        animationWaitTime = animTime;
    }

    public void AcrobatLoop(float animTime)
    {
        SetTrigger("AcrobatLoop", animTime);
    }

    public void Spiral(float animTime)
    {
        SetTrigger("Spiral", animTime);
    }

    public void ChangeOrbit(float animTime)
    {
        SetTrigger("ChangeOrbit", animTime);
    }
}
