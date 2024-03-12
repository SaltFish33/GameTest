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
    private Text playerScore;
    private Text pcScore;
    private Text drawScore;

    public void Init(Text player, Text pc, Text draw)
    {
        this.playerScore = player;
        this.pcScore = pc;
        this.drawScore = draw;
    }

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
