using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ScoreType 
{ 
    Player,
    PC,
    Draw
}


public class Scoring : MonoBehaviour
{
    public Text playerScore;
    public Text pcScore;
    public Text drawScore;

    public void Score(ScoreType type, int value)
    {
        switch (type)
        {
            case ScoreType.Player:
                playerScore.text = value.ToString();
                break;
            case ScoreType.PC:
                pcScore.text = value.ToString();
                break;
            case ScoreType.Draw:
                drawScore.text = value.ToString();
                break;
        }
    }

}
