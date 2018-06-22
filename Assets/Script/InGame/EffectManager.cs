using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
	public static EffectManager instance;
	
	public JudgeEffect[] judgePool;


	private void Awake()
	{
		instance = this;
	}

	private void Update()
	{

	}

	public void ShowJudgeEffect(Judge _judge, int _lineNum)
	{
		int index = _lineNum;
		judgePool[index].Show(_judge);
	}
}
