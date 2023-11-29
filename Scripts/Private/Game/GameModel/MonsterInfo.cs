using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterInfo
{
    public int MonsterID;
    public bool active;
    public float Speed;
    public Vector3 Position;

    public MonsterInfo()
    {
        active = false;
        Position = Vector3.zero;
    }

    public void InitMonster(int monsterId, Vector3 position, float speed)
    {
        MonsterID = monsterId;
        active = true;
        Position = position;
        Speed = speed;
    }


    public void Update(float deltaTime)
    {
    }


    public void Dispose()
    {
        active = false;
    }


    public void Attack(GamePlayerInfo gamePlayerInfo, float deltaTime)
    {
        if (!active)
            return;

        Position += (gamePlayerInfo.PlayerPosition - Position).normalized * Speed * deltaTime;
    }

    public List<object> CreateSerializeObject()
    {
        List<object> retv = new List<object>();
        retv.Add(MonsterID);
        retv.Add(active);
        retv.Add(Position.x);
        retv.Add(Position.y);

        return retv;
    }

    public void DeserializeObject(object[] retv)
    {
        MonsterID = Convert.ToInt32(retv[0]);
        active = (bool)retv[1];
        Position = new Vector2(Convert.ToSingle(retv[2]), Convert.ToSingle(retv[3]));
    }
}