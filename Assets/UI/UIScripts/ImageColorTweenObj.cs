using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ImageColorTweenObj : BasicColorTweenObject
{
    public Image image = null;
    protected override void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (image == null)
        {
            image = GetComponent<Image>();
        }
        graphic = image;
        InitColor(normalColor);
    }

}
