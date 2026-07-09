using UnityEngine;

public class CarSfxHandler : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource engineSound;
    [SerializeField] private AudioSource tiresScreechSound;
    [SerializeField] private AudioSource collisionSound;

    [SerializeField] private CarController car;

    [Header("Engine")]
    [SerializeField] private float minPitch = 0.6f;
    [SerializeField] private float maxPitch = 2.0f;

    [SerializeField] private float minVolume = 0.15f;
    [SerializeField] private float maxVolume = 1f;

    [Header("Drift")]
    [SerializeField] private float driftVolumeLerp = 8f;

    bool isPaused;

    private void Awake()
    {
        if (!car)
            car = GetComponent<CarController>();
    }

    void Update()
    {
        if (Time.timeScale == 0f)
        {
            if (!isPaused)
            {
                engineSound.Pause();
                tiresScreechSound.Pause();
                isPaused = true;
            }

            return;
        }

        if (isPaused)
        {
            engineSound.UnPause();
            tiresScreechSound.UnPause();
            isPaused = false;
        }

        UpdateEngine();
        UpdateDrift();
    }

    void UpdateEngine()
    {
        float speed01 =
            Mathf.Clamp01(car.CurrentSpeed / car.MaxSpeed);

        float targetPitch =
            Mathf.Lerp(minPitch, maxPitch, speed01);

        float targetVolume =
            Mathf.Lerp(minVolume, maxVolume, speed01);

        if (car.IsUsingNitro)
        {
            targetPitch += 0.2f;
            targetVolume += 0.1f;
        }

        engineSound.pitch =
            Mathf.Lerp(
                engineSound.pitch,
                targetPitch,
                Time.deltaTime * 6f);

        engineSound.volume =
            Mathf.Lerp(
                engineSound.volume,
                Mathf.Clamp01(targetVolume),
                Time.deltaTime * 8f);
    }

    void UpdateDrift()
    {
        float targetVolume = 0f;

        if (car.IsDrifting)
        {
            targetVolume =
                Mathf.InverseLerp(
                    15f,
                    60f,
                    Mathf.Abs(car.DriftAngle));

            targetVolume = Mathf.Clamp01(targetVolume);
        }

        tiresScreechSound.volume =
            Mathf.Lerp(
                tiresScreechSound.volume,
                targetVolume,
                Time.deltaTime * driftVolumeLerp);

        tiresScreechSound.pitch =
            Mathf.Lerp(
                tiresScreechSound.pitch,
                0.8f + targetVolume * 0.4f,
                Time.deltaTime * driftVolumeLerp);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        float impact = collision.relativeVelocity.magnitude;

        if (impact < 1f)
            return;

        collisionSound.pitch =
            Random.Range(0.95f, 1.05f);

        collisionSound.volume =
            Mathf.Clamp01(impact / 15f);

        collisionSound.Play();
    }
}