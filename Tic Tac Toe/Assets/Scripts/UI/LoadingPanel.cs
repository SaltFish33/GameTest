using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LoadingPanel : BasePanel
{
    public override void ShowMe()
    {
        base.ShowMe();
        GetControl<Slider>("LoadingBar").value = 0;
        EventCenter.Instance.AddEventListener("GamePanelInitComplete", EndSlider);
    }

    private void EndSlider()
    {
        SetSlider().Forget();
    }

    private async UniTaskVoid SetSlider()
    {
        GetControl<Slider>("LoadingBar").value = 1;
        SetDescrition("更新完成");
        await UniTask.Delay(2000);
        UIMgr.Instance.HidePanel("LoadingPanel");
    }

    public void SetDescrition(string value)
    {
        GetControl<Text>("DescritionText").text = value;
    }


    public override void HideMe()
    {
        base.HideMe();
        EventCenter.Instance.RemoveEventListener("GamePanelInitComplete", EndSlider);
    }
}
