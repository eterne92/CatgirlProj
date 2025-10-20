using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BasicColorTweenObject : BasicTweenObject
{
    protected RectTransform rectTransform;
    public Color normalColor;
    public Color hoverColor;
    protected Graphic graphic;
    protected int ColorId = 0;

    protected virtual void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        graphic = GetComponent<Graphic>();
        InitColor(normalColor);
    }

    public bool raycastTarget
    {
        get
        {
            return graphic.raycastTarget;
        }
        set
        {
            graphic.raycastTarget = value;
        }
    }

    public Color color
    {
        get
        {
            return graphic.color;
        }
        set
        {
            graphic.color = value;
        }
    }
    public void SetColorToUnHover()
    {
        SetColor(normalColor);
    }
    public void SetColorToHover()
    {
        SetColor(hoverColor);
    }

    public virtual void Hover(float time = 0.2f)
    {
        DoColorTween(hoverColor, time);
    }
    public virtual void UnHover(float time = 0.3f)
    {
        DoColorTween(normalColor, time);
    }
    public void InitColor(Color color)
    {
        graphic.color = color;
        if (color.a == 0)
        {
            graphic.enabled = false;
        }
        else
        {
            graphic.enabled = true;
        }
    }
    public void SetColor(Color color)
    {
        ColorId++;
        graphic.color = color;
        if (color.a == 0)
        {
            graphic.enabled = false;
        }
        else
        {
            graphic.enabled = true;
        }
    }
    public void InitAlpha(float alpha)
    {
        Color c = graphic.color;
        c.a = alpha;
        InitColor(c);
    }
    public void SetAlpha(float targetAlpha)
    {
        Color c = graphic.color;
        c.a = targetAlpha;
        SetColor(c);
    }
    public virtual void DoColorTween(Color TargetColor, float time, Action callBack = null, EaseType type = EaseType.EaseOutQuad, bool IsCallbackRequiredOnComplete = true)
    {
        if (graphic == null)
        {
            Debug.LogError(transform.name + "graphic没有初始化");
            return;
        }
        else
        {
            StartCoroutine(ColorTween(graphic, TargetColor, time, callBack, type, IsCallbackRequiredOnComplete));
        }
    }
    public virtual void DoAlphaTween(float targetAlpha, float time, Action callBack = null, EaseType type = EaseType.EaseOutQuad, bool IsCallbackRequiredOnComplete = true)
    {
        if (graphic == null)
        {
            Debug.LogError("graphic没有初始化");
            return;
        }
        else
        {
            StartCoroutine(ColorTween(graphic, new Color(graphic.color.r, graphic.color.g, graphic.color.b, targetAlpha)
                , time, callBack, type, IsCallbackRequiredOnComplete));
        }
    }


    IEnumerator ColorTween(Graphic graphic,  Color targetColor, float time, Action callback, EaseType type, bool IsCallbackRequiredOnComplete)
    {
        ColorId++;
        int id = ColorId;

        graphic.enabled = true;
        Color startColor = graphic.color;
        Func<float, float> EaseFunc = GetEaseFuncByType(type, time);

        float nowTime = 0;
		nowTime += GetTimeDelta();
		while (nowTime < time && id == ColorId)
        {
			graphic.color = Color.LerpUnclamped(startColor, targetColor , EaseFunc(nowTime / time));
            yield return null;
			nowTime += GetTimeDelta();
		}

        if (id == ColorId)
        {
            if (IsToTarget(type) == true)
            {
                graphic.color = targetColor;
            }
            else
            {
                graphic.color = startColor;
            }
            if(graphic.color.a == 0)
            {
                graphic.enabled = false;
            }
            callback?.Invoke();
        }
        else
        {
            if (IsCallbackRequiredOnComplete == false)
            {
                callback?.Invoke();
            }
            //被中断
        }
    }
}
