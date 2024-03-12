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
    /// ����״̬��0����δѡ�� 1�������ѡ��2�������ѡ��
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
    /// ���������Ž�
    /// </summary>
    /// <param name="depth"></param>
    /// <param name="alpha">(PCѡ��)ģ���е�����½�ֵ�������;ģ��ı����С���Ϳ��Խ���</param>
    /// <param name="beta">(���ѡ��)ģ���е���С�Ͻ�ֵ</param>
    /// <returns></returns>
    private int MinimaxSearch(int depth, int alpha, int beta)
    {
        //��������ݹ�
        if (depth == 9)
        {
            return 0;
        }

        int bestValue;
        int value;
        //��ǰģ��ΪPC�ٿأ���Ŀ��ӦΪ���֣� ����Ϊ��С��
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
                    //��ʤ������
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
                        //�ص����ϲ㣬����ѡ������
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
        // �����
        for (int i = 0; i < 3; i++)
        {
            if (grids[i, 0] == playerType && grids[i, 1] == playerType && grids[i, 2] == playerType)
            {
                return true;
            }
        }

        // �����
        for (int i = 0; i < 3; i++)
        {
            if (grids[0, i] == playerType && grids[1, i] == playerType && grids[2, i] == playerType)
            {
                return true;
            }
        }

        // ���Խ���
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
