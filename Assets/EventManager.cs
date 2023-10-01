using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager instance;

    public static event Action OnConnectNodes;
    public static event Action OnCancelConnection;
    public static event Action OnButtonClick;
    public static event Action OnPipePlace;
    public static event Action OnVictory;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public static void PipePlace()
    {
        OnPipePlace?.Invoke();
    }

    public static void ConnectNodes()
    {
        OnConnectNodes?.Invoke();
    }

    public static void CancelConnection()
    {
        OnCancelConnection?.Invoke();
    }

    public static void ButtonClick()
    {
        OnButtonClick?.Invoke();
    }

    public static void Victory()
    {
        OnVictory?.Invoke();
    }

}
