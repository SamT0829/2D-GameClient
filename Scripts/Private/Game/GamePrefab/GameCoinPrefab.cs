using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCoinPrefab : MonoBehaviour
{
    public Coin Coin;

    public void Init(Coin coin)
    {
        Coin = coin;
    }

    public void NetworkUpdate(Coin coin)
    {
        Coin = coin;  
        transform.position = coin.Position;
        gameObject.SetActive(coin.active);
    }
}