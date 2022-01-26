using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;

public class Enemy : CustomizableCharacter
{
    /*
     * TODO
     * 
     * Player kodunu yazarken Slip() fonksiyonu tekrar incelenecek.
     */
    public void Slip()
    {
        if (initialFatness > 0)
        {
            Vector3 desiredPos = 0.4f * Vector3.back * (initialFatness - fatness) / initialFatness;
            characterTransform.localPosition = Vector3.MoveTowards(characterTransform.localPosition, desiredPos, Time.deltaTime * 0.4f);
        }
    }

    /*
     * TODO Die()
     * 
     * Bir enemy �ld���nde level�n bitip bitmedi�ini kontrol edece�im.
     * Geriye f�rlama animasyonu eklenecek.
     */
    public void Die()
    {
        DisableCollider();
        SetAnimatorTrigger("Die");
        ProjectileMotion.Simulate(_transform, _transform.position - _transform.forward * 2, 0.75f, () =>
        {
            _transform.LeanMoveY(-10, 1.44f);
            ObjectPooler.Instance.SpawnFromPool("Splash Effect", _transform.position  + Vector3.down * 0.45f, Quaternion.identity);
        });
        Level.Instance.RemoveEnemy(this);
    }

    protected override Vector3 CalculateLocalScale()
    {
        return new Vector3(1 + fatness * 3 / 100f, 1 + fatness * 3 / 200f, 1 + fatness * 3 / 100f);
    }
}
