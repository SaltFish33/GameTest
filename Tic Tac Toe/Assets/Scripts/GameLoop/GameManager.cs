using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : BaseManager<GameManager>
{
    private int playerScore;
    private int pcScore;
    private int drawScore;

    /// <summary>
    /// 格子状态，0代表未选择， 1代表玩家选择，2代表电脑选择
    /// </summary>
    private int[,] grids = new int[3, 3];
    private int curCount;
    private int curCharacter;
    private bool isFirstPlay;


    public void Init()
    {
        playerScore = 0;
        pcScore = 0;
        drawScore = 0;
        curCount = 0;
        isFirstPlay = true;

        UIMgr.Instance.ShowPanel<GamePanel>("GamePanel", E_UI_Layer.Bot, async (panel) =>
        {
            await panel.Init(OnGridClick, ResetGrid);
            ResetGrid();
            EventCenter.Instance.EventTrigger("GamePanelInitComplete");
        });
        
    }

    public void ResetGrid()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                grids[i, j] = 0;
            }
        }
        curCount = 0;
        if (isFirstPlay)
        {
            isFirstPlay = false;
            return;
        }
        int index = UnityEngine.Random.Range(1, 3);
        if (index == 2)
        {
            ComputerChoose();
            ++curCount;
        }
            
    }

    private void OnGridClick(Vector2Int pos)
    {
        grids[pos.x, pos.y] = 1;
        ++curCount;
        if (CheckWin(1))
        {
            ++playerScore;
            MusicMgr.Instance.PladySoundByAB(AudioClipDefine.GameWin, false);
            EndGame(ScoreType.Player, playerScore);
            return;
        }
        else if(curCount == 9)
        {
            ++drawScore;
            EndGame(ScoreType.Draw, drawScore);
            return;
        }
        ComputerChoose();
        ++curCount;
        
        if (CheckWin(2))
        {
            ++pcScore;
            MusicMgr.Instance.PladySoundByAB(AudioClipDefine.GameFailed, false);
            EndGame(ScoreType.PC, pcScore);
            return;
        }
        else if(curCount == 9)
        {
            ++drawScore;
            EndGame(ScoreType.Draw, drawScore);
            return;
        }
        
    }

    private void ComputerChoose()
    {
        curCharacter = 2;
        MinimaxSearch(curCount, int.MinValue, int.MaxValue);

        grids[bestX, bestY] = 2;
        UIMgr.Instance.GetPanel<GamePanel>("GamePanel").ChooseGrid(new Vector2Int(bestX, bestY));
    }

    int bestX, bestY;

    /// <summary>
    /// 回溯求最优解
    /// </summary>
    /// <param name="depth"></param>
    /// <param name="alpha">(PC选择)模拟中的最大下界值，如果中途模拟的比这个小，就可以结束</param>
    /// <param name="beta">(玩家选择)模拟中的最小上界值</param>
    /// <returns></returns>
    private int MinimaxSearch(int depth, int alpha, int beta)
    {
        //最深处结束递归
        if (depth == 9)
        {
            return 0;
        }

        int bestValue;
        int value;
        //当前模拟为PC操控，则目标应为最大分， 否则为最小分
        if (curCharacter == 2)
        {
            bestValue = int.MinValue;
        }
        else
        {
            bestValue = int.MaxValue;
        }

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (grids[i, j] != 0)
                {
                    continue;
                }

                if (curCharacter == 2)
                {
                    TryPlay(i, j);
                    //获胜，结束
                    if (CheckWin(2))
                    {
                        value = int.MaxValue;
                    }
                    else
                    {
                        value = MinimaxSearch(depth + 1, alpha, beta);
                    }
                    UndoTryPlay(i, j);

                    if (value >= bestValue)
                    {
                        bestValue = value;
                        //回到最上层，设置选择坐标
                        if (depth == curCount)
                        {
                            bestX = i;
                            bestY = j;
                        }
                    }

                    alpha = Mathf.Max(alpha, bestValue);
                    if (beta <= alpha)
                    {
                        break;
                    }
                }
                else
                {
                    TryPlay(i, j);
                    if (CheckWin(1))
                    {
                        value = int.MinValue;
                    }
                    else
                    {
                        value = MinimaxSearch(depth + 1, alpha, beta);
                    }
                    UndoTryPlay(i, j);

                    if (value <= bestValue)
                    {
                        bestValue = value;
                        if (depth == curCount)
                        {
                            bestX = i;
                            bestY = j;
                        }
                    }

                    beta = Mathf.Min(beta, bestValue);
                    if (beta <= alpha)
                    {
                        break;
                    }
                }

                grids[i, j] = 0; 
            }
        }

        return bestValue;
    }

    private void TryPlay(int x, int y)
    {
        grids[x, y] = curCharacter;
        curCharacter = curCharacter == 1 ? 2 : 1;
    }

    private void UndoTryPlay(int x, int y)
    {
        grids[x, y] = 0;
        curCharacter = curCharacter == 1 ? 2 : 1;
    }


    private bool CheckWin(int playerType)
    {
        // 检查行
        for (int i = 0; i < 3; i++)
        {
            if (grids[i, 0] == playerType && grids[i, 1] == playerType && grids[i, 2] == playerType)
            {
                return true;
            }
        }

        // 检查列
        for (int i = 0; i < 3; i++)
        {
            if (grids[0, i] == playerType && grids[1, i] == playerType && grids[2, i] == playerType)
            {
                return true;
            }
        }

        // 检查对角线
        if ((grids[0, 0] == playerType && grids[1, 1] == playerType && grids[2, 2] == playerType) ||
            (grids[0, 2] == playerType && grids[1, 1] == playerType && grids[2, 0] == playerType))
        {
            return true;
        }

        return false;
    }

    public void EndGame(ScoreType winer,int value)
    {
        //Debug.Log($"Winer is {winer.ToString()}");
        EventCenter.Instance.EventTrigger("OnGameEnd");
        UIMgr.Instance.GetPanel<GamePanel>("GamePanel").Score(winer, value);
    }

    public void OnDestroy()
    {
        
    }

}
