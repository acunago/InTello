using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MuestraScript : MonoBehaviour
{
    public UnityEvent onEvent;
    public MyCondition cond;
    bool myResult;
    // Start is called before the first frame update
    void Start()
    {
        onEvent.Invoke();
        myResult = cond.Invoke();
        Debug.Log("El resultado es" + myResult);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
[Serializable]
public class MyCondition : SerializableCallback<bool> { }