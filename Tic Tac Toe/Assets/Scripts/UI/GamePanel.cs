using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GamePanel : BasePanel
{
    public GridManager gridManager;

    public Scoring scoring;

    private UnityAction resetAction;
    public void Init(UnityAction<Vector2Int> gridClickAction, UnityAction resetAction, Sprite normalSprite, Sprite playerSprite, Sprite pcSprite)
    {
        gridManager.Init(gridClickAction, normalSprite, playerSprite, pcSprite);
        this.resetAction = resetAction;
    }

    public void ResetGrid()
    {
        gridManager.ResetGrid();
    }

    public void ResetScore()
    {

    }

    public void ChooseGrid(Vector2Int pos)
    {
        gridManager.ChooseGrid(pos);
    }

    public void Score(ScoreType type, int value)
    {
        scoring.Score(type, value);
    }

    protected override void OnClick(string btnName)
    {
        base.OnClick(btnName);
        if(btnName == "ResetBtn")
        {
            ResetGrid();
            resetAction?.Invoke();
        }
        else if(btnName == "QuitBtn")
        {
            Application.Quit();
        }
    }
}


