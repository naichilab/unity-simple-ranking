﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using NCMB;
using NCMB.Extensions;


namespace naichilab
{
    public class RankingSceneManager : MonoBehaviour
    {
        private const string OBJECT_ID = "objectId";
        private const string COLUMN_SCORE = "score";
        private const string COLUMN_NAME = "name";
        private const string COLUMN_HASH = "hash";

        // 表示するランキングデータの行数
        private const int MAX_VIEW_ROW = 30;

        // 読み込むランキングデータの行数
        // ・ハッシュ値による検証有効時のみ使用します。
        //   通信パケット改ざんが行われた場合、サーバ側ではじかない限りデータが登録されることは避けられません。
        //   表示行数分だけ読み込むと、不正なデータがはじかれた結果、表示に必要なデータが足りなくなってしまいます。
        //   仕方なく読み込み行数を増やしておくことで対応しておきます。
        private const int MAX_READ_ROW = 60;

        [SerializeField] Text captionLabel;
        [SerializeField] Text scoreLabel;
        [SerializeField] Text highScoreLabel;
        [SerializeField] InputField nameInputField;
        [SerializeField] Button sendScoreButton;
        [SerializeField] Button closeButton;
        [SerializeField] RectTransform scrollViewContent;
        [SerializeField] GameObject rankingNodePrefab;
        [SerializeField] GameObject readingNodePrefab;
        [SerializeField] GameObject notFoundNodePrefab;
        [SerializeField] GameObject unavailableNodePrefab;

        private string _objectid = null;

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
        private void CrearObjectID()
        {
            PlayerPrefs.SetString(BoardIdPlayerPrefsKey, null);
        }


        private string BoardIdPlayerPrefsKey
        {
            get { return string.Format("{0}_{1}_{2}", "board", _board.ClassName, OBJECT_ID); }
        }

        private RankingInfo _board;
        private RankingRecord _lastScoreRecord; // メモリ書き換えを警戒してハッシュ値を含むレコードで保持しています

        private NCMBObject _ncmbRecord;

        private bool EnabledVerifyHash
        {
            get { return RankingLoader.Instance.EnabledVerifyHash; }
        }

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
            _lastScoreRecord = new RankingRecord("default", RankingLoader.Instance.LastScore); // 後で書き換えるので名前は適当に登録しています

            Debug.Log(BoardIdPlayerPrefsKey + "=" + PlayerPrefs.GetString(BoardIdPlayerPrefsKey, null));

            StartCoroutine(GetHighScoreAndRankingBoard());
        }

        IEnumerator GetHighScoreAndRankingBoard()
        {
            scoreLabel.text = _lastScoreRecord.Score.TextForDisplay;
            captionLabel.text = string.Format("{0}ランキング", _board.BoardName);

            //ハイスコア取得
            RankingRecord hiScoreRecord = null;
            {
                highScoreLabel.text = "取得中...";

                var hiScoreCheck = new YieldableNcmbQuery<NCMBObject>(_board.ClassName);
                hiScoreCheck.WhereEqualTo(OBJECT_ID, ObjectID);
                yield return hiScoreCheck.FindAsync();

                if (hiScoreCheck.Count > 0)
                {
                    //既にハイスコアは登録されている
                    _ncmbRecord = hiScoreCheck.Result.First();

                    var s = _board.BuildScore(_ncmbRecord[COLUMN_SCORE].ToString());
                    if (s != null) {
                        hiScoreRecord = new RankingRecord(_ncmbRecord[COLUMN_NAME].ToString(),
                                                          s,
                                                          _ncmbRecord.ContainsKey(COLUMN_HASH) ? _ncmbRecord[COLUMN_HASH].ToString() : "");
                        if (EnabledVerifyHash && !hiScoreRecord.VerifyHash()) {
                            hiScoreRecord = null;
                        }
                    }

                    highScoreLabel.text = hiScoreRecord != null ? hiScoreRecord.Score.TextForDisplay : "エラー";
                    nameInputField.text = _ncmbRecord[COLUMN_NAME].ToString();
                }
                else
                {
                    //登録されていない
                    highScoreLabel.text = "-----";
                }
            }

            //ランキング取得
            yield return StartCoroutine(LoadRankingBoard());

            //スコア更新している場合、ボタン有効化
            if (hiScoreRecord == null)
            {
                sendScoreButton.interactable = true;
            }
            else
            {
                if (_board.Order == ScoreOrder.OrderByAscending)
                {
                    //数値が低い方が高スコア
                    sendScoreButton.interactable = _lastScoreRecord.Score.Value < hiScoreRecord.Score.Value;
                }
                else
                {
                    //数値が高い方が高スコア
                    sendScoreButton.interactable = hiScoreRecord.Score.Value < _lastScoreRecord.Score.Value;
                }

                Debug.Log(string.Format("登録済みスコア:{0} 今回スコア:{1} ハイスコア更新:{2}", hiScoreRecord.Score.Value, _lastScoreRecord.Score.Value,
                    sendScoreButton.interactable));
            }
        }


        public void SendScore()
        {
            StartCoroutine(SendScoreEnumerator());
        }

        private IEnumerator SendScoreEnumerator()
        {
            if (EnabledVerifyHash && !_lastScoreRecord.VerifyHash()) { // メモリが書き換えられてないかのチェック
                highScoreLabel.text = "検証エラー";
                yield break; 
            }

            sendScoreButton.interactable = false;
            highScoreLabel.text = "送信中...";

            //ハイスコア送信
            if (_ncmbRecord == null)
            {
                _ncmbRecord = new NCMBObject(_board.ClassName);
                _ncmbRecord.ObjectId = ObjectID;
            }

            _ncmbRecord[COLUMN_NAME] = InputtedNameForSave;
            _ncmbRecord[COLUMN_SCORE] = _lastScoreRecord.Score.Value;
            _lastScoreRecord.Name = InputtedNameForSave; // 名前変更
            _lastScoreRecord.RefreshHash(); // ハッシュ再計算
            _ncmbRecord[COLUMN_HASH] = _lastScoreRecord.Hash;
            NCMBException errorResult = null;

            yield return _ncmbRecord.YieldableSaveAsync(error => errorResult = error);

            if (errorResult != null)
            {
                //NCMBのコンソールから直接削除した場合に、該当のobjectIdが無いので発生する（らしい）
                _ncmbRecord.ObjectId = null;
                CrearObjectID();
                yield return _ncmbRecord.YieldableSaveAsync(error => errorResult = error); //新規として送信
            }

            //ObjectIDを保存して次に備える
            ObjectID = _ncmbRecord.ObjectId;

            highScoreLabel.text = _lastScoreRecord.Score.TextForDisplay;

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

            var msg = Instantiate(readingNodePrefab, scrollViewContent);

            //2017.2.0b3の描画されないバグ暫定対応
            MaskOffOn();

            var so = new YieldableNcmbQuery<NCMBObject>(_board.ClassName);
            so.Limit = EnabledVerifyHash ? MAX_READ_ROW : MAX_VIEW_ROW;
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
                    if (rank >= MAX_VIEW_ROW) break;

                    IScore highScore = _board.BuildScore(r[COLUMN_SCORE].ToString());
                    if (highScore == null) continue;

                    // ハッシュ検証
                    RankingRecord highScoreRecord = new RankingRecord(r[COLUMN_NAME].ToString(),
                                                                      highScore,
                                                                      r.ContainsKey(COLUMN_HASH) ? r[COLUMN_HASH].ToString() : "");
                    if (EnabledVerifyHash && !highScoreRecord.VerifyHash()) continue;

                    var n = Instantiate(rankingNodePrefab, scrollViewContent);
                    var rankNode = n.GetComponent<RankingNode>();
                    rankNode.NoText.text = (++rank).ToString();
                    rankNode.NameText.text = r[COLUMN_NAME].ToString();

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