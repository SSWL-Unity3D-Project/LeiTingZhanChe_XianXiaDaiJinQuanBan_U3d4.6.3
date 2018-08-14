using UnityEngine;

public class TestLiZi : MonoBehaviour
{
    public ParticleSystem m_Particle;
    public Material m_Mat;
	// Use this for initialization
	void Start()
    {
        m_Particle.renderer.material = m_Mat;
    }
}