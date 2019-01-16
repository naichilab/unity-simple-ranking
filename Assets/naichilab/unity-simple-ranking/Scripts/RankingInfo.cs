using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace naichilab
{
    /// <summary>
    /// １つのリーダーボード情報
    /// </summary>
    [CreateAssetMenu]
    public class RankingInfo : ScriptableObject
    {
        /// <summary>
        /// リーダーボード名
        /// </summary>
        public string BoardName = "ハイスコアランキング";

        /// <summary>
        /// クラス名（NCMBオブジェクト名として使われる）
        /// </summary>
        public string ClassName = "HiScore";

        /// <summary>
        /// スコアタイプ（数値 or 時間）
        /// </summary>
        public ScoreType Type;

        /// <summary>
        /// 並び順
        /// asc:昇順（小さい方が高スコア）
        /// desc:降順（大きい方が高スコア）
        /// </summary>
        [Tooltip("ASC:数値が小さい方がハイスコア、DESC:逆")] public ScoreOrder Order;

        /// <summary>
        /// 表示カスタムフォーマット
        /// </summary>
        public string CustomFormat;


        /// <summary>
        /// テキストからIScoreを復元する
        /// </summary>
        /// <param name="scoreText"></param>
        /// <returns></returns>
        public IScore BuildScore(string scoreText)
        {
            try
            {
                switch (Type)
                {
                    case ScoreType.Number:
                        double d = double.Parse(scoreText);
                        return new NumberScore(d, CustomFormat);
                        break;
                    case ScoreType.Time:
                        long ticks = long.Parse(scoreText);
                        TimeSpan t = new TimeSpan(ticks);
                        return new TimeScore(t, CustomFormat);
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("不正なデータが渡されました。[" + scoreText + "]");
            }

            return null;
        }
    }
}