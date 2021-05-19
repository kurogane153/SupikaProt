using System.Collections;
using UnityEngine;

public class LastBossDisolveEffectScript : MonoBehaviour
{
    public　Material m_Material;
    YieldInstruction m_Instruction = new WaitForEndOfFrame();
    MeshRenderer meshRenderer;
    public GameObject _particle;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnDestroy()
    {
        m_Material.SetFloat("_CutOff", 0);
    }

    public void StartDisovle()
    {
        StartCoroutine("Animate");
    }

    IEnumerator Animate()
    {
        meshRenderer.material = m_Material;
        _particle.SetActive(true);
        float time = 0;
        float duration = 5.5f;
        int dir = 1;

        while (true) {
            yield return m_Instruction;

            time += Time.deltaTime * dir;
            var t = time / duration;

            if (t > 1f) {
                dir = -1;
            } else if (t < 0) {
                dir = 1;
            }


            m_Material.SetFloat("_CutOff", t);
        }
    }
}
