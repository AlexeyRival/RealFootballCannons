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
    
    //�������� ������
    [ClientRpc]
    private void TickRPC() 
    {
        text.text = $"������ ����� {seconds}! {playersCount}/4 �������";
    }
    //������ �����
    [ClientRpc]
    private void StartRPC() 
    {
        isStarted = true;
        text.text = "���� ��������!";
        Skorer.instance.StartMatch();
    }
    private void Update()
    {
        if (isServer&&!isStarted)
        {
            if (GameObject.FindGameObjectsWithTag("Player").Length != playersCount)
            {
                playersCount = GameObject.FindGameObjectsWithTag("Player").Length;
                if (false)//playersCount < 2)//������� �������(�������� � ����� ������������)
                {
                    text.text = $"����������! {playersCount}/4 �������";
                }
                else
                {
                    timerStart = true;
                    text.text = $"������ ����� {seconds}! {playersCount}/4 �������";
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
                    text.text = $"������ ����� {seconds}! {playersCount}/4 �������";
                    TickRPC();
                }
            }
        }
    }
}
