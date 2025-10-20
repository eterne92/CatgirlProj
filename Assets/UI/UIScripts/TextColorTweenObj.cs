using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextColorTweenObj : BasicColorTweenObject
{
    public TextMeshProUGUI text;

    protected override void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if(text == null )
        {
            text = GetComponent<TextMeshProUGUI>();
        }
        graphic = text;
        InitColor(normalColor);
    }

    public void SetText(string str)
    {
        if (gameObject.activeInHierarchy == true && str != text.text)
        {
            SetAlpha(text.color.a / 3);
            UnHover();
        }
        text.text = str;
    }
}
