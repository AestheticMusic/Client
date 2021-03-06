﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static float dragMinDistance = 200f;
    public static float dragMinDelta = 6f;

    public const float maxBeatInLine = 8f; // 4f
    public const float lineLength = 24.0f; // 12.0f
    public const float lineHeight = 6.5f;

    public string musicName;

    // [HideInInspector]
    public float bpm = 120.0f;
    // [HideInInspector]
    public float speed = 1f;
    [HideInInspector]
    public float musicSync = 0f;
    [HideInInspector]
    public float noteSync = 0f;
    // [HideInInspector]
    public float noteMinTime = 0f;
    // [HideInInspector]
    public float noteMaxTime = 0f;

    public bool auto = false;

    public bool isPlay = false;

    [HideInInspector]
    public NoteSpawn noteSpawn;
    [HideInInspector]
    public NoteJudgement noteJudgement;
    [HideInInspector]
    public int combo = 0;
    [HideInInspector]
    public string noteDataPath;

    [HideInInspector]
    public float time = -3f;
    float oldTime = 0;

    public float syncedTime
    {
        get
        {
            return time + musicSync + noteSync;
        }
    }
    public float oneBeatToLine
    {
        get
        {
            return lineLength / maxBeatInLine;
        }
    }

    public Transform[] lines;
    //public Text debugText, latencyCheckText;

    private MusicManager m;
    private GameUIManager ui;

    private void Awake()
    {
        instance = this;

        noteSpawn = this.GetComponentInChildren<NoteSpawn>();
        noteJudgement = this.GetComponentInChildren<NoteJudgement>();

        float widthRatio = Screen.width / 1920f;
        dragMinDistance = 200f * widthRatio;
        dragMinDelta = 6f * widthRatio;
    }

    private void Start()
    {
        m = MusicManager.instance;
        ui = GameUIManager.instance;

        DebugLoad();
        StartCoroutine(StartDelay());
    }

    private IEnumerator StartDelay()
    {
        time = -3f;
        m.Stop();
        isPlay = true;
        while (time < 0f)
            yield return null;
        m.Play();
        yield return null;
        time = 0f;
        m.musicTime = 0f;
    }

    private void DebugLoad()
    {
        //noteDataPath = "NoteDatas/DARK FORCE_Hard";
        //AudioClip music = Resources.Load<AudioClip>("Musics/DARK FORCE");
        noteDataPath = "NoteDatas/" + musicName;
        AudioClip music = Resources.Load<AudioClip>("Sounds/BGM/" + musicName);
        m.LoadMusic(music);
    }

    int latencyCnt = 0;

    private void FixedUpdate()
    {
        if (!isPlay)
            return;

        time += m.musicTime - oldTime;
        oldTime = m.musicTime;

        if (Mathf.Abs(time - m.musicTime) >= 0.05f)
        {
            //++latencyCnt;
            //latencyCheckText.text += latencyCnt + " : " + (time - m.musicTime) + "\n";
            time = m.musicTime;
        }
    }

    private void Update()
    {
        if (!isPlay)
            return;

        UpdateTimeLimits();

        //debugText.text = string.Format("D : {0:F3}, M : {1:F3} , S : {2:F3}, E : {3:F3}, FPS : {4:F2}", (time - m.musicTime), m.musicTime, syncedTime, Time.deltaTime, 1.0f / Time.deltaTime);
    }

    public Vector3 ScreenToLinePosition(Vector2 _scrn, int _lineNum)
    {
        Vector3 world = Camera.main.ScreenToWorldPoint(_scrn);
        return lines[_lineNum].InverseTransformPoint(world);
    }

    private void UpdateTimeLimits()
    {
        float length = MusicManager.instance.musicLength;
        noteMinTime = Mathf.Clamp(syncedTime - NoteJudgement.judgePerfect, 0f, length);
        noteMaxTime = Mathf.Clamp(syncedTime + maxBeatInLine * (120f / bpm) / speed, noteMinTime, length);
    }

    //public void SetDebugText(string data)
    //{
    //    debugText.text = data;
    //}

    public void SetAutoMode(bool _isOn)
    {
        auto = _isOn;
    }


    public void SetSpeed(float _amoount)
    {
        speed = _amoount;
    }
}
