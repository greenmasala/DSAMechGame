using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static event Action OnGameOver;

    [SerializeField] private MapGenerator mapGenerator;
    [SerializeField] private int objectiveToDestroy = 10;

    private MapData map => mapGenerator.Map;
    private int objectivesLeft;
    private bool isGameOver;

    public void ObjectiveDestroyed()
    {
        if (isGameOver) return;

        objectivesLeft--;
        if (objectivesLeft <= 0)
        {
            Debug.Log($"{objectiveToDestroy} objectives destroyed! Game Over!");
            GameOver();
        }
    }

    public void OnPlayerDie()
    {
        if (isGameOver) return;

        Debug.Log("Player died! Game Over!");
        GameOver();
    }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        GameStart();
    }

    private void GameStart()
    {
        isGameOver = false;
        objectivesLeft = objectiveToDestroy;
        mapGenerator.GenerateMap();
    }

    private void GameOver()
    {
        isGameOver = true;
        OnGameOver?.Invoke();
        GameStart();
    }
}
