using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RaceManager : MonoBehaviour
{
    public static RaceManager Instance;

    [SerializeField] private CarController playerCar;
    [SerializeField] private int lapsToFinish = 3;

    private bool raceStarted;
    private bool raceFinished;
    private float raceStartTime;

    public float CurrentRaceTime { get; private set; }

    private readonly List<RaceResult> results =
        new List<RaceResult>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        playerCar.OnStartedMoving += StartRace;
    }

    void Update()
    {
        if (raceStarted && !raceFinished)
            CurrentRaceTime = Time.time - raceStartTime;
    }

    private void OnDestroy()
    {
        if (playerCar != null)
            playerCar.OnStartedMoving -= StartRace;
    }

    private void StartRace()
    {
        if (raceStarted)
            return;

        raceStarted = true;
        raceStartTime = Time.time;
    }

    public void FinishRace(CarController car)
    {
        if (results.Any(r => r.car == car))
            return;

        RaceResult result =
            new RaceResult
            {
                car = car,
                finishTime = CurrentRaceTime,
                place = results.Count + 1
            };

        results.Add(result);

        if (car.CompareTag("Player"))
        {
            raceFinished = true;

            car.GetComponent<RaceResultsUI>()
                .ShowResults(result.place);

            Time.timeScale = 0f;
        }
    }

    public int LapsToFinish => lapsToFinish;

    public IReadOnlyList<RaceResult> Results => results;
}

public class RaceResult
{
    public CarController car;
    public float finishTime;
    public int place;
}