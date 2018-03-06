using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public static Player instance;

    /*
    =상태=
- 일반
- 달리기
- 점프

=Normal=
일반 공격
반대 공격

=Long=
지속 공격

=Batter=
연타 공격
    */

    Animator ani;
    AudioSource audioPlayer;

    Rigidbody2D ri;

    [SerializeField]
    bool isRun = false;

    [SerializeField]
    bool[] isMaintain = new bool[2] { false, false };

    public int dir = 1;
    public float moveSpeed = 0;
    public float jumpPower = 100f;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        ri = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
        audioPlayer = GetComponent<AudioSource>();
        SetAniSpeed(1f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            isRun = !isRun;
            ani.SetBool("Run", isRun);
            if (isRun)
                moveSpeed = 0.3f;
            else
                moveSpeed = 0f;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ri.velocity = new Vector2(0, jumpPower);
            ani.SetTrigger("Jump");
        }

        if (Input.GetKeyDown(KeyCode.Z))
            SetDir(-dir);

    }

    /// <summary>
    /// 일반 타격 액션을 취합니다.
    /// </summary>
    /// <param name="_lineNum">판정 방향</param>
    public void PlayAction(int _lineNum)
    {

        switch (_lineNum)
        {
            //왼쪽
            case 0:
                ani.SetFloat("LeftNormal", Random.Range(0, 1));
                ani.SetTrigger("LeftArm");
                break;
            //오른쪽
            case 1:
                ani.SetFloat("RightNormal", Random.Range(0, 1));
                ani.SetTrigger("RightArm");
                break;
            //연타 공격
            case 2:
                ani.SetFloat("LeftNormal", Random.Range(0, 2));
                ani.SetTrigger("LeftArm");

                ani.SetFloat("RightNormal", Random.Range(0, 2));
                ani.SetTrigger("RightArm");
                break;
        }
    }

    /// <summary>
    /// 지속 타격 액션을 취합니다.
    /// </summary>
    /// <param name="_lineNum">판정 방향</param>
    /// <param name="_during">진행 상황</param>
    public void PlayAction(int _lineNum, float _during)
    {

        if (Mathf.Abs(_during) == 1)
        {
            if (ConvertDir(_lineNum) == -1)
            {
                ani.SetFloat("LeftMaintain", -1);
                isMaintain[0] = false;
            }
            else if (ConvertDir(_lineNum) == 1)
            {
                ani.SetFloat("RightMaintain", -1);
                isMaintain[1] = false;
            }
            return;
        }

        if (ConvertDir(_lineNum) == -1)
        {
            ani.SetFloat("LeftNormal", -1);
            ani.SetFloat("LeftMaintain", _during);

            if (_during > 0f && !isMaintain[0])
            {
                isMaintain[0] = true;
                ani.SetTrigger("LeftArm");
            }

        }
        else if (ConvertDir(_lineNum) == 1)
        {
            ani.SetFloat("RightNormal", -1);
            ani.SetFloat("RightMaintain", _during);

            if (_during > 0f && !isMaintain[1])
            {
                isMaintain[1] = true;
                ani.SetTrigger("RightArm");
            }

        }

    }

    /// <summary>
    /// 방향을 지정합니다.
    /// </summary>
    /// <param name="_dir">-1 = Left , 1 = Right</param>
    public void SetDir(int _dir)
    {
        dir = _dir;

        if (_dir == -1)
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        else
            transform.localRotation = Quaternion.Euler(0, 0, 0);

    }

    private int ConvertDir(int _lineNum)
    {
        return dir * (_lineNum == 0 ? -1 : 1);
    }

    public void ShotAudio()
    {
        audioPlayer.PlayOneShot(audioPlayer.clip, 0.5f);
    }

    /// <summary>
    /// 애니메이션의 스피드를 제어합니다.
    /// </summary>
    /// <param name="_speed"></param>
    public void SetAniSpeed(float _speed)
    {
        ani.speed = _speed;
    }

}
