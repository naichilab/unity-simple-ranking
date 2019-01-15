using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace naichilab
{
    /// <summary>
    /// スコア情報インターフェース
    /// </summary>
    public interface IScore
    {
        /// <summary>
        /// スコアタイプ
        /// </summary>
        ScoreType Type { get; }

        /// <summary>
        /// 表示用文字列
        /// </summary>
        string TextForDisplay { get; }

        /// <summary>
        /// 保存用文字列
        /// </summary>
        string TextForSave { get; }

        /// <summary>
        /// 値
        /// </summary>
        double Value { get; }
    }
}