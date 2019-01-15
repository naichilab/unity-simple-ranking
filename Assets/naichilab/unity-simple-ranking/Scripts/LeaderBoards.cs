using System;
using UnityEngine;

namespace naichilab
{
    [CreateAssetMenu]
    public class LeaderBoards : ScriptableObject
    {
        [SerializeField] private LeaderBoardSetting[] _leaderBoards;

        public LeaderBoardSetting GetLeaderBoard(int index)
        {
            return _leaderBoards[index];
        }
    }
}