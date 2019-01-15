using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Serialization;

namespace naichilab
{
	public class RankingLoader:MonoBehaviour
	{
		[SerializeField]
		public LeaderBoards leaderBoards;

		[HideInInspector]
		[NonSerialized]
		public LeaderBoardSetting CurrentBoard;
		
		[HideInInspector]
		[NonSerialized]
		public IScore Score;

		#region singleton

		private static RankingLoader instance;

		public static RankingLoader Instance {
			get {
				if (instance == null) {
					instance = (RankingLoader)FindObjectOfType (typeof(RankingLoader));

					if (instance == null) {
						Debug.LogError (typeof(RankingLoader) + "is nothing");
					}
				}
				return instance;
			}
		}

		#endregion

		public void SendScoreAndShowRanking (TimeSpan time,int boardId=0)
		{
			CurrentBoard = leaderBoards.GetLeaderBoard(boardId);
			if (CurrentBoard.Type != ScoreType.Time) {
				throw new ArgumentException ("スコアの型が違います。");
			}

			this.Score = new TimeScore (time, CurrentBoard.CustomFormat);
			this.LoadRankingScene ();
		}

		public void SendScoreAndShowRanking (double score,int boardId=0)
		{
			CurrentBoard = leaderBoards.GetLeaderBoard(boardId);
			if (CurrentBoard.Type != ScoreType.Number) {
				throw new ArgumentException ("スコアの型が違います。");
			}

			this.Score = new NumberScore (score, CurrentBoard.CustomFormat);
			this.LoadRankingScene ();
		}

		private void LoadRankingScene ()
		{			
			SceneManager.LoadScene ("Ranking", LoadSceneMode.Additive);
		}

		public IScore BuildScore (string scoreText)
		{
			try {
				switch (CurrentBoard.Type) {
				case ScoreType.Number:
					double d = double.Parse (scoreText);
					return new NumberScore (d, CurrentBoard.CustomFormat);
					break;
				case ScoreType.Time:
					long ticks = long.Parse (scoreText);
					TimeSpan t = new TimeSpan (ticks);
					return new TimeScore (t, CurrentBoard.CustomFormat);
					break;
				}
			} catch (Exception ex) {
				Debug.LogWarning ("不正なデータが渡されました。[" + scoreText + "]");
			}

			return null;
		}
	}
}