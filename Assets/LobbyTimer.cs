using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
public class LobbyTimer : NetworkBehaviour
{
    private Text text;

    [SyncVar]
    private int playersCount;

    private float timer=10;
    [SyncVar]
    private int seconds =10;

    private bool timerStart;
    public static bool isStarted;

    void Start()
    {
        text = GetComponent<Text>();
    }
    
    //обратный отсчёт
    [ClientRpc]
    private void TickRPC() 
    {
        text.text = $"Запуск через {seconds}! {playersCount}/4 игроков";
    }
    //начало матча
    [ClientRpc]
    private void StartRPC() 
    {
        isStarted = true;
        text.text = "Игра началась!";
        Skorer.instance.StartMatch();
    }
    private void Update()
    {
        if (isServer&&!isStarted)
        {
            if (GameObject.FindGameObjectsWithTag("Player").Length != playersCount)
            {
                playersCount = GameObject.FindGameObjectsWithTag("Player").Length;
                if (false)//playersCount < 2)//минимум игроков(отключён в целях тестирования)
                {
                    text.text = $"Подготовка! {playersCount}/4 игроков";
                }
                else
                {
                    timerStart = true;
                    text.text = $"Запуск через {seconds}! {playersCount}/4 игроков";
                }
            }
            if (timerStart)
            {
                timer -= Time.deltaTime;
                if (timer < seconds)
                {
                    seconds = (int)timer;
                    if (seconds == -1) 
                    {
                        StartRPC();
                        return;
                    }
                    text.text = $"Запуск через {seconds}! {playersCount}/4 игроков";
                    TickRPC();
                }
            }
        }
    }
}
