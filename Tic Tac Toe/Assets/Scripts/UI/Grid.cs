using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum GridStatus
{
    Null,
    Player,
    PC
}

public class Grid : MonoBehaviour, IPointerClickHandler
{
    public Vector2Int pos;

    private UnityAction<Vector2Int> clickAction;
    private GridStatus status;

    private Image image;
    private Sprite normalSprite;
    private Sprite playerSprite;
    private Sprite pcSprite;

    private bool isEnable;
    public void Init(UnityAction<Vector2Int> clickAction, Sprite normalSprite, Sprite playerSprite, Sprite pcSprite)
    {
        this.clickAction = clickAction;
        image = GetComponent<Image>();
        status = GridStatus.Null;
        isEnable = true;
        this.normalSprite = normalSprite;
        this.playerSprite = playerSprite;
        this.pcSprite = pcSprite;
        EventCenter.Instance.AddEventListener("OnGameEnd", OnGameEnd);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isEnable) return;
        Click(GridStatus.Player);
        clickAction?.Invoke(pos);
    }

    public void Click(GridStatus status)
    {
        this.status = status;
        switch (status)
        {
            case GridStatus.Player:
                image.sprite = playerSprite;
                break;
            case GridStatus.PC:
                image.sprite = pcSprite;
                break;
        }
        
        isEnable = false;
    }

    public void ResetGrid()
    {
        status = GridStatus.Null;
        isEnable = true;
        image.sprite = normalSprite;
    }

    private void OnGameEnd()
    {
        isEnable = false;
    }

    private void OnDestroy()
    {
        EventCenter.Instance.RemoveEventListener("OnGameEnd", OnGameEnd);
    }
}
