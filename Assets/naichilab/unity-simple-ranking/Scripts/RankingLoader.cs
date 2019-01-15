using UnityEngine;
using UnityEngine.SceneManagement;
using System;

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
        [SerializeField] public LeaderBoards leaderBoards;

        /// <summary>
        /// 表示対象のボード
        /// </summary>
        [NonSerialized] public LeaderBoardSetting CurrentBoard;

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


        /// <summary>
        /// 時間型スコアの送信とランキング表示を行います
        /// </summary>
        /// <param name="time"></param>
        /// <param name="boardId"></param>
        public void SendScoreAndShowRanking(TimeSpan time, int boardId = 0)
        {
            var board = leaderBoards.GetLeaderBoard(boardId);
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
            var board = leaderBoards.GetLeaderBoard(boardId);
            var sc = new NumberScore(score, board.CustomFormat);
            SendScoreAndShowRanking(sc, board);
        }

        private void SendScoreAndShowRanking(IScore score, LeaderBoardSetting board)
        {
            if (board.Type != score.Type)
            {
                throw new ArgumentException("スコアの型が違います。");
            }

            CurrentBoard = board;
            LastScore = score;
            SceneManager.LoadScene("Ranking", LoadSceneMode.Additive);
        }


        public IScore BuildScore(string scoreText)
        {
            try
            {
                switch (CurrentBoard.Type)
                {
                    case ScoreType.Number:
                        double d = double.Parse(scoreText);
                        return new NumberScore(d, CurrentBoard.CustomFormat);
                        break;
                    case ScoreType.Time:
                        long ticks = long.Parse(scoreText);
                        TimeSpan t = new TimeSpan(ticks);
                        return new TimeScore(t, CurrentBoard.CustomFormat);
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