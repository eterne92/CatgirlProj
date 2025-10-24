
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PressableButton : BasicUIView, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public TextColorTweenObj buttonText;
    public ImageColorTweenObj buttonImage;
    [NonSerialized]
    public UnityEvent PointerEnterd;
    public UnityEvent PointerDowned;
    [NonSerialized]
    public UnityEvent PointerExited;
    protected Vector2 OgSize;
    protected State state = State.idle;
    bool used = false;
    public enum State
    {
        idle,
        hover,
        disable,
    }
    public float hoverTime = 0.2f;
    public float unHoverTime = 0.3f;
    protected override void Awake()
    {
        base.Awake();
        OgSize = rectTransform.sizeDelta;
        SetTimeScaled(false);
        buttonText.SetTimeScaled(false);
        buttonImage.SetTimeScaled(false);
    }

    private void Start()
    {
        ChangeStateToIdle();
    }

    private void OnDisable()
    {
        ChangeStateToIdle();
        used = true;
    }

    private void OnEnable()
    {
        if (used)
        {
            Debug.Log("Used");
            ChangeStateToIdle();
        }
        
    }

    protected void OnPointerEntered()
    {
        PointerEnterd?.Invoke();
    }

    protected void OnPointerExited()
    {
        PointerExited?.Invoke();
    }
    protected void OnPointerDowned()
    {
        PointerDowned?.Invoke();
    }

    public void SetText(string str)
    {
        buttonText.text.text = str;
    }
    public virtual void ChangeStateToIdle()
    {
        state = State.idle;
        buttonText.UnHover(unHoverTime);
        buttonImage.UnHover(unHoverTime);
        DoSizeTween(OgSize, unHoverTime +0.1f, type: EaseType.M3Spring);
    }
    public virtual void ChangeStateToHover()
    {
        state = State.hover;
        buttonText.Hover(hoverTime);
        buttonImage.Hover(hoverTime);
        DoSizeTween(OgSize + new Vector2(0.2f * OgSize.x, 0), hoverTime +0.1f, type: EaseType.M3Spring);
    }
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (state != State.disable)
        {
            OnPointerDowned();
        }
    }
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (state != State.disable)
        {
            ChangeStateToHover();
            OnPointerEntered();
        }
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (state != State.disable)
        {
            ChangeStateToIdle();
            OnPointerExited();
        }
    }


}
