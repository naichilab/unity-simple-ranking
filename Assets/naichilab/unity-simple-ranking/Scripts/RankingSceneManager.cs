using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using NCMB;
using NCMB.Extensions;
using UnityEngine.Serialization;

namespace naichilab
{
    public class RankingSceneManager : MonoBehaviour
    {
        private const string OBJECT_ID = "objectId";
        private const string COLUMN_SCORE = "score";
        private const string COLUMN_NAME = "name";


        [SerializeField] Text captionLabel;
        [SerializeField] Text scoreLabel;
        [SerializeField] Text highScoreLabel;
        [SerializeField] Text currentRankLabel;
        [SerializeField] InputField nameInputField;
        [SerializeField] Button sendScoreButton;
        [SerializeField] Button closeButton;
        [SerializeField] RectTransform scrollViewContent;
        [SerializeField] GameObject rankingNodePrefab;
        [SerializeField] GameObject readingNodePrefab;
        [SerializeField] GameObject notFoundNodePrefab;
        [SerializeField] GameObject unavailableNodePrefab;
        [SerializeField] RectTransform rivalScrollViewContent;

        private string _objectid = null;
        
        private int _currentRank = 0;

        private string ObjectID
        {
            get { return _objectid ?? (_objectid = PlayerPrefs.GetString(BoardIdPlayerPrefsKey, null)); }
            set
            {
                if (_objectid == value)
                    return;
                PlayerPrefs.SetString(BoardIdPlayerPrefsKey, _objectid = value);
            }
        }

        private string BoardIdPlayerPrefsKey
        {
            get { return string.Format("{0}_{1}_{2}", "board", _board.ClassName, OBJECT_ID); }
        }

        private RankingInfo _board;
        private IScore _lastScore;

        private NCMBObject _ncmbRecord;

        /// <summary>
        /// 入力した名前
        /// </summary>
        /// <value>The name of the inputted.</value>
        private string InputtedNameForSave
        {
            get
            {
                if (string.IsNullOrEmpty(nameInputField.text))
                {
                    return "名無し";
                }

                return nameInputField.text;
            }
        }

        void Start()
        {
            sendScoreButton.interactable = false;
            _board = RankingLoader.Instance.CurrentRanking;
            _lastScore = RankingLoader.Instance.LastScore;

            Debug.Log(BoardIdPlayerPrefsKey + "=" + PlayerPrefs.GetString(BoardIdPlayerPrefsKey, null));

            StartCoroutine(GetHighScoreAndRankingBoard());
        }

        IEnumerator GetHighScoreAndRankingBoard()
        {
            scoreLabel.text = _lastScore.TextForDisplay;
            captionLabel.text = string.Format("{0}ランキング", _board.BoardName);

            //ハイスコア取得
            {
                highScoreLabel.text = "取得中...";
                if(currentRankLabel) currentRankLabel.text = "取得中...";

                var hiScoreCheck = new YieldableNcmbQuery<NCMBObject>(_board.ClassName);
                hiScoreCheck.WhereEqualTo(OBJECT_ID, ObjectID);
                yield return hiScoreCheck.FindAsync();

                if (hiScoreCheck.Count > 0)
                {
                    //既にハイスコアは登録されている
                    _ncmbRecord = hiScoreCheck.Result.First();

                    var s = _board.BuildScore(_ncmbRecord[COLUMN_SCORE].ToString());
                    highScoreLabel.text = s != null ? s.TextForDisplay : "エラー";

                    nameInputField.text = _ncmbRecord[COLUMN_NAME].ToString();
                }
                else
                {
                    //登録されていない
                    highScoreLabel.text = "-----";
                    if (currentRankLabel)
                    {
                        currentRankLabel.text = "-----";
                    }
                }
            }

            //ランキング取得
            yield return StartCoroutine(LoadRankingBoard());

            //スコア更新している場合、ボタン有効化
            if (_ncmbRecord == null)
            {
                sendScoreButton.interactable = true;
            }
            else
            {
                var highScore = _board.BuildScore(_ncmbRecord[COLUMN_SCORE].ToString());

                if (_board.Order == ScoreOrder.OrderByAscending)
                {
                    //数値が低い方が高スコア
                    sendScoreButton.interactable = _lastScore.Value < highScore.Value;
                }
                else
                {
                    //数値が高い方が高スコア
                    sendScoreButton.interactable = highScore.Value < _lastScore.Value;
                }

                Debug.Log(string.Format("登録済みスコア:{0} 今回スコア:{1} ハイスコア更新:{2}", highScore.Value, _lastScore.Value,
                    sendScoreButton.interactable));
            }
        }


        public void SendScore()
        {
            StartCoroutine(SendScoreEnumerator());
        }

        private IEnumerator SendScoreEnumerator()
        {
            sendScoreButton.interactable = false;
            highScoreLabel.text = "送信中...";

            //ハイスコア送信
            if (_ncmbRecord == null)
            {
                _ncmbRecord = new NCMBObject(_board.ClassName);
                _ncmbRecord.ObjectId = ObjectID;
            }

            _ncmbRecord[COLUMN_NAME] = InputtedNameForSave;
            _ncmbRecord[COLUMN_SCORE] = _lastScore.Value;
            NCMBException errorResult = null;

            yield return _ncmbRecord.YieldableSaveAsync(error => errorResult = error);

            if (errorResult != null)
            {
                //NCMBのコンソールから直接削除した場合に、該当のobjectIdが無いので発生する（らしい）
                _ncmbRecord.ObjectId = null;
                yield return _ncmbRecord.YieldableSaveAsync(error => errorResult = error); //新規として送信
            }

            //ObjectIDを保存して次に備える
            ObjectID = _ncmbRecord.ObjectId;

            highScoreLabel.text = _lastScore.TextForDisplay;

            yield return StartCoroutine(LoadRankingBoard());
        }


        /// <summary>
        /// ランキング取得＆表示
        /// </summary>
        /// <returns>The ranking board.</returns>
        private IEnumerator LoadRankingBoard()
        {
            int nodeCount = scrollViewContent.childCount;
            for (int i = nodeCount - 1; i >= 0; i--)
            {
                Destroy(scrollViewContent.GetChild(i).gameObject);
            }

            nodeCount = rivalScrollViewContent.childCount;
            for (int i = nodeCount - 1; i >= 0; i--)
            {
                Destroy(rivalScrollViewContent.GetChild(i).gameObject);
            }

            var msg = Instantiate(readingNodePrefab, scrollViewContent);

            //2017.2.0b3の描画されないバグ暫定対応
            MaskOffOn();

            var so = new YieldableNcmbQuery<NCMBObject>(_board.ClassName);
            so.Limit = 30;
            if (_board.Order == ScoreOrder.OrderByAscending)
            {
                so.OrderByAscending(COLUMN_SCORE);
            }
            else
            {
                so.OrderByDescending(COLUMN_SCORE);
            }

            yield return so.FindAsync();

            Debug.Log("データ取得 : " + so.Count.ToString() + "件");
            // ランキングのラベルが登録されていたら自分のランキングを取得する
            if (currentRankLabel)
            {
                yield return FetchMyRank();
            }
            Destroy(msg);

            if (so.Error != null)
            {
                Instantiate(unavailableNodePrefab, scrollViewContent);
            }
            else if (so.Count > 0)
            {
                int rank = 0;
                foreach (var r in so.Result)
                {
                    var n = Instantiate(rankingNodePrefab, scrollViewContent);
                    var rankNode = n.GetComponent<RankingNode>();
                    rankNode.NoText.text = (++rank).ToString();
                    rankNode.NameText.text = r[COLUMN_NAME].ToString();
                    // 自分と同じObjectIDだったら色を赤にする
                    if (r.ObjectId == ObjectID)
                    {
                        SetNodeColorText(rankNode);
                    }

                    var s = _board.BuildScore(r[COLUMN_SCORE].ToString());
                    rankNode.ScoreText.text = s != null ? s.TextForDisplay : "エラー";

//                    Debug.Log(r[COLUMN_SCORE].ToString());
                }
            }
            else
            {
                Instantiate(notFoundNodePrefab, scrollViewContent);
            }
        }

        /// <summary>
        /// 今回のスコアでのランキングの情報を取得する
        /// </summary>
        private IEnumerator FetchMyRank()
        {
            var hiScoreCheck = new YieldableNcmbQuery<NCMBObject>(_board.ClassName);
            if (_board.Order == ScoreOrder.OrderByAscending)
            {
                //数値が低い方が高スコア
                hiScoreCheck.WhereLessThan(COLUMN_SCORE, _lastScore.Value);
            }
            else
            {
                //数値が高い方が高スコア
                hiScoreCheck.WhereGreaterThan(COLUMN_SCORE, _lastScore.Value);
            }

            int countResult = -1;
            NCMBException countError = null;

            hiScoreCheck.CountAsync((count, error) =>
            {
                countResult = count + 1;
                countError = error;
            });
            yield return new WaitWhile(() => countResult == -1 && countError == null);
            currentRankLabel.text =  countResult.ToString();
            if (rivalScrollViewContent)
            {
                FetchNearerScore(countResult);
            }
        }

        /// <summary>
        /// ライバルのスコアを獲得する
        /// </summary>
        /// <param name="myRank"></param>
        private void FetchNearerScore(int myRank)
        {
            var nearerCheck = new YieldableNcmbQuery<NCMBObject>(_board.ClassName);
            // ライバルのランキング取得
            int numSkip = myRank - 2;
            if(numSkip < 0) numSkip = 0;
            
            nearerCheck.Skip  = numSkip;
            nearerCheck.Limit = 5;
            if (_board.Order == ScoreOrder.OrderByAscending)
            {
                nearerCheck.OrderByAscending(COLUMN_SCORE);
            }
            else
            {
                nearerCheck.OrderByDescending(COLUMN_SCORE);
            }
            nearerCheck.FindAsync ((List<NCMBObject> objList ,NCMBException e) => {

                if (e != null) {
                    //検索失敗時の処理
                } else {
                    //検索成功時の処理
                    // 取得したレコードをHighScoreクラスとして保存
                    int rank = numSkip;
                    foreach (var r in objList)
                    {
                        var n = Instantiate(rankingNodePrefab, rivalScrollViewContent);
                        var rankNode = n.GetComponent<RankingNode>();
                        rankNode.NoText.text = (++rank).ToString();
                        rankNode.NameText.text = r[COLUMN_NAME].ToString();
                        // 自分と同じObjectIDだったら色を赤にする
                        if (r.ObjectId == ObjectID)
                        {
                            SetNodeColorText(rankNode);
                        }

                        var s = _board.BuildScore(r[COLUMN_SCORE].ToString());
                        rankNode.ScoreText.text = s != null ? s.TextForDisplay : "エラー";
                    }
                }
            });
        }
        
        private void SetNodeColorText(RankingNode node)
        {
            node.NoText.color = Color.red;
            node.NameText.color = Color.red;
            node.ScoreText.color = Color.red;
        }

        public void OnCloseButtonClick()
        {
            closeButton.interactable = false;
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("Ranking");
        }
        
        private void MaskOffOn()
        {
            //2017.2.0b3でなぜかScrollViewContentを追加しても描画されない場合がある。
            //親maskをOFF/ONすると直るので無理やり・・・
            var m = scrollViewContent.parent.GetComponent<Mask>();
            m.enabled = false;
            m.enabled = true;
        }
    }
}