using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
public class AnimatorHelper 
{
    /// <summary>
    /// xử lý callback ngay khi animation chạy xong;
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public static async void RunActionSequence(Animator animator,int layerIndex, Callback callback) {
        while(AnimatorIsPlaying(animator, layerIndex)) {
            await Task.Yield();
            Debug.Log("loop AnimatorIsPlaying");

        }
        Debug.Log("loop AnimatorIsStop");
        callback();
    }
    private static bool AnimatorIsPlaying(Animator animator, int layerIndex){
        return animator.GetCurrentAnimatorStateInfo(layerIndex).length < animator.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime;
    }
}
