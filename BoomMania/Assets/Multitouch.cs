using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Multitouch : MonoBehaviour
{
    public static Multitouch instance;
    public Touch MoveTouch;
    public Touch FireTouch;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        MoveTouch.fingerId = -1;
        FireTouch.fingerId = -1;
    }
    private void Update()
    {
        foreach (Touch touch in Input.touches)
        {
            if(touch.phase == TouchPhase.Began)
            {
                if(touch.position.x < Screen.width / 2)
                {
                    MoveTouch = touch;
                }
                else
                {
                    FireTouch = touch;
                }
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                if (touch.fingerId == MoveTouch.fingerId)
                    MoveTouch = touch;
                if (touch.fingerId == FireTouch.fingerId)
                    FireTouch = touch;
            }
            else if(touch.phase == TouchPhase.Ended)
            {
                if (touch.fingerId == MoveTouch.fingerId)
                {
                    MoveTouch.phase = TouchPhase.Ended;
                    MoveTouch.fingerId = -1;
                }
                if (touch.fingerId == FireTouch.fingerId)
                {
                    FireTouch.phase = TouchPhase.Ended;
                    FireTouch.fingerId = -1;
                }
                  
            }
        }
    }
}
