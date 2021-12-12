using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistenceData : MonoBehaviour
{
    public int puesto;
    public int selectedPlayer;
    public int selectedSkin;
    public int playerWinner;
    public int playerWinnerSkin;
    public string playerWinnerName;

    public int hitsStats;
    public int jumpStats;
    public int pushStats;
    public int killsStats;
    public int averageRhythmStats;

    public int GetTickets()
    {
        int tickets = hitsStats + jumpStats / 2 - pushStats * 2 + killsStats * 15;
        tickets = (int) (tickets * (float) averageRhythmStats / 100);
        switch (puesto)
        {
            case 0:
                tickets += 50;
                break;
            case 1:
                tickets += 35;
                break;
            case 2:
                tickets += 10;
                break;
            default:
                break;
        }
        return tickets;
    }
}
