using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class AttackJoystick : Joystick
{
    protected bool canInvoke = true;
    public static event Action<Vector2> FireValue;
    public new static AttackJoystick instance { get; private set; }
    //[Range(0, 3)]
    //[SerializeField] float autoFireHardness;

    protected override void Awake()
    {
        base.Awake();
        instance = this;
    }

    private void Start()
    {
        RenderJoystick(false);
        //aim = Aim.instance;
    }

    public override void OnTouchDown(Touch touch)
    {      
        base.OnTouchDown(touch); 
        canInvoke = true;
    }

    public override void OnTouchDrag(Touch touch)
    {
        base.OnTouchDrag(touch);
        Vector2 swipe = touch.position - (Vector2)ring.position;
        Pet.instance.DrawLine(Output);
        //DisableOnBigRange(swipe);       
    }

    public override void OnTouchUp()
    {
        if (canInvoke)
        {
            if (Output.magnitude >= 0.5f)
                FireValue?.Invoke(Output);
        }
        base.OnTouchUp();
          
    }

 
    /*private void DisableOnBigRange(Vector2 swipeLength)
    {
        if (canvasGroup.alpha == 1)
        {
            if (swipeLength.magnitude > ring.rect.width / 2 + (ring.rect.width / 2) * autoFireHardness) 
            {
                if (canInvoke)
                {
                    FireValue?.Invoke(Output);
                    canInvoke = false;
                    OnTouchUp();
                    StartCoroutine(WaitAndDeactivate());
                }
            }
            else
            {
            }

        }
    }*/

    IEnumerator WaitAndDeactivate()
    {
        yield return new WaitForSeconds(0.1f);
        RenderJoystick(false);
    }

    protected override void RenderJoystick(bool render)
    {
        if(Pet.instance != null)
        {
            if (!render)
                Pet.instance.StopDrawing();
        }      
        base.RenderJoystick(render);

    }
}
