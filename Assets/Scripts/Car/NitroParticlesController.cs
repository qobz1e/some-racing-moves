using UnityEngine;

public class NitroParticlesController : MonoBehaviour
{
    [SerializeField] private CarController car;
    [SerializeField] private ParticleSystem ps;

    private ParticleSystem.EmissionModule emission;

    [Header("Settings")]
    [SerializeField] private float normalRate = 0f;
    [SerializeField] private float nitroRate = 40f;

    private void Awake()
    {
        emission = ps.emission;
    }

    private void Update()
    {
        bool active =
            car != null &&
            car.IsUsingNitro;

        emission.rateOverTime =
            active ? nitroRate : normalRate;

        if (active && !ps.isPlaying)
            ps.Play();
        else if (!active && ps.isPlaying)
            ps.Stop();
    }
}