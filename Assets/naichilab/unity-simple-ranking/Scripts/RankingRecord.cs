using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace naichilab
{
    /// <summary>
    /// ランキングのレコード
    /// ・主にハッシュ値を作成する目的で用意しました
    /// </summary>
    public class RankingRecord
    {
        public string ObjectID { get; private set; }
        public string Name     { get; private set; }
        public IScore Score    { get; private set; }
        public string Hash     { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// ・DBから復元する場合はhashの引数があるものを使用します
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="name"></param>
        /// <param name="score"></param>
        /// <param name="hash"></param>
        public RankingRecord(string objectId, string name, IScore score)
        {
            ObjectID = objectId;
            Name     = name;
            Score    = score;
            Hash     = CalcHash();
        }
        public RankingRecord(string objectId, string name, IScore score, string hash) 
        {
            ObjectID = objectId;
            Name     = name;
            Score    = score;
            Hash     = hash;
        }

        /// <summary>
        /// ハッシュ計算
        /// ・計算が推測できないよう、通信パケットに含まれていないクライアントキーを混ぜ合わせています
        /// </summary>
        /// <returns></returns>
        string CalcHash()
        {
            return (ObjectID + Name + Score.TextForSave + NCMB.NCMBSettings.ClientKey).ToHMACSHA256();
        }

        /// <summary>
        /// ハッシュ値の検証
        /// </summary>
        /// <returns>true:OK、false:NG</returns>
        public bool VerifyHash()
        {
            return CalcHash() == Hash;
        }

        /// <summary>
        /// 名前変更
        /// ・ハッシュ値が再計算されることに注意してください
        /// </summary>
        public void ChangeName(string name)
        {
            Name = name;
            Hash = CalcHash();
        }
    }
}
