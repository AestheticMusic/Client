using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public static Player instance;

    /*
    공격 모션 설정  
    
    =Normal=
    일반 공격
    반대 공격
    
    =Long=
    지속 공격
    
    =Batter=
    연타 공격

    공격 애니메이션 분류

    공격 상태
      > 공격 방향
        > 공격 방식
         
    공격 상태 >> State
      - 0 : 스탠딩
      - 1 : 달리기
      - 2 : 에어리어 (점프 포함)

    공격 방향 > Dir
     - 0 : 단방향
     - 1 : 양방향

    공격 방식 >> Category
     - 0 : 단타 (Normal)
     - 1 : 연타 (Batter)
     - 2 : 지속 (Long)
     - 3 : 패링 (Drag)

    공격 방식 종류 >> Kind
    - 1/n (n은 애니메이션 갯수)
       
    */

    Animator ani;
    AudioSource audioPlayer;

    Rigidbody2D ri;

    [SerializeField]
    bool isRun = false;

    [SerializeField]
    bool[] isAttackDir = new bool[2] { false, false };

    [SerializeField]
    float[] attackDirResetTime = new float[2] { 0.5f, 0.5f };

    [SerializeField]
    float[] currentAttackDirResetTime = new float[2] { 0.5f, 0.5f };

    [Header("0:Normal, 1:Batter, 2:Long, 3:Drag")]
    public List<AnimationAmount> standAniAmountList;
    public List<AnimationAmount> runAniAmountList;
    public List<AnimationAmount> airAniAmountList;


    public int state = 0;
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
            state = 1;
            if (isRun)
                moveSpeed = 0.3f;
            else
                moveSpeed = 0f;
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            ri.velocity = new Vector2(0, jumpPower);
            state = 2;
        }
        else
        {
            state = 0;
        }

        ani.SetInteger("State", state);

        if (isAttackDir[0] && currentAttackDirResetTime[0] > 0f)
        {
            currentAttackDirResetTime[0] -= Time.deltaTime;
            if (currentAttackDirResetTime[0] > 0f)
                isAttackDir[0] = false;
        }


        if (isAttackDir[1] && currentAttackDirResetTime[1] > 0f)
        {
            currentAttackDirResetTime[1] -= Time.deltaTime;
            if (currentAttackDirResetTime[1] > 0f)
                isAttackDir[1] = false;
        }

    }

    /// <summary>
    /// 일반 타격 액션을 취합니다.
    /// </summary>
    /// <param name="_lineNum">판정 방향</param>
    public void PlayAction(int _lineNum, int _category, float _during = 0f)
    {
        switch (_lineNum)
        {
            //왼쪽
            case 0:
                dir = -1;
                SetDir(dir);
                isAttackDir[0] = true;
                currentAttackDirResetTime[0] = attackDirResetTime[0];
                break;
            //오른쪽
            case 1:
                dir = 1;
                SetDir(dir);
                isAttackDir[1] = true;
                currentAttackDirResetTime[1] = attackDirResetTime[1];
                break;
            //연타 노트
            case 2:
                isAttackDir[0] = isAttackDir[1] = true;
                break;
        }

        //좌우 동시
        if (isAttackDir[0] && isAttackDir[1])
        {
            ani.SetInteger("Dir", 1);
            ani.SetInteger("Category", _category);
            AnimationAmount amount = GetAnimationAmount(state, _category);
            ani.SetFloat("Kind", (1 / amount.multiple) * Random.Range(0, amount.multiple + 1));
            ani.SetTrigger("Action");
        }
        else if (isAttackDir[0] || isAttackDir[1])
        {
            ani.SetInteger("Dir", 0);
            ani.SetInteger("Category", _category);
            AnimationAmount amount = GetAnimationAmount(state, _category);
            ani.SetFloat("Kind", (1 / amount.single) * Random.Range(0, amount.single + 1));
            ani.SetTrigger("Action");
        }
        else
        {
            ani.SetInteger("Dir", -1);
        }
        print("action");
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

    public AnimationAmount GetAnimationAmount(int _state, int _category)
    {

        switch (_state)
        {
            case 0:
                return standAniAmountList.Find(item => item.category == _category);
            case 1:
                return runAniAmountList.Find(item => item.category == _category);
            case 2:
                return airAniAmountList.Find(item => item.category == _category);
        }

        return null;
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

[System.Serializable]
public class AnimationAmount
{
    public int category;

    public int single;
    public int multiple;

}