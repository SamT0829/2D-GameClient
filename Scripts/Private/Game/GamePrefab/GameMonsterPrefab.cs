using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMonsterPrefab : MonoBehaviour
{
    public MonsterInfo MonsterInfo;

    public void Init(MonsterInfo monsterInfo)
    {
        MonsterInfo = monsterInfo;
    }

    public void NetworkUpdate(MonsterInfo monsterInfo)
    {
        MonsterInfo = monsterInfo;
        transform.position = MonsterInfo.Position;
        gameObject.SetActive(MonsterInfo.active);
    }
}
