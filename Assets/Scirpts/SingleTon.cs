using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleTon<T> : BaseBehavior where T : SingleTon<T>
{
    private static T instance;
    public static T Instance{
        get {
            return instance;
        }
    }

    protected virtual void Awake() {
        if(instance != null)
        {
            if(instance != this)
                Destroy(gameObject);
        }
        else
        {
            instance = (T) this;
        }
    }

    public static bool isInitialize
    {
        get {return instance != null;}
    }

    protected virtual void OnDestroy()
    {
        if(instance == this)
        {
            instance = null;
        }
    }
}
