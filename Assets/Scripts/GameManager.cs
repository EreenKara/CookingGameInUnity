using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{

	public event EventHandler OnStateChanged;
	public event EventHandler OnGamePaused;
	public event EventHandler OnGameContinue;
	public static GameManager Instance{ get; private set; }
	private enum State
	{
		WaitingToStart,
		CountdownToStart,
		GamePlaying,
		GameOver
	}
	private State state;
    private float countdownToStartTimer = 3f;
    private float gamePlayingTimer;
    private float gamePlayingTimerMax= 25f;
	private bool isGamePaused = false;
    private void Awake()
    {
		Instance = this;
        state = State.WaitingToStart;
    }
	
    private void Update()
    {
		switch (state)
		{
			case State.WaitingToStart:
				break;
			case State.CountdownToStart:
                countdownToStartTimer -= Time.deltaTime;
                if (countdownToStartTimer < 0f)
                {
                    state = State.GamePlaying;
					gamePlayingTimer = gamePlayingTimerMax;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
			case State.GamePlaying:
                gamePlayingTimer -= Time.deltaTime;
                if (gamePlayingTimer < 0f)
                {
					state = State.GameOver;
					OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
			case State.GameOver:
				break;
			default:
				break;
		}
	}
    private void Start()
    {
		// Oyunu durdurmak için Escape butonuna abone ediyoruz.
        GameInput.Instace.OnPauseAction += GameInput_OnPauseAction;
        DeliveryManager.Instance.OnRecipeSuccess += DeliveryManager_OnRecipeSuccess;
        DeliveryManager.Instance.OnRecipeFailed += DeliveryManager_OnRecipeFailed;
        GameInput.Instace.OnInteractAction += GameInput_OnInteractAction;
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e)
    {
		if(state == State.WaitingToStart)
		{
			state = State.CountdownToStart;
			OnStateChanged?.Invoke(this, EventArgs.Empty);
		}
    }

    private void DeliveryManager_OnRecipeFailed(object sender, EventArgs e)
    {
		// Yanlis sipariste -5 time 
		float punitiveTime = 5f;
        gamePlayingTimer -= punitiveTime;
		if(gamePlayingTimer < 0f)
		{
			gamePlayingTimer = 0f;
		}
    }

    private void DeliveryManager_OnRecipeSuccess(object sender, EventArgs e)
    {
		// Dogru sipariste +10 time 
		float extraTime = 10f;
        gamePlayingTimer += extraTime;
		if (gamePlayingTimer>gamePlayingTimerMax)
		{
            gamePlayingTimer = gamePlayingTimer - (gamePlayingTimer % gamePlayingTimerMax);
        }
    }

    private void GameInput_OnPauseAction(object sender, EventArgs e)
    {
        ContinueOrPauseGame();
    }

    public bool IsGamePlaying()
	{
		return state == State.GamePlaying;
	}
	public bool IsCountDownToStartActive()
	{
		return state == State.CountdownToStart;
	}
    public bool IsGameOver()
    {
        return state == State.GameOver;
    }
    public float GetCountDownToStartTimer()
	{
		return countdownToStartTimer;
	}
	public float GetGamePlayingTimerNormalized()
	{
		return 1-(gamePlayingTimer/gamePlayingTimerMax);
	}
	public void ContinueOrPauseGame()
	{
		isGamePaused= !isGamePaused;
		if (isGamePaused)
		{
            Time.timeScale = 0f;
			OnGamePaused?.Invoke(this, EventArgs.Empty);
		}
		else
		{
            Time.timeScale = 1f;
            OnGameContinue?.Invoke(this, EventArgs.Empty);
        }

    }

}
