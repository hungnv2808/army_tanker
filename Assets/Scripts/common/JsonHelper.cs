using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonHelper
{
    public class Wrap<M> 
    {
        public M Obj;
        public Wrap(M obj) {
            this.Obj = obj;
        }
    }
    public static string ToJson<T>(T obj) {
        Wrap<T> wrap = new Wrap<T>(obj);
        return JsonUtility.ToJson(wrap, true);
    }
    public static T FormJon<T> (string json) {
        var temp = JsonUtility.FromJson<Wrap<T>>(json);
        return temp.Obj;
    }
}
