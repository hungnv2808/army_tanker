using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    public static int BarrierOvercomeCount = 0;
    public static int BarrierOvercomeMaxCount = 5;
    public int Overcome() {
        BarrierOvercomeCount += 1;
        return BarrierOvercomeCount;
    }
}
