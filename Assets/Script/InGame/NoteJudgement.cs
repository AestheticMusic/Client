﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteJudgement : MonoBehaviour
{
    public const float noteRadius = 1.0f;
    public const float noteTouchMargin = 1.0f;

    public const float judgePerfect = 0.108f;	// -13 ~ 13 frames - 0.216;
    public const float judgeNice = 0.183f;	//  14 ~ 22 frames - 0.366;
    public const float judgeGood = 0.2495f;	//  23 ~ 30 frames - 0.499;
    public const float judgePreMiss = 0.308f;   //  31 ~ 37 frames - 0.616;

    public BatterNote currentBatter = null;

    public GameObject explosionEffect;
    public AudioClip weaponSound;

    private GameManager g;
    private TouchManager t;
    private EffectManager e;

    private void Start()
    {
        g = GameManager.instance;
        t = TouchManager.instance;
        e = EffectManager.instance;
    }

    private void LateUpdate()
    {
        CheckInput();
        PlayerAction();
    }

    private void PlayerAction()
    {
        float[] lineProgress = new float[2] { -1f, -1f };
        foreach (Note n in g.noteSpawn.notes)
        {
            if (lineProgress[0] >= 0f && lineProgress[1] >= 0f)
                break;
            if (lineProgress[n.lineNum] >= 0f)
                continue;

            if (n.data.noteType == Note.N_LONG)
                lineProgress[n.lineNum] = ((LongNote)n).GetLongProgress();
        }
        if (lineProgress[0] > 0f)
            Player.instance.PlayAction(0, 2, lineProgress[0]);
        if (lineProgress[1] > 0f)
            Player.instance.PlayAction(1, 2, lineProgress[1]);
    }

    private void CheckInput()
    {
        if (t.infos.Count == 0)
            return;

        for (int i = 0; i < t.infos.Count; ++i)
        {
            TouchInfo info = t.infos[i];
            TouchLine(info);
        }
    }

    private void TouchLine(TouchInfo _info)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(_info.position);
        int lineNum = (worldPos.x < 0) ? 0 : 1;
        // Vector3 localPos = g.lines[lineNum].InverseTransformPoint(worldPos);
        // Vector3 startPos = g.lines[lineNum].InverseTransformPoint(Camera.main.ScreenToWorldPoint(_info.startPosition));

        // 연타 노트 체크.
        if (currentBatter != null)
        {
            if (_info.state == TouchState.Press)
                HitNote(currentBatter);
            return;
        }

        // Press의 경우.
        if (_info.state == TouchState.Press)
        {
            Note targetNote = null;
            for (int i = 0; i < g.noteSpawn.notes.Count; ++i)
            {
                Note n = g.noteSpawn.notes[i];
                if (n.lineNum != lineNum)
                    continue;
                Judge judge = CheckJudgement(n);
                if (judge == Judge.None || judge == Judge.Miss)
                    continue;
                if (n.data.noteType == Note.N_DRAG)
                    continue;

                if (targetNote == null || n.time < targetNote.time)
                    targetNote = n;
            }
            if (targetNote != null)
                HitNote(targetNote);
        }
        // Stay의 경우.
        else if (_info.state == TouchState.Stay)
        {
            DragNote targetDrag = null;
            for (int i = 0; i < g.noteSpawn.notes.Count; ++i)
            {
                Note n = g.noteSpawn.notes[i];
                if (n.lineNum != lineNum)
                    continue;

                // 롱 노트 체크.
                if (n.data.noteType == Note.N_LONG)
                {
                    LongNote ln = (LongNote)n;
                    if (ln.firstTouched)
                        ln.touched = true;
                }
                // 드래그 노트 체크.
                else if (n.data.noteType == Note.N_DRAG)
                {
                    Judge judge = CheckJudgement(n);
                    if (judge == Judge.None || judge == Judge.Miss)
                        continue;
                    DragNote dn = (DragNote)n;
                    if (targetDrag == null || n.time < targetDrag.time)
                        targetDrag = dn;
                }
            }
            if (targetDrag != null &&
                _info.CheckDrag() == targetDrag.drag * (targetDrag.lineNum == 1 ? -1 : 1))
            {
                HitNote(targetDrag);
                _info.ResetDrag();
            }
        }
    }

    public Judge CheckJudgement(Note _note)
    {
        float errorTime = _note.time - g.syncedTime;

        //print("errorTime : " + errorTime + " / judge : " + judgePerfect);

        if (_note.data.noteType == Note.N_BATTER)
            return Judge.Perfect;

        if (errorTime < -judgePerfect)
            return Judge.Miss;
        else if (Mathf.Abs(errorTime) <= judgePerfect)
            return Judge.Perfect;
        else if (errorTime <= judgeNice)
            return Judge.Nice;
        else if (errorTime <= judgeGood)
            return Judge.Good;
        else if (errorTime <= judgePreMiss)
            return Judge.PreMiss;
        return Judge.None;
    }

    public void HitNote(Note _note)
    {
        if (_note.data.noteType == Note.N_LONG)
        {
            LongNote ln = (LongNote)_note;
            if (ln.firstTouched)
                return;
            ln.firstTouched = true;
            ln.touched = true;
        }
        else if (_note.data.noteType == Note.N_BATTER)
            currentBatter.Hit();
        else
            g.noteSpawn.RemoveNote(_note);

        JudgementNote(CheckJudgement(_note), _note);
    }

    public void JudgementNote(Judge _judge, Note _note)
    {
        switch (_judge)
        {
            case Judge.Perfect:
            case Judge.Nice:
            case Judge.Good:
                CameraManager.instance.shake.ShakeCam();
                ++g.combo;
                break;
            case Judge.PreMiss:
            case Judge.Miss:
                g.combo = 0;
                break;
        }

        if (_judge != Judge.Miss)
        {
            MusicManager.instance.PlaySE(weaponSound, 0.3f);

            ObjectPoolManager.Instance.Get(explosionEffect.name).transform.position = _note.transform.position;

            if (_note.data.noteType != Note.N_LONG)
            {
                if (_note.data.noteType == Note.N_BATTER)
                {
                    Player.instance.PlayAction(2, 1);
                }
                else if (_note.data.noteType == Note.N_DRAG)
                {
                    Player.instance.PlayAction(_note.lineNum, 3);
                }
                else
                {
                    Player.instance.PlayAction(_note.lineNum, 0);
                }
            }
        }

        e.ShowJudgeEffect(_judge, _note.lineNum);
    }
}

public enum Judge
{
    Miss = -1,
    None = 0,
    Perfect = 1,
    Nice = 2,
    Good = 3,
    PreMiss = 4,
}