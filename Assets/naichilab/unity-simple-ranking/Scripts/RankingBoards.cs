using System.Linq;
using UnityEngine;

namespace naichilab
{
    /// <summary>
    /// ランキング設定
    /// </summary>
    [CreateAssetMenu]
    public class RankingBoards : ScriptableObject
    {
        /// <summary>
        /// ランキング設定一覧
        /// </summary>
        [SerializeField] private RankingInfo[] items;

        /// <summary>
        /// ランキング情報取得
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RankingInfo GetRankingInfo(int id)
        {
            if (items == null || items.Length == 0) Debug.LogError("ランキング設定が１つも見つかりませんでした。");
            if (id < 0 || id >= items.Length)
                Debug.LogError(string.Format("存在しないランキングIDが指定されました。指定可能[{0}~{1}] 指定[{2}]", 0, items.Length - 1, id));
            return items[id];
        }

        public void CheckDuplicateClassName()
        {
            //複数ランキング使用時にCLass名重複していないかチェック
            var dupClass = items
                .GroupBy(r => r.ClassName)
                .Where(grp => grp.Count() > 1)
                .Select(grp => grp.Key)
                .FirstOrDefault();
            if (!string.IsNullOrEmpty(dupClass))
            {
                Debug.LogError(string.Format("いくつかのRanking設定でClassNameが重複しています。ClassName=[{0}]", dupClass));
            }
        }
    }
}