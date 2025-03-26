using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ScriptEase//なめらかな動きを表現するためのclass
{
    public static float BezierEazing(float t, float p1, float p2)
    {
        //bezier_easing(t,p1,p2);
        //0 <= t <= 1
        //p1,p2の値はおよそ0から1
        //returns eased value

        return 3 * t * (1 - t) * (1 - t) * p1 + 3 * (t * t) * (1 - t) * p2 + t * t * t;
    }

    /*
    具体例
    p1:0.1,p2:0.5 だんだん加速
    p1:0.5,p2:0.9 だんだん減速
    p1:0.33,p2:0.66 等速
    */
}