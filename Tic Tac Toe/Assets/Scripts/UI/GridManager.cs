using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.Events;

public class GridManager : MonoBehaviour
{
    public List<GridLine> gridLines = new List<GridLine>();

    public void Init(UnityAction<Vector2Int> clickAction, Sprite normalSprite, Sprite playerSprite, Sprite pcSprite)
    {
        for (int i = 0; i < gridLines.Count; i++) 
        {
            gridLines[i].Init(clickAction, normalSprite, playerSprite, pcSprite);
        }
    }

    public void ResetGrid()
    {
        for (int i = 0; i < gridLines.Count; i++)
        {
            gridLines[i].ResetGrid();
        }
    }

    public void ChooseGrid(Vector2Int pos)
    {
        gridLines[pos.x].ChooseGrid(pos.y);
    }

}
