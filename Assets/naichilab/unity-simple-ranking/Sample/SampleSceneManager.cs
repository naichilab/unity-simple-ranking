using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SampleSceneManager : MonoBehaviour
{
    public Text scoreText;
    [NonSerialized] int score = 0;

    public void OnPlusButtonPressed()
    {
        score++;
        scoreText.text = score.ToString();
    }

    public void OnMinusButtonPressed()
    {
        score--;
        scoreText.text = score.ToString();
    }

    public void OnResultButton0Pressed()
    {
        naichilab.RankingLoader.Instance.SendScoreAndShowRanking(score, 0);
    }

    public void OnResultButton1Pressed()
    {
        naichilab.RankingLoader.Instance.SendScoreAndShowRanking(score, 1);
    }

    public void LocalSaveReset()
    {
        score = 0;
        scoreText.text = score.ToString();
        PlayerPrefs.DeleteAll();
    }
}