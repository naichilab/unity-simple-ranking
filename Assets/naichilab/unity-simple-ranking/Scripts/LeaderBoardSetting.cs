using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace naichilab
{
    /// <summary>
    /// １つのリーダーボード情報
    /// </summary>
    [CreateAssetMenu]
    public class LeaderBoardSetting : ScriptableObject
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
    }
}