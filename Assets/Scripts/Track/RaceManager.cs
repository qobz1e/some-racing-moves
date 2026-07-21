using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RaceManager : MonoBehaviour
{
    public static RaceManager Instance;

    [SerializeField] private CarController playerCar;
    [SerializeField] private EconomyManager economy;
    [SerializeField] private RaceFlowUI raceFlowUI;
    [SerializeField] private RaceResultsUI raceResults;
    [SerializeField] private int lapsToFinish = 3;

    [SerializeField] private WaypointNode[] waypoints;

    private readonly List<CarController> cars = new();
    private List<CarController> sortedCars = new();

    private bool raceStarted;
    private bool raceFinished;
    private float raceStartTime;

    public float CurrentRaceTime { get; private set; }

    public RaceResult PlayerResult { get; private set; }

    private readonly List<RaceResult> results =
        new List<RaceResult>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        MusicManager.Instance.PlayRace();
        playerCar.OnStartedMoving += StartRace;

        UnlockSystem.Reset();
    }

    void Update()
    {
        if (raceStarted && !raceFinished)
            CurrentRaceTime = Time.time - raceStartTime;
    }

    void FixedUpdate()
    {
        foreach (var car in cars)
        {
            car.UpdateWaypointProgress(waypoints);
        }
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

    public void OnLapCompleted(CarController car)
    {
        if (car.CompareTag("Player"))
        {
            int place = GetCarPlace(car);

            int reward =
                place == 1 ? 100 :
                place == 2 ? 50 :
                place == 3 ? 30 : 10;

            economy.AddCoins(reward);
        }
    }

    public void FinishRace(CarController car)
    {
        if (results.Any(r => r.car == car))
            return;

        RaceResult result =
            new RaceResult
            {
                name = car.gameObject.name,
                car = car,
                finishTime = CurrentRaceTime,
                place = results.Count + 1
            };

        results.Add(result);

        if (car.CompareTag("Player"))
        {
            int stars =
                raceResults.CalculateStars(
                    CurrentRaceTime
                );

            PlayerResult = result;

            raceResults.CheckUnlocks(stars);

            raceFlowUI.ShowLeaderboard();
        }

        if (raceFlowUI.gameObject.activeSelf)
        {
            raceFlowUI.RefreshLeaderboard();
        }
    }

    public void RegisterCar(CarController car)
    {
        if (!cars.Contains(car))
            cars.Add(car);
    }

    public void UnregisterCar(CarController car)
    {
        cars.Remove(car);
    }

    public int GetCarPlace(CarController car)
    {
        sortedCars.Clear();
        sortedCars.AddRange(cars);

        sortedCars.Sort((a, b) =>
            b.Progress.CompareTo(a.Progress));

        return sortedCars.IndexOf(car) + 1;
    }

    public int GetPlayerPlace()
    {
        return GetCarPlace(playerCar);
    }

    public int LapsToFinish => lapsToFinish;

    public IReadOnlyList<RaceResult> Results => results;

    public int CarCount => cars.Count;

    public WaypointNode[] Waypoints => waypoints;
}

public class RaceResult
{
    public string name;
    public CarController car;
    public float finishTime;
    public int place;
}