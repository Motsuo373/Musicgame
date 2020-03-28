using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UniRx;
using UniRx.Triggers;

public class GameManager : MonoBehaviour
{
    [SerializeField] string FilePath;
    [SerializeField] string MoviePath;

    [SerializeField] Button Play;
    [SerializeField] Button SetChart;

    [SerializeField] GameObject Blue;
    [SerializeField] GameObject Red;

    [SerializeField] Transform SpawnPoint;
    [SerializeField] Transform BeatPoint;

    //動画関係
    VideoPlayer Movie;
    [SerializeField] GameObject Screen;

    float PlayTime; //ゲーム開始時間
    float Distance; //判定位置までの距離
    float During; //初期位置から判定までの時間
    bool isPlaying; //ゲーム中か
    int GoIndex; //Notesの発射対象のノーツのインデックス


    string Title;
    int BPM;
    List<GameObject> Notes;


    void OnEnable()
    {
        Movie = this.GetComponent<VideoPlayer>();


        //ノーツから初期位置までにかける時間を2000msとする
        Distance = Math.Abs(BeatPoint.position.x - SpawnPoint.position.x);
        During = 2 * 1000;
        isPlaying = false;
        GoIndex = 0;

        //調整用
        Debug.Log(Distance);


        //ボタンに関する設定
        Play.onClick
          .AsObservable()
          .Subscribe(_ => play());

        SetChart.onClick
          .AsObservable()
          .Subscribe(_ => loadChart());

        this.UpdateAsObservable()
            .Where(_ => isPlaying)
            .Where(_ => Notes.Count > GoIndex)
            .Where(_ => Notes[GoIndex].GetComponent<NoteController>().getTiming() <= ((Time.time * 1000 - PlayTime) + During))
            .Subscribe(_ => {
                Notes[GoIndex].GetComponent<NoteController>().go(Distance, During);
                GoIndex++;
            });

    }

    void loadChart()
    {
        //動画のパスの設定
        Movie.clip = (VideoClip)Resources.Load(MoviePath);

        Notes = new List<GameObject>();

        string jsonText = Resources.Load<TextAsset>(FilePath).ToString(); //FilePath⇒譜面のJsonファイルパス

        JsonNode json = JsonNode.Parse(jsonText);
        Title = json["title"].Get<string>();
        BPM = int.Parse(json["bpm"].Get<string>());

        foreach(var note in json["notes"]) //jsonファイルから読み込み
        {
            string type = note["type"].Get<string>();
            float timing = float.Parse(note["timing"].Get<string>());

            GameObject Note;  //Objectの生成
            if (type == "red")
            {
                Note = Instantiate(Red, SpawnPoint.position, Quaternion.identity);
            } else if (type == "blue")
            {
                Note = Instantiate(Blue, SpawnPoint.position, Quaternion.identity);
            }
            else
            {
                Note = Instantiate(Red, SpawnPoint.position, Quaternion.identity); // default red
            }

            // setParameter関数を発火
            Note.GetComponent<NoteController>().setParameter(type, timing);
            Notes.Add(Note);
        }
    }
    void play()
    {
        //動画関係
        //なんかここ違う気がするな・・・
        var videoPlayer = Screen.AddComponent<VideoPlayer>(); //Videoコンポーネントの追加
        videoPlayer.clip = Movie.clip;

        Movie.Stop();
        Movie.Play();

        PlayTime = Time.time * 1000;
        isPlaying = true;
        Debug.Log("Game Start");
    }
    
}
