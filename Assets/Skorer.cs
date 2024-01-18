using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Text;

public class Skorer : NetworkBehaviour
{
    private Text text;

    private int[] scores = { 0, 0, 0, 0};
    private List<CannonPlayer> players;

    private bool isGameStart;

    //синглтон
    public static Skorer instance;

    void Start()
    {
        text = GetComponent<Text>();
        instance = this;
    }
    
    //начало матча
    public void StartMatch() 
    {
        players = new List<CannonPlayer>();
        GameObject[] plrs = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < plrs.Length; ++i) 
        {
            players.Add(plrs[i].GetComponent<CannonPlayer>());
        }

        scores = new int[players.Count];
        UpdateTextRPC(scores);
    }

    //Гол
    public void MakeGoal(int sender, int reciever) 
    {
        if (reciever < players.Count)
        {
            --scores[reciever];
            ++scores[sender];
            UpdateTextRPC(scores);
        }
    }
    
    //чтобы счёт обновлялся у всех
    [ClientRpc]
    private void UpdateTextRPC(int[] scores) 
    {
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < players.Count; ++i) 
        {
            builder.Append($"<color=#{ColorUtility.ToHtmlStringRGB(players[i].color)}> Игрок ");
            builder.Append(i);
            builder.Append(' ');
            builder.Append(scores[i]);
            builder.Append("</color>\n");
        }
        text.text = builder.ToString();
    }
}
