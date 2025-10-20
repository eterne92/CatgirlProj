using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class BasicUIView : BasicTweenObject
{
    //canvasGroup不一定挂载在这个gameobj上。
    [SerializeField]
    private CanvasGroup canvasGroup;
    /// <summary>
    /// 只用来处理UI组件，一般使用transform(以后出现问题再改),可能在更改屏幕分辨率，Camera大小，canvas模式后出错
    /// </summary>
    protected RectTransform rectTransform;
    private int AlphaId = 0;
    private int SizeTweenId = 0;
    //有时候挂载在自身上，有时在父对象上，在LimitPosition里会用到
    protected Canvas canvas;

    protected virtual void Awake()
    {
        rectTransform =  (RectTransform)transform;
        if(canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
        canvas = GetComponentInParent<Canvas>();
    }
    public RectTransform GetRectTransform()
    {
        return rectTransform;
    }
    public Canvas GetCanvas()
    {
        return canvas;
    }
    public CanvasGroup GetCanvasGroup()
    {
        return canvasGroup;
    }
    public override void StopEveryTween()
    {
        base.StopEveryTween();
        AlphaId++;
        SizeTweenId++;
    }
    public void StopAlphaTween()
    {
        AlphaId++;
    }
    public void SetCanvasGroupAlpha(float alpha)
    {
        AlphaId++;
        canvasGroup.alpha = alpha;
    }
    public void SetSize(Vector2 Size)
    {
        SizeTweenId++;
        SetRealSize(Size);
    }
    public void DoAlphaTween(float targetAlpha, float time, Action callBack = null, EaseType type = EaseType.EaseOutQuad, bool IsCallbackRequiredOnComplete = true)
    {
        if (canvasGroup == null)
        {
            Debug.LogError("canvasGroup没有初始化");
            return;
        }
        else
        {
            StartCoroutine(AlphaTween(targetAlpha, time, callBack, type, IsCallbackRequiredOnComplete));
        }
    }


    IEnumerator AlphaTween(float targetAlpha, float time, Action callback, EaseType type, bool IsCallbackRequiredOnComplete)
    {
        AlphaId++;
        int id = AlphaId;

        float startAlpha = canvasGroup.alpha;
        Func<float, float> EaseFunc = GetEaseFuncByType(type, time);

        float nowTime = 0;
		nowTime += GetTimeDelta();

        while (nowTime < time && id == AlphaId)
        {
            canvasGroup.alpha = startAlpha + (targetAlpha - startAlpha) * EaseFunc(nowTime / time);
            yield return null;
			nowTime += GetTimeDelta();
		}

        if (id == AlphaId)
        {
            if (IsToTarget(type) == true)
            {
                canvasGroup.alpha = targetAlpha;
            }
            else
            {
                canvasGroup.alpha = startAlpha;
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
    public void DoSizeTween(Vector2 TargetSize, float time, Action callBack = null, EaseType type = EaseType.EaseOutQuad, bool IsCallbackRequiredOnComplete = true)
    {
            StartCoroutine(SizeTween(TargetSize, time,  type, callBack, IsCallbackRequiredOnComplete));
    }
    IEnumerator SizeTween(Vector2 TargetSize, float time, EaseType type, Action callback, bool IsCallbackRequiredOnComplete)
    {
        SizeTweenId++;
        int id = SizeTweenId;

        Vector2 startSize = rectTransform.rect.size;

        float nowTime = 0;
		nowTime += GetTimeDelta();

		if (IsRandomShake(type) == false)
        {
            Func<float, float> EaseFunc = GetEaseFuncByType(type, time);
            while (nowTime < time && id == SizeTweenId)
            {
                SetRealSize( Vector2.LerpUnclamped(startSize, TargetSize, EaseFunc(nowTime / time)));
                yield return null;
				nowTime += GetTimeDelta();
			}
        }
        else
        {
            Func<float, Vector3> V3ShakeFunc = GetVector3ShakeFuncByType(type);
            Vector2 Delta = TargetSize - startSize;
            while (nowTime < time && id == SizeTweenId)
            {
                SetRealSize( startSize + Vector2.Scale(V3ShakeFunc(nowTime / time), Delta));
                yield return null;
				nowTime += GetTimeDelta();
			}
        }

        if (id == SizeTweenId)
        {
            if (IsToTarget(type) == true)
            {
                SetRealSize(TargetSize );
            }
            else
            {
                SetRealSize( startSize );
            }
            callback?.Invoke();
        }
        else
        {
            if (IsCallbackRequiredOnComplete == false)
            {
                callback?.Invoke();
            }
        }
    }

    private void SetRealSize(Vector2 size)
    {
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
    }
    public void SetCanvasGroupUnblock()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
    public void SetCanvasGroupBlockable()
    {
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
    public float GetHeight()
    {
        return rectTransform.rect.height;
    }
    public Vector3 LimitRectVisible()
    {
        return LimitRectVisible(new RectOffset(0, 0, 0, 0));
    }
    public Vector3 LimitRectVisible(RectOffset margin)
    {
        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            Vector3 delta = GetLimitDelta(margin, Vector3.zero, new Vector3(Screen.width, Screen.height, 0) ,IsOverlay: true);
            rectTransform.anchoredPosition += new Vector2(delta.x, delta.y);
            return delta;
        }
        else
        {
            Vector2 delta = GetLimitDelta(margin, Vector3.zero, new Vector3(Screen.width, Screen.height, 0));
            Vector3 newPos = Camera.main.WorldToScreenPoint(transform.position);
            newPos += new Vector3(delta.x,delta.y);
            SetWorldPosition(Camera.main.ScreenToWorldPoint(newPos));
            return delta;
        }
    }
    public Vector2 GetLimitDelta(RectOffset margin , Vector3 min,Vector3 max ,bool IsOverlay = false)
    {
        Vector3 minCard;
        Vector3 maxCard;
        GetPointMinMax(out minCard, out maxCard,IsOverlay);
        minCard.x -= margin.left;
        minCard.y -= margin.bottom;
        maxCard.x += margin.right;
        maxCard.y += margin.top;

        Vector2 delta = Vector3.zero;
        if (minCard.x < min.x) //卡的左下角不能小于摄像机左下角
        {
            delta.x  = min.x - minCard.x;
        }
        if (minCard.y < (min.y)) //卡的左下角不能小于摄像机左下角
        {
            delta.y = min.y - minCard.y;
        }
        if (maxCard.x > max.x) //卡的右上角不能大于摄像机右上角
        {
            delta.x = max.x - maxCard.x;
        }
        if (maxCard.y > max.y) //卡的右上角不能大于摄像机右上角
        {
            delta.y = max.y - maxCard.y;
        }
        return delta;
    }
    public virtual void GetPointMinMax(out Vector3 minCard, out Vector3 maxCard, bool IsOverlay = false)
    {
        GetPointMinMax(rectTransform, out minCard, out maxCard, IsOverlay);
    }
    public void GetPointMinMax(RectTransform JudgedRectTransform, out Vector3 minCard, out Vector3 maxCard , bool IsOverlay = false)
    {
        Vector3[] worldCorners = new Vector3[4];
        JudgedRectTransform.GetWorldCorners(worldCorners);
        if (IsOverlay == false)
        {
            for (int i = 0; i < worldCorners.Length; i++)
            {
                worldCorners[i] = Camera.main.WorldToScreenPoint(worldCorners[i]);
            }
        }

        float xMin = float.MaxValue;
        for (int i = 0; i < worldCorners.Length; i++)
        {
            if (xMin > worldCorners[i].x)
            {
                xMin = worldCorners[i].x;
            }
        }
        float yMin = float.MaxValue;
        for (int i = 0; i < worldCorners.Length; i++)
        {
            if (yMin > worldCorners[i].y)
            {
                yMin = worldCorners[i].y;
            }
        }
        float ZMin = float.MaxValue;
        for (int i = 0; i < worldCorners.Length; i++)
        {
            if (ZMin > worldCorners[i].z)
            {
                ZMin = worldCorners[i].z;
            }
        }
        minCard = new Vector3(xMin, yMin,ZMin);


        float xMax = float.MinValue;
        for (int i = 0; i < worldCorners.Length; i++)
        {
            if (xMax < worldCorners[i].x)
            {
                xMax = worldCorners[i].x;
            }
        }
        float yMax = float.MinValue;
        for (int i = 0; i < worldCorners.Length; i++)
        {
            if (yMax < worldCorners[i].y)
            {
                yMax = worldCorners[i].y;
            }
        }
        float zMax = float.MinValue;
        for (int i = 0; i < worldCorners.Length; i++)
        {
            if (zMax < worldCorners[i].z)
            {
                zMax = worldCorners[i].z;
            }
        }
        maxCard = new Vector3(xMax, yMax,zMax);
    }

}
