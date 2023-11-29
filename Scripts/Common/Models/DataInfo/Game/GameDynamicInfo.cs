using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public enum MultiPlayerGameDynamicInfo
{
    GameState,
    Timer,
    CoinSpawner,
    BulletSpawner,
    MonsterSpawner,
}

public class GameDynamicInfo<TEnum> where TEnum : Enum
{
    private enum GameDynamicInfoKey
    {
        GameDynamicData,
    }

    private Dictionary<TEnum, object> gameDynamicDataTable = new Dictionary<TEnum, object>();

    public void AddData(TEnum gameDynamicDataKey, object data)
    {
        if (!gameDynamicDataTable.TryGetValue(gameDynamicDataKey, out object gameDynamicDataValue))
        {
            gameDynamicDataTable[gameDynamicDataKey] = data;
        }
    }

    public bool GetData<T>(TEnum gameDynamicDataKey, out T outData)
    {
        if (gameDynamicDataTable.TryGetValue(gameDynamicDataKey, out object gameDynamicDataValue))
        {
            if (ExtensionMethod.ChangeType(gameDynamicDataValue, out T gameDynamicData))
            {
                outData = gameDynamicData;
                return true;
            }
        }

        outData = default;
        return false;
    }

    private bool RetrivieData<T>(TEnum gameDynamicDataKey, out T data) where T : class
    {
        data = null;
        if (gameDynamicDataTable.TryGetValue(gameDynamicDataKey, out object msgData))
        {
            data = (T)msgData;
            if (data != null)
                return true;

            return false;
        }

        return false;
    }

    public void DeserializeGameStaticObject(Dictionary<string, object> gameDynamicInfoData)
    {
        if (DictionaryMethod.RetrivieClassData(gameDynamicInfoData, GameDynamicInfoKey.GameDynamicData, out Dictionary<string, object> gameDynamicData))
        {
            gameDynamicDataTable = gameDynamicData.ToDictionary(x =>
            {
                var result = Enum.Parse(typeof(TEnum), x.Key.ToString());
                return (TEnum)result;
            }, x => x.Value);
        }
    }
}