using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
public class Joystick : MonoBehaviour
{
    public struct TouchInfo
    {
        public bool valid;
        public Touch touch;
        public TouchInfo(bool valid, Touch touch)
        {
            this.valid = valid;
            this.touch = touch;
        }
    }

    [SerializeField] protected RectTransform ring;
    [SerializeField] protected RectTransform enableArea;
    [SerializeField] protected CanvasGroup canvasGroup;
    public Vector2 Output { get; private set; }
    public static Joystick instance { get; private set; }
    protected bool activated = false;
    protected bool lifted = true;
    protected int currentFinger=-1;

    protected virtual void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        RenderJoystick(false);
    }

    private void Update()
    {
        if(Input.touchCount <= 2 && Input.touchCount > 0)
        {
            TouchInfo touchInfo = GetTouch();
            if(touchInfo.valid)
            {
                switch (touchInfo.touch.phase)
                {
                    case TouchPhase.Began:
                        OnTouchDown(touchInfo.touch);
                        break;
                    case TouchPhase.Moved:
                        OnTouchDrag(touchInfo.touch);
                        break;
                    case TouchPhase.Stationary:
                        OnTouchDrag(touchInfo.touch);
                        break;
                    case TouchPhase.Canceled:
                        OnTouchUp();
                        break;
                    default:
                        OnTouchUp();
                        break;
                }
            }         
        }
        else 
        {
            if (!lifted)
            {
                OnTouchUp();
                currentFinger = -1;
            }
        }
        
    }

    private TouchInfo GetTouch()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(enableArea, touch.position))
                {
                    currentFinger = touch.fingerId;
                    return new TouchInfo(true, touch);
                }
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                if (touch.fingerId == currentFinger)
                    return new TouchInfo(true, touch);
            }
            else if (touch.phase == TouchPhase.Stationary)
            {
                if (touch.fingerId == currentFinger)
                    return new TouchInfo(true, touch);
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                if (touch.fingerId == currentFinger)
                {
                    currentFinger = -1;
                    return new TouchInfo(true, touch);
                }                
            }
        }
        return new TouchInfo(false, new Touch());
    }

    public virtual void OnTouchDown(Touch touch)
    {
        activated = true;
        lifted = false;
        ring.transform.position = touch.position;
        OnTouchDrag(touch);
        RenderJoystick(true);
    }

    public virtual void OnTouchDrag(Touch touch)
    {
        transform.position = touch.position;
        Vector2 swipeLength = (touch.position - (Vector2)ring.position);
        transform.localPosition = Vector2.ClampMagnitude(swipeLength, ring.rect.width * 0.5f);
        Output = transform.localPosition / (ring.rect.width / 2);
    }

    public virtual void OnTouchUp()
    {
        transform.localPosition = Vector3.zero;
        Output = Vector2.zero;
        activated = false;
        RenderJoystick(false);
        lifted = true;
    }

    protected virtual void RenderJoystick(bool render)
    {
        canvasGroup.alpha = render ? 1 : 0;
    }
}
