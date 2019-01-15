using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace naichilab
{
    [CreateAssetMenu]
    public class LeaderBoardSetting : ScriptableObject
    {
        public string BoardName = "ハイスコア";
        public string ClassName = "HiScore";
        
        public ScoreType Type;
        [Tooltip("ASC:数値が小さい方がハイスコア、DESC:逆")] public ScoreOrder Order;
        public string CustomFormat;
    }
}