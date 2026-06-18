using UnityEngine;

public class WheelParticleHandler : MonoBehaviour
{
    [SerializeField] private CarController _car;
    private ParticleSystem particleSystemSmoke;

    private ParticleSystem.EmissionModule particleSystemEmissionModule;
    float particleEmissionRate = 0;

    void Awake()
    {
        particleSystemSmoke = GetComponent<ParticleSystem>();

        particleSystemEmissionModule = particleSystemSmoke.emission;

        particleSystemEmissionModule.rateOverTime = 0;
    }

    void Update()
    {
        particleEmissionRate = Mathf.Lerp(particleEmissionRate, 0, Time.deltaTime * 5);
        particleSystemEmissionModule.rateOverTime = particleEmissionRate;
        
        if (_car.IsDrifting)
        {
            particleEmissionRate = 13;
        }
    }
}
