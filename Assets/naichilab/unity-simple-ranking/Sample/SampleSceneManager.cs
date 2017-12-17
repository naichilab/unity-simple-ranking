using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SampleSceneManager : MonoBehaviour
{

	[SerializeField]
	Button minusButton;

	[SerializeField]
	Button plusButton;

	[SerializeField]
	Text scoreText;

	[SerializeField]
	Button resultButton;


	int score = 0;



	public void OnPlusButtonPresset ()
	{
		this.score++;
		this.scoreText.text = this.score.ToString ();
	}

	public void OnMinusButtonPresset ()
	{
		this.score--;
		this.scoreText.text = this.score.ToString ();
	}

	public void OnResultButtonPresset ()
	{
	    naichilab.RankingLoader.Instance.SendScoreAndShowRanking(this.score);
	}

    public void LocalSaveReset()
    {
        PlayerPrefs.DeleteAll();
    }

}
