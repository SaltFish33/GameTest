using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GamePanel : BasePanel
{
    public GridManager gridManager;

    public Scoring scoring;

    private UnityAction resetAction;

    private Sprite normalSprite;
    private Sprite playerSprite;
    private Sprite pcSprite;
    private Sprite audioOnSprite;
    private Sprite audioOffSprite;

    private int audioType;
    public async UniTask Init(UnityAction<Vector2Int> gridClickAction, UnityAction resetAction)
    {
        normalSprite = await SpritesMgr.Instance.LoadSprite(SpriteDefine.NormalGrid, AltasDefine.GamePanel);
        playerSprite = await SpritesMgr.Instance.LoadSprite(SpriteDefine.PlayerGrid, AltasDefine.GamePanel);
        pcSprite = await SpritesMgr.Instance.LoadSprite(SpriteDefine.PCGrid, AltasDefine.GamePanel);
        audioOnSprite = await SpritesMgr.Instance.LoadSprite(SpriteDefine.AudioOn, AltasDefine.GamePanel);
        audioOffSprite = await SpritesMgr.Instance.LoadSprite(SpriteDefine.AudioOff, AltasDefine.GamePanel);

        gridManager.Init(gridClickAction, normalSprite, playerSprite, pcSprite);
        scoring.Init(GetControl<Text>("PlayerScore"),GetControl<Text>("PCScore"),GetControl<Text>("DrawScore"));

        this.resetAction = resetAction;
        audioType = 1;
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
        else if(btnName == "AudioBtn")
        {
            var image = GetControl<Image>("AudioBtn");
            if(audioType == 1)
            {
                audioType = 0;
                MusicMgr.Instance.ChangeBKValue(audioType);
                MusicMgr.Instance.ChangedSoundValue(audioType);
                image.sprite = audioOffSprite;
            }
            else
            {
                audioType = 1;
                MusicMgr.Instance.ChangeBKValue(audioType);
                MusicMgr.Instance.ChangedSoundValue(audioType);
                image.sprite = audioOnSprite;
            }
        }
    }

    private void OnDestroy()
    {
        normalSprite = null;
        playerSprite = null;
        pcSprite = null;
    }
}


