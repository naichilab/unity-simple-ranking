using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using System.Text;

namespace naichilab
{
    // <summary>
    /// string型の拡張メソッド
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// 空チェック
        /// </summary>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string self)
        {
            return string.IsNullOrEmpty(self);
        }

        /// <summary>
        /// ハッシュ化（HMACSHA256）
        /// 参考:http://hensa40.cutegirl.jp/archives/4066
        ///      環境的に SHA256CryptoServiceProvider が存在しないので HMACSHA256 を使用
        /// </summary>
        /// <returns></returns>
        public static string ToHMACSHA256(this string self)
        {            
            // パスワードをUTF-8エンコードでバイト配列として取り出す
            byte[] byteValues = System.Text.Encoding.UTF8.GetBytes(self);

            // HMACSHA256のハッシュ値を計算する
            HMACSHA256 crypto256 = new HMACSHA256(byteValues);
            byte[] hash256Value = crypto256.ComputeHash(byteValues);

            // HMACSHA256の計算結果をUTF8で文字列として取り出す
            StringBuilder hashedText = new StringBuilder();
            for (int i = 0; i < hash256Value.Length; i++) {
                // 16進の数値を文字列として取り出す
                hashedText.AppendFormat("{0:X2}", hash256Value[i]);
            }
            return hashedText.ToString();
        }
    }
}
