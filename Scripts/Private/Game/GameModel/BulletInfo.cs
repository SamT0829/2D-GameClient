using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletInfo
{
    public int BulletId;
    public bool alive;
    public long PlayerAcountId;

    public void InitBullet(long playerAcountId)
    {
        alive = true;
        PlayerAcountId = playerAcountId;
    }

    public List<object> CreateSerializeObject()
    {
        List<object> retv = new List<object>();
        retv.Add(BulletId);
        retv.Add(alive);
        retv.Add(PlayerAcountId);
        
        return retv;
    }

    public void DeserializeObject(object[] retv)
    {
        BulletId = Convert.ToInt32(retv[0]);
        alive = Convert.ToBoolean(retv[1]);
        PlayerAcountId = Convert.ToInt64(retv[2]);
    }
}
