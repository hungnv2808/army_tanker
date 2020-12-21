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
    public static async void RunActionSequence(Animator animator, Callback callback) {
        while(AnimatorIsPlaying(animator)) {
            await Task.Yield();
            // Debug.Log("loop AnimatorIsPlaying");
        }
        callback();
    }
    private static bool AnimatorIsPlaying(Animator animator){
        return animator.GetCurrentAnimatorStateInfo(0).length <= animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }
}
