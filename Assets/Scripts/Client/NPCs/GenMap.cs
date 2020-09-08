using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class GenMap : MonoBehaviour
{
    private static GenMap s_instance;
    public MovementNode[] Team0Roots;
    public MovementNode[] Team1Roots;
    public static GenMap Instance {
        get {
            return s_instance;
        }
    }
    private void Awake() {
        if (s_instance != null && s_instance != this) {
            Destroy(s_instance);
        }
        s_instance = this;
    }
    
}
