using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Victory : MonoBehaviour
{
    // Start is called before the first frame update
    [System.Serializable]
    public struct Skins
    {
        public List<GameObject> skins;
    }
    public List<Skins> playerDemo;
    public GameObject[] puesto;
    private PersistenceData inGameData;
    public TMP_Text nameText;
    public TMP_Text hitsText;
    public TMP_Text jumpText;
    public TMP_Text pushText;
    public TMP_Text killsText;
    public TMP_Text averageRhythmText;
    public TMP_Text ticketsText;
    public EfectosSonido efectosSonido;

    public UnityEvent OnStart;

    private void Awake()
    {
        MenuMusicPlayer.Instance.PlayMusic(1);
    }

    void Start()
    {
        efectosSonido = GetComponent<EfectosSonido>();
        inGameData = FindObjectOfType<PersistenceData>();
        puesto[inGameData.puesto].SetActive(true);
        playerDemo[inGameData.playerWinner].skins[inGameData.playerWinnerSkin].SetActive(true);
        nameText.SetText(inGameData.playerWinnerName);
        hitsText.SetText("Golpes dados: " + inGameData.hitsStats);
        jumpText.SetText("Saltos: " + inGameData.jumpStats);
        pushText.SetText("Golpes recibidos: " + inGameData.pushStats);
        killsText.SetText("Bajas: " + inGameData.killsStats);
        averageRhythmText.SetText("Ritmo ratio: " + inGameData.averageRhythmStats + "%");
        ticketsText.SetText("" + inGameData.GetTickets());
        SonarEfecto(inGameData.selectedPlayer);
        OnStart.Invoke();
    }

    public IEnumerator SonarEfecto(int t)
    {
        yield return new WaitForSeconds(2);
        switch (t)
        {
            case 0:
                efectosSonido.PlayEffect(0);    //Punch
                break;
            case 1:
                efectosSonido.PlayEffect(1);    //XX
                break;
            case 2:
                efectosSonido.PlayEffect(2);    //FANTASMA
                break;
        }
    }
    public void GetTicketsFromGame()
    {
        FindObjectOfType<GemasManager>().AddTickets(inGameData.GetTickets());
    }

    public void OnContinue()
    {
        FindObjectOfType<SceneTransitioner>().StartTransition(1, 1f, "Regresando al Lobby");
    }
}
