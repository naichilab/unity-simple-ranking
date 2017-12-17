using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace naichilab
{
	public enum ScoreOrder
	{
		/// <summary>
		/// 昇順(数値が小さい方がハイスコア)
		/// </summary>
		[Tooltip ("昇順(数値が小さい方がハイスコア)")]
		OrderByAscending,
		/// <summary>
		/// 降順(数値が大きい方がハイスコア)
		/// </summary>
		[Tooltip ("昇順(数値が小さい方がハイスコア)")]
		OrderByDescending
	}
}