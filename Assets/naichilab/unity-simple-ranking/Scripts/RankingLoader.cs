using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using UnityEngine.Serialization;

namespace naichilab
{
    /// <summary>
    /// ランキング読み込みクラス
    /// </summary>
    public class RankingLoader : MonoBehaviour
    {
        /// <summary>
        /// リーダーボード一覧
        /// </summary>
        [SerializeField] public RankingBoards RankingBoards;

        /// <summary>
        /// 表示対象のボード
        /// </summary>
        [NonSerialized] public RankingInfo CurrentRanking;

        /// <summary>
        /// 直前のスコア
        /// </summary>
        [NonSerialized] public IScore LastScore;

        #region singleton

        private static RankingLoader instance;

        public static RankingLoader Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = (RankingLoader) FindObjectOfType(typeof(RankingLoader));

                    if (instance == null)
                    {
                        Debug.LogError(typeof(RankingLoader) + "is nothing");
                    }
                }

                return instance;
            }
        }

        #endregion

        void Start()
        {
            //Class名重複をチェック
            RankingBoards.CheckDuplicateClassName();
        }


        /// <summary>
        /// 時間型スコアの送信とランキング表示を行います
        /// </summary>
        /// <param name="time"></param>
        /// <param name="boardId"></param>
        public void SendScoreAndShowRanking(TimeSpan time, int boardId = 0)
        {
            var board = RankingBoards.GetRankingInfo(boardId);
            var sc = new TimeScore(time, board.CustomFormat);
            SendScoreAndShowRanking(sc, board);
        }

        /// <summary>
        /// 数値型スコアの送信とランキング表示を行います
        /// </summary>
        /// <param name="score"></param>
        /// <param name="boardId"></param>
        public void SendScoreAndShowRanking(double score, int boardId = 0)
        {
            var board = RankingBoards.GetRankingInfo(boardId);
            var sc = new NumberScore(score, board.CustomFormat);
            SendScoreAndShowRanking(sc, board);
        }

        private void SendScoreAndShowRanking(IScore score, RankingInfo board)
        {
            if (board.Type != score.Type)
            {
                throw new ArgumentException("スコアの型が違います。");
            }

            CurrentRanking = board;
            LastScore = score;
            SceneManager.LoadScene("Ranking", LoadSceneMode.Additive);
        }
    }
}