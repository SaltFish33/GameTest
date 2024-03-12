using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GridLine : MonoBehaviour
{
    public List<Grid> grids = new List<Grid>();

    private int index;

    public void Init(UnityAction<Vector2Int> clickAction, Sprite normalSprite, Sprite playerSprite, Sprite pcSprite, int index)
    {
        this.index = index;
        for (int i = 0; i < grids.Count; i++) 
        {
            grids[i].Init(clickAction, normalSprite, playerSprite, pcSprite, new Vector2Int(index, i));
        }
    }

    public void ResetGrid()
    {
        for (int i = 0; i < grids.Count; i++)
        {
            grids[i].ResetGrid();
        }
    }

    public void ChooseGrid(int pos)
    {
        grids[pos].Click(GridStatus.PC);
    }
}
