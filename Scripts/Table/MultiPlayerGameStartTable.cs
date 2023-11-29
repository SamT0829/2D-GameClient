using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

public class MultiPlayerGameStartTable : TableBase
{
    private const string s_id = "UID";
    private const string s_gameType = "GameType";
    private const string s_position = "Position";

    private Dictionary<GameType, Dictionary<int, Vector3>> _gameTypeStartPositionTable =
        new Dictionary<GameType, Dictionary<int, Vector3>>();

    public Vector3 GetStartPosition(GameType gameType, int UID)
    {
        Vector3 position = Vector3.zero;
        if (_gameTypeStartPositionTable.TryGetValue(gameType, out Dictionary<int, Vector3> positionTable))
        {
            if (positionTable.TryGetValue(UID, out position))
                return position;

        }

        Debug.LogFormat("Can't Get start position from {0} game type ,{1} uid", gameType, UID);
        return position;
    }


    protected override void OnRowParsed(List<object> rowContent)
    {
        int uid = rowContent[GetColumnNameIndex(s_id)] as ValueTypeWrapper<int>;
        string gameTypeName = rowContent[GetColumnNameIndex(s_gameType)] as ValueTypeWrapper<string>;
        GameType gameType = (GameType)Enum.Parse(typeof(GameType), gameTypeName);
        string position = rowContent[GetColumnNameIndex(s_position)] as ValueTypeWrapper<string>;
        object[] positionArray = JsonConvert.DeserializeObject<object[]>(position);
        Vector3 startPosition;
        try
        {
            startPosition = new Vector3(float.Parse(positionArray[0].ToString()), float.Parse(positionArray[1].ToString()), float.Parse(positionArray[2].ToString()));
        }
        catch
        {
            Console.WriteLine("'{0}' is not in a valid format.", position);
            return;
        }

        if (!_gameTypeStartPositionTable.TryGetValue(gameType, out Dictionary<int, Vector3> startPositionTable))
        {
            startPositionTable = new Dictionary<int, Vector3>();
            _gameTypeStartPositionTable[gameType] = startPositionTable;
        }
        if (!startPositionTable.TryGetValue(uid, out Vector3 vectorPosition))
        {
            vectorPosition = startPosition;
            startPositionTable[uid] = vectorPosition;
        }
    }

    protected override void OnTableParsed()
    {
    }
}
