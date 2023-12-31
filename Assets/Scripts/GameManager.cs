using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton

    public float timeToMatch = 10f;
    public float currentTimeToMatch = 0;

    public int Points = 0; // Cantidad de puntos
    public UnityEvent OnPointsUpdated; // Evento para cuando se actualizan los puntos
    public UnityEvent<GameState> OnGameStateUpdated; // Evento para cuando se actualiza el estado del juego


    public enum GameState
    {
        Idle,
        InGame,
        GameOver
    }

    public GameState gameState;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if(gameState == GameState.InGame)
        {
            currentTimeToMatch += Time.deltaTime; // El tiempo delta es la cantidad de tiempo real que ha pasado desde el frame anterior
            if(currentTimeToMatch > timeToMatch)
            {
                gameState = GameState.GameOver;
                OnGameStateUpdated?.Invoke(gameState);
            }
        }
    }


    public void AddPoints(int newPoints)
    {
        Points += newPoints;
        OnPointsUpdated?.Invoke();
        currentTimeToMatch = 0f;
    }

    public void RestartGame()
    {
        Points = 0;
        gameState = GameState.InGame;
        OnGameStateUpdated?.Invoke(gameState);
        currentTimeToMatch = 0f;
    }

    public void ExitGame()
    {
        Points = 0;
        gameState = GameState.Idle;
        OnGameStateUpdated?.Invoke(gameState);
    }

    public void StartGame()
    {
        Points = 0;
        gameState = GameState.InGame;
        OnGameStateUpdated?.Invoke(gameState);
        currentTimeToMatch = 0;
    }

}
