using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SampleSceneManager : MonoBehaviour
{
    // メモリを書き換えてハイスコアを改ざんするハッキングが存在します。
    // unity-simple-ranking 内で取り回しているスコアのメモリは書き換えを検知するようにしていますが、
    // このクラスの score は検知していません。
    // Webアプリであれば書き換えは難しい気がしますが、PC/iOS/Androidなどであれば、暗号化・もしくは書き換えを
    // 検知する仕組みにすると安全です。

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