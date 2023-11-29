using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBulletPrefab : MonoBehaviour
{
    public BulletInfo BulletInfo;
    // Component
    private Rigidbody2D rb;
    private float _speed;
    private Vector2 _dir;

    bool HasInputAuthority;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        rb.velocity = _speed * _dir * Time.fixedDeltaTime;
    }

    public void NetworkUpdate(BulletInfo bulletInfo)
    {
        BulletInfo = bulletInfo;
        if (!bulletInfo.alive)
            Destroy(gameObject);
    }
    public void InitBullet(Vector2 dir, BulletInfo bulletInfo, float speed, bool hasInputAuthority)
    {
        if (bulletInfo != null)
            BulletInfo = bulletInfo;

        _dir = dir;
        _speed = speed;
        HasInputAuthority = hasInputAuthority;
    }
}
