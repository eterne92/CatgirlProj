using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasAlphaController : BasicUIView
{
    public EaseType ShowEaseType;
    public EaseType HideEaseType;
    public void Show(float time = 0.3f)
    {
        DoAlphaTween(1f, time, null, ShowEaseType);
        SetCanvasGroupBlockable();
    }

    public void Hide(float time = 0.3f)
    {
        DoAlphaTween(0f, time, SetCanvasGroupUnblock, ShowEaseType);
    }
}
