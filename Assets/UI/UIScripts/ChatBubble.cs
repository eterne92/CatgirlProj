
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChatBubble : BasicUIView
{
    public TextColorTweenObj Text;
    public ImageColorTweenObj Image;
    [NonSerialized]
    public Vector3 OgPosition;
    protected Vector2 OgSize;
    public float hoverTime = 0.2f;
    public float unHoverTime = 0.3f;
    protected override void Awake()
    {
        base.Awake();
        Image.image.alphaHitTestMinimumThreshold = 0.05f;
        OgSize = rectTransform.sizeDelta;
        OgPosition = transform.localPosition;
    }

    public void Hover()
    {
        Text.Hover(hoverTime);
        Image.Hover(hoverTime);
        DoLocalScaleTween(1.2f * Vector3.one, hoverTime + 0.1f, type: EaseType.M3Spring);
    }
    public void UnHover()
    {
        Text.UnHover(unHoverTime);
        Image.UnHover(unHoverTime);
        DoLocalScaleTween(Vector3.one, unHoverTime + 0.1f, type: EaseType.M3Spring);
    }


    public void SetText(string text)
    {
        Text.text.text = text;
    }

    public void SetPrice(int price)
    {
        Text.text.text = price.ToString();
    }

    public void Shake()
    {
        Text.SetColorToHover();
        Image.SetColorToHover();
        GetVibrationData().Set(5, 6f);
        SetSize(OgSize);
        DoSizeTween(1.2f * OgSize, 0.6f, type: EaseType.VibrationCustom);
        Text.DoColorTween(Text.normalColor , time:0.6f, type: EaseType.EaseInCubic);
        Image.DoColorTween(Image.normalColor, time: 0.6f, type: EaseType.EaseInCubic);
    }
}

