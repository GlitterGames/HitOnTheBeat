using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GemasManager : MonoBehaviour
{
    public TMP_Text ticketsText;

    private void Start()
    {
        UpdateTickets();
    }

    public void AddTickets(int tickets)
    {
        int ticketActual = PlayerPrefs.GetInt("gemas", 0);
        int duplicado = PlayerPrefs.GetInt("duplicado", 0);
        if (duplicado == 1) tickets *= 2;
        ticketActual += tickets;
        if (ticketActual > 99999999) ticketActual = 99999999;
        else if (ticketActual < 0) ticketActual = 0;
        ticketsText.SetText(ticketActual.ToString());
        PlayerPrefs.SetInt("gemas", ticketActual);
    }

    public void SetTickets(int tickets)
    {
        if (tickets > 99999999) tickets = 99999999;
        else if (tickets < 0) tickets = 0;
        ticketsText.SetText(tickets.ToString());
        PlayerPrefs.SetInt("gemas", tickets);
    }

    public void UpdateTickets()
    {
        int ticket = PlayerPrefs.GetInt("gemas", 0);
        ticketsText.SetText(ticket.ToString());
    }
    public void SetX2(int boolean)
    {
        PlayerPrefs.SetInt("duplicado", boolean);
    }
}
