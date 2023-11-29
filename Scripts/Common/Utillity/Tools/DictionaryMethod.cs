using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DictionaryMethod
{
    public static bool RetrivieSturctData<E, D>(Dictionary<string, object> message, E index, out D value) where D : struct where E : Enum
    {
        value = default;

        if (message.TryGetValue(index.ToString(), out object msgValue))
        {
            try
            {
                value = (D)msgValue;
            }
            catch
            {
                //轉型失敗  
                Debug.LogFormat("Message {0} is different types at {1}", typeof(E).Name, index);
                value = default;
                return false;
            }

            return true;
        }

        value = default;
        return false;
    }

    public static bool RetrivieClassData<E, C>(Dictionary<string, object> message, E index, out C value) where C : class where E : Enum
    {
        value = default;

        if (message.TryGetValue(index.ToString(), out object msgValue))
        {
            try
            {
                value = (C)msgValue;
            }
            catch
            {
                //轉型失敗  
                Debug.LogFormat("Message {0} is different types at {1}", typeof(E).Name, index);
                value = default;
                return false;
            }

            return true;
        }

        value = default;
        return false;
    }

    public static bool RetrivieSturctData<E, D>(Dictionary<int, object> message, E index, out D value) where D : struct where E : Enum
    {
        value = default;

        if (message.TryGetValue(index.GetHashCode(), out object msgValue))
        {
            try
            {
                value = (D)msgValue;
            }
            catch
            {
                //轉型失敗  
                Debug.LogFormat("Message {0} is different types at {1}", typeof(E).Name, index);
                value = default;
                return false;
            }

            return true;
        }

        value = default;
        return false;
    }

    public static bool RetrivieClassData<E, C>(Dictionary<int, object> message, E index, out C value) where C : class where E : Enum
    {
        value = default;

        if (message.TryGetValue(index.GetHashCode(), out object msgValue))
        {
            try
            {
                value = (C)msgValue;
            }
            catch
            {
                //轉型失敗  
                Debug.LogFormat("Message {0} is different types at {1}", typeof(E).Name, index);
                value = default;
                return false;
            }

            return true;
        }

        value = default;
        return false;
    }
}
