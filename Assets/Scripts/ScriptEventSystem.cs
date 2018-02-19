using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class ScriptEventSystem
{
    
    [SerializeField]
    private bool isControllable;
    [SerializeField]
    [Range(-1, 1)]
    private float speed;
    [SerializeField]
    private int eventTimer;
    [SerializeField]
    private Canvas canvas;

    public bool IsControllable
    {
        get
        {
            return isControllable;
        }

        set
        {
            isControllable = value;
        }
    }

    public float Speed
    {
        get
        {
            return speed;
        }
        set
        {
            speed = value;
        }
    }

    public int EventTimer
    {
        get
        {
            return eventTimer;
        }
    }

    public IEnumerator EventRun()
    {
        
        for (int i = 0; i < eventTimer; i++)
        {
            canvas.enabled = false;
            Player.Instance.MyRigidbody.velocity = new Vector2(speed * 10, Player.Instance.MyRigidbody.velocity.y);
            yield return new WaitForSeconds(0.5f);
        }
        
    }
}
