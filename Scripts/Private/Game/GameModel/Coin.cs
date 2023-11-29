using System;
using System.Collections.Generic;
using UnityEngine;


public class Coin
{
    public int CoinID;
    public bool active;
    public Vector3 Position;

    public Coin()
    {
        active = false;
        Position = Vector3.zero;
    }

    public void InitCoin(int coinId, Vector3 position)
    {
        CoinID = coinId;
        active = true;
        Position = position;
    }

    public void RemoveCoin()
    {
        active = false;
    }

    public List<object> CreateSerializeObject()
    {
        List<object> retv = new List<object>();
        retv.Add(CoinID);
        retv.Add(active);
        retv.Add(Position.x);
        retv.Add(Position.y);

        return retv;
    }

    public void DeserializeObject(object[] retv)
    {
        CoinID = Convert.ToInt32(retv[0]);
        active = (bool)retv[1];
        Position = new Vector2(Convert.ToSingle(retv[2]), Convert.ToSingle(retv[3]));
    }
}