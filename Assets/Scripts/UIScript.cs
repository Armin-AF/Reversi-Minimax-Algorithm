using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    public static UIScript Instance { get; private set; }


    public void Awake()
    {
        
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void Timer() //Adding timer
    {
        
        int time = (int) Time.deltaTime;

        var seconds = time % 60;
        var textSeconds = gameObject.transform.Find("Seconds").GetComponent<Text>().text;

        var minutes = time / 60;
        var textMinutes = gameObject.transform.Find("Minutes").GetComponent<Text>().text;

    }
    public void DisplayTurn(TileState turn)
    {
        switch (turn)
        {
            case TileState.Black:
                gameObject.transform.Find("BlacksTurn").GetComponent<Image>().enabled = true;
                gameObject.transform.Find("WhitesTurn").GetComponent<Image>().enabled = false;
                break;
            case TileState.White:
                gameObject.transform.Find("BlacksTurn").GetComponent<Image>().enabled = false;
                gameObject.transform.Find("WhitesTurn").GetComponent<Image>().enabled = true;
                break;
            case TileState.Empty:
            default:
                break;
        }
    }

    public void DisplayWin(TileState winnerUi)
    {
        switch (winnerUi)
        {
            case TileState.Black:
                gameObject.transform.Find("BlackWins").GetComponent<Image>().enabled = true;
                break;
            case TileState.White:
                gameObject.transform.Find("WhiteWins").GetComponent<Image>().enabled = true;
                break;
            case TileState.Empty:
                gameObject.transform.Find("Draw").GetComponent<Image>().enabled = true;
                break;
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
