using UnityEngine;

namespace naichilab
{
    /// <summary>
    /// リーダーボード一覧
    /// </summary>
    [CreateAssetMenu]
    public class LeaderBoards : ScriptableObject
    {
        /// <summary>
        /// リーダーボード一覧
        /// </summary>
        [SerializeField] private LeaderBoardSetting[] leaderBoards;

        /// <summary>
        /// リーダーボードを取得する
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public LeaderBoardSetting GetLeaderBoard(int index)
        {
            return leaderBoards[index];
        }
    }
}