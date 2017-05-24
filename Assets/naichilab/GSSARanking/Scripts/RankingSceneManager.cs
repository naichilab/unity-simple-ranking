using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GSSA;
using System.Linq;

namespace naichilab
{
	public class RankingSceneManager : MonoBehaviour
	{
		[SerializeField] Text scoreLabel;
		[SerializeField] Text highScoreLabel;
		[SerializeField] InputField nameInputField;
		[SerializeField] Button sendScoreButton;
		[SerializeField] Button closeButton;
		[SerializeField] RectTransform scrollViewContent;
		[SerializeField] GameObject rankingNodePrefab;
		[SerializeField] GameObject readingNodePrefab;
		[SerializeField] GameObject notFoundNodePrefab;


		private SpreadSheetObject highScoreSpreadSheetObject;

		/// <summary>
		/// 入力した名前
		/// </summary>
		/// <value>The name of the inputted.</value>
		private string InputtedNameForSave {
			get {
				if (string.IsNullOrEmpty (this.nameInputField.text)) {
					return "名無し";
				}
				return this.nameInputField.text;
			}
		}

		void Start ()
		{
			this.sendScoreButton.interactable = false;

			StartCoroutine (GetHighScoreAndRankingBoard ());
		}

		IEnumerator GetHighScoreAndRankingBoard ()
		{
			this.scoreLabel.text = RankingLoader.Instance.Score.TextForDisplay;
				
			//ハイスコア取得
			{
				this.highScoreLabel.text = "取得中...";

				var hiScoreCheck = new SpreadSheetQuery ();
				yield return hiScoreCheck.Where ("id", "=", SpreadSheetSetting.Instance.UniqueID).FindAsync ();

				if (hiScoreCheck.Count > 0) {
					//既にハイスコアは登録されている
					highScoreSpreadSheetObject = hiScoreCheck.Result.First ();

					var s = RankingLoader.Instance.BuildScore (highScoreSpreadSheetObject ["hiscore"].ToString ());
					this.highScoreLabel.text = s != null ? s.TextForDisplay : "エラー";

					this.nameInputField.text = highScoreSpreadSheetObject ["name"].ToString ();
				} else { 
					//登録されていない
					this.highScoreLabel.text = "-----";
				}
			}

			//ランキング取得
			yield return StartCoroutine (LoadRankingBoard ());


			//スコア更新している場合、ボタン有効化
			if (this.highScoreSpreadSheetObject == null) {
				this.sendScoreButton.interactable = true;
			} else {
				var highScore = RankingLoader.Instance.BuildScore (this.highScoreSpreadSheetObject ["hiscore"].ToString ());
				var score = RankingLoader.Instance.Score;

				if (RankingLoader.Instance.setting.Order == ScoreOrder.OrderByAscending) {
					//数値が低い方が高スコア
					this.sendScoreButton.interactable = score.Value < highScore.Value;
				} else {
					//数値が高い方が高スコア
					this.sendScoreButton.interactable = highScore.Value < score.Value;
				}
			}
		}


		public void SendScore ()
		{
			StartCoroutine (SendScoreEnumerator ());			
		}

		private IEnumerator SendScoreEnumerator ()
		{
			this.sendScoreButton.interactable = false;
			this.highScoreLabel.text = "送信中...";

			//ハイスコア送信
			if (this.highScoreSpreadSheetObject == null) {
				this.highScoreSpreadSheetObject = new SpreadSheetObject ();
				this.highScoreSpreadSheetObject ["id"] = SpreadSheetSetting.Instance.UniqueID;
			}

			this.highScoreSpreadSheetObject ["name"] = this.InputtedNameForSave;
			this.highScoreSpreadSheetObject ["hiscore"] = RankingLoader.Instance.Score.TextForSave;
			yield return this.highScoreSpreadSheetObject.SaveAsync ();

			this.highScoreLabel.text = RankingLoader.Instance.Score.TextForDisplay;

			yield return StartCoroutine (LoadRankingBoard ());
		}


		/// <summary>
		/// ランキング取得＆表示
		/// </summary>
		/// <returns>The ranking board.</returns>
		private IEnumerator LoadRankingBoard ()
		{
			int nodeCount = scrollViewContent.childCount;
			Debug.Log (nodeCount);
			for (int i = nodeCount - 1; i >= 0; i--) {
				Destroy (scrollViewContent.GetChild (i).gameObject);
			}

			var msg = Instantiate (readingNodePrefab, scrollViewContent);

			var so = new SpreadSheetQuery ();
			yield return so.OrderByDescending ("hiscore").Limit (30).FindAsync ();

			Debug.Log ("count : " + so.Count.ToString ());
			Destroy (msg);

			if (so.Count > 0) {

				int rank = 0;
				foreach (var r in so.Result) {

					var n = Instantiate (this.rankingNodePrefab, scrollViewContent);
					var rankNode = n.GetComponent<RankingNode> ();
					rankNode.NoText.text = (++rank).ToString ();
					rankNode.NameText.text = r ["name"].ToString ();

					var s = RankingLoader.Instance.BuildScore (r ["hiscore"].ToString ());
					rankNode.ScoreText.text = s != null ? s.TextForDisplay : "エラー";

					Debug.Log (r ["hiscore"].ToString ());
				}

			} else {
				Instantiate (this.notFoundNodePrefab, scrollViewContent);
			}
		}

		public void OnCloseButtonClick ()
		{
			this.closeButton.interactable = false;
			UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync ("Ranking");
		}
	}
}