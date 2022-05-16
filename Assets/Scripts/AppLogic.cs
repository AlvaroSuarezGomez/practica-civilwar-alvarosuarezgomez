using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppLogic : MonoBehaviour
{
    public static AppLogic Instance { get; private set; }

    private string player1Name = "Player1";

    private string player2Name = "Player2";

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SetPlayer1Name(string name)
    {
        player1Name = name;
    }

    public void SetPlayer2Name(string name)
    {
        player2Name = name;
    }
}
