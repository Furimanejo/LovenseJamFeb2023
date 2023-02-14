using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;

public class HornyShips : NetworkBehaviour
{
    public static HornyShips instance { get; private set; }
    [Networked] public BoardPlayer currentTurnPlayer { get; set; }
    [Networked] public bool matchOngoing { get; private set; }
    private BoardPlayer[] players;
    [SerializeField] TMP_Text statusText;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        if (matchOngoing)
        {
            if (currentTurnPlayer == BoardPlayer.Local)
                statusText.text = "Your Turn";
            else
                statusText.text = "Opponent's Turn";
            currentTurnPlayer.MatchUpdate();
        }
        else
        {
            players = FindObjectsOfType<BoardPlayer>();
            if(players.Length < 2)
            {
                statusText.text = "Waiting for another player";
            }
            else if (players.Length == 2)
            {
                var bothReady = players[0].readyToPlay && players[1].readyToPlay;
                if(bothReady == false)
                {
                    statusText.text = "Setup phase";
                }
                else
                {
                    // start match
                    matchOngoing = true;
                    var index = Random.Range(0, 2);
                    currentTurnPlayer = players[index];
                }
            }
        }
    }

    public void AdvanceTurns()
    {
        if(currentTurnPlayer.points >= 17)
        {
            if (currentTurnPlayer == BoardPlayer.Local)
                statusText.text = "You Win!";
            else
                statusText.text = "You Lose!";
            Destroy(this);
        }
        else
        {
            print("advance");
            if(currentTurnPlayer == players[0])
                currentTurnPlayer = players[1];
            else
                currentTurnPlayer = players[0];
        }
    }
}