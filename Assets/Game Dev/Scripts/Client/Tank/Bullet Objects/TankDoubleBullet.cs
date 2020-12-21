using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TankDoubleBullet : TankBullet
{
    // private void Update() {
    //     this.m_transform.Rotate(0, 0, Time.deltaTime * (-720.0f));  /*tốc độ quay 360 độ 1s*/
    // }
    public override void Explode() {
        GetEffect("Prefabs/Effect/ExplosionFireballFire", "ExplosionFireballFire");
    }
}