using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

public class EventManager : MonoBehaviour
{
    private static EventManager _instance;
    public static EventManager Instance { get { return _instance; } }
    private Queue<EventCaller> waitingAddEventTable = new Queue<EventCaller>();
    private Queue<EventCaller> waitingRemoveEventTable = new Queue<EventCaller>();
    private Queue<IEvent> waitingSendEventList = new Queue<IEvent>();
    private Dictionary<Type, Dictionary<int, Action<IEvent>>> eventTable = new Dictionary<Type, Dictionary<int, Action<IEvent>>>();

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
            _instance = this;


        Application.runInBackground = true;
        DontDestroyOnLoad(gameObject);
    }


    public void RegisterEventListener<T>(int messageKey, Action<IEvent> callBack) where T : IEvent
    {
        EventCaller listener = new EventCaller(typeof(T), callBack, messageKey);
        waitingAddEventTable.Enqueue(listener);
    }

    public void UnRegisterEventListener<T>(int messageKey, Action<IEvent> callBack) where T : IEvent
    {
        EventCaller listener = new EventCaller(typeof(T), callBack, messageKey);
        waitingRemoveEventTable.Enqueue(listener);
    }

    public void SendEvent(IEvent callEvent)
    {
        waitingSendEventList.Enqueue(callEvent);
    }

    private void Update()
    {
        AddPendingEvents();
        SendPendingEvent();
        RemovePendingEvents();
    }

    private void AddPendingEvents()
    {
        while (waitingAddEventTable.Count > 0)
        {
            EventCaller listener = waitingAddEventTable.Dequeue();
            Dictionary<int, Action<IEvent>> callBackTable;
            Action<IEvent> callBackList;

            if (!eventTable.TryGetValue(listener.EventType(), out callBackTable))
            {
                callBackTable = new Dictionary<int, Action<IEvent>>();
                eventTable[listener.EventType()] = callBackTable;
            }

            if (!callBackTable.TryGetValue(listener.GetMessageKey(), out callBackList))
            {
                callBackTable[listener.GetMessageKey()] = listener.FunctionCall();
            }
            else
            {
                callBackTable[listener.GetMessageKey()] += listener.FunctionCall();
            }
        }
    }

    private void SendPendingEvent()
    {
        while (waitingSendEventList.Count > 0)
        {
            IEvent listener = waitingSendEventList.Dequeue();
            Dictionary<int, Action<IEvent>> callBackTable;
            Action<IEvent> callBackList;
            Type eventType = listener.GetEventType();

            if (!eventTable.TryGetValue(eventType, out callBackTable))
                continue;

            if (listener.GetSendAll())
            {
                foreach (Action<IEvent> actions in callBackTable.Values)
                {
                    actions.Invoke(listener);
                }
            }
            else
            {
                if (!callBackTable.TryGetValue(listener.GetMessageKey(), out callBackList))
                    continue;

                callBackList.Invoke(listener);
            }
        }
    }

    private void RemovePendingEvents()
    {
        while (waitingRemoveEventTable.Count > 0)
        {
            EventCaller listener = waitingRemoveEventTable.Dequeue();
            Dictionary<int, Action<IEvent>> callBackTable;
            Action<IEvent> callBackList;

            if (!eventTable.TryGetValue(listener.EventType(), out callBackTable))
                continue;

            if (!callBackTable.TryGetValue(listener.GetMessageKey(), out callBackList))
                continue;

            callBackList -= listener.FunctionCall();

            if (callBackList == null)
            {
                callBackTable.Remove(listener.GetMessageKey());
            }
        }
    }
}

