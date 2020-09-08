using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEvent 
{
    void SendDispatchShooted(byte eventCode);
    void SendDispatchDeath(byte eventCode, string whoDamage, int whoViewID);
    void SendDispatchShootedNoCountdown(byte eventCode, bool isShooted);
}
