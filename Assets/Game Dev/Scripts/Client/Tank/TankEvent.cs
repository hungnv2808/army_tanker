using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankEvent 
{
    public const byte EVENT_SEND_DISPATCH_RECOIL_GUN_ANIMATION = 1;
    public const byte EVENT_SEND_DISPATCH_TURRENT_SHOOTED = 2;
    public const byte EVENT_SEND_DISPATCH_TURRENT_SHOOTED_NO_COUNTDOWN = 3;
    public const byte EVENT_SEND_DISPATCH_TURRENT_SHOOTED_NO_COUNTDOWN_STOP = 4;
    public const byte EVENT_SEND_DISPATCH_DEATH = 5;
    public const byte EVENT_SEND_DISPATCH_REVIVAL = 6;
}
