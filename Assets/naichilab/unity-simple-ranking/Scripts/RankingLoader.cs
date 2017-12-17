using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

namespace naichilab
{
	public class RankingLoader:MonoBehaviour
	{
		[SerializeField]
		public ScoreTypeSetting setting;

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

		public void SendScoreAndShowRanking (TimeSpan time)
		{
			if (this.setting.Type != ScoreType.Time) {
				throw new ArgumentException ("スコアの型が違います。");
			}

			this.Score = new TimeScore (time, this.setting.CustomFormat);
			this.LoadRankingScene ();
		}

		public void SendScoreAndShowRanking (double score)
		{
			if (this.setting.Type != ScoreType.Number) {
				throw new ArgumentException ("スコアの型が違います。");
			}

			this.Score = new NumberScore (score, this.setting.CustomFormat);
			this.LoadRankingScene ();
		}

		private void LoadRankingScene ()
		{			
			SceneManager.LoadScene ("Ranking", LoadSceneMode.Additive);
		}

		public IScore BuildScore (string scoreText)
		{
			try {
				switch (this.setting.Type) {
				case ScoreType.Number:
					double d = double.Parse (scoreText);
					return new NumberScore (d, this.setting.CustomFormat);
					break;
				case ScoreType.Time:
					long ticks = long.Parse (scoreText);
					TimeSpan t = new TimeSpan (ticks);
					return new TimeScore (t, this.setting.CustomFormat);
					break;
				}
			} catch (Exception ex) {
				Debug.LogWarning ("不正なデータが渡されました。[" + scoreText + "]");
			}

			return null;
		}
	}
}