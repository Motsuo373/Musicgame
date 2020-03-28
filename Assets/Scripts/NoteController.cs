using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class NoteController : MonoBehaviour
{
    string Type; //ノーツのタイプ
    float Timing; //ノーツの発射タイミング

    float Distance; //判定位置までの距離
    float During; //初期位置から判定までの時間

    Vector3 firstPos; //初期の位置
    bool isGo;//ノーツが発射しているか
    float GoTime;

    void OnEnable()
    {
        isGo = false;
        firstPos = this.transform.position;
        //オブジェクトの向き調整
        transform.Rotate(new Vector3(90, 0, 0));

        this.UpdateAsObservable() //ノーツが発射しているかをチェックして、発射しているならノーツの位置を計算して動かす
            .Where(_ => isGo)
            .Subscribe(_ => { this.gameObject.transform.position = new Vector3(firstPos.x - Distance * (Time.time * 1000 - GoTime) / During, firstPos.y, firstPos.z); });
    }

    public void setParameter(string type, float timing)
    {
        Type = type;
        Timing = timing;
    }

    public string getType()
    {
        return Type;
    }

    public float getTiming()
    {
        return Timing;
    }

    public void go(float distance, float during)
    {
        Distance = distance;
        During = during;
        GoTime = Time.time * 1000;

        isGo = true;

    }


}

