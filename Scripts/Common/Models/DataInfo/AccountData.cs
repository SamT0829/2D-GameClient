using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAccountInfo
{
    public long Money = 0L;
    public long Diamond = 0L;


    [SerializeField]
    private long serverTimeCheckPoint = 0;
    public long ServerTime { get { return serverTimeCheckPoint; } set { serverTimeCheckPoint = value; } }


    public List<object> SerializeObject()
    {
        List<object> retv = new List<object>();
        retv.Add(Money);
        retv.Add(Diamond);

        return retv;
    }

    public void DeserializeObject(object[] retv)
    {
        Money = Convert.ToInt64(retv[0]);
        Diamond = Convert.ToInt64(retv[1]);
    }
}
