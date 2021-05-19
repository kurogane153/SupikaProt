using UnityEngine;

public class BoostEffect : MonoBehaviour
{
    [SerializeField] ParticleSystem[] _boostParticles;

    void Start()
    {
        StopBoostParticles();
    }

    public void PlayBoostParticles()
    {
        foreach (var bp in _boostParticles) {
            bp.gameObject.SetActive(true);
            bp.Play();
        }
    }

    public void StopBoostParticles()
    {
        foreach (var bp in _boostParticles) {
            bp.Stop();
        }
    }
    
}
