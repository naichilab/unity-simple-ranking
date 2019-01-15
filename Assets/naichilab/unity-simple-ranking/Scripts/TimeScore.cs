using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace naichilab
{
    /// <summary>
    /// 時間型スコア
    /// </summary>
    public class TimeScore : IScore
    {
        private TimeSpan time;
        private string format;

        public TimeScore(TimeSpan time, string format = "")
        {
            this.time = time;
            this.format = format;
        }

        public ScoreType Type
        {
            get { return ScoreType.Time; }
        }

        public string TextForDisplay
        {
            get
            {
                if (!string.IsNullOrEmpty(format))
                {
                    return new DateTime(0).Add(time).ToString(format);
                }
                else
                {
                    return new DateTime(0).Add(time).ToString();
                }
            }
        }

        public string TextForSave
        {
            get { return time.Ticks.ToString(); }
        }

        public double Value
        {
            get { return time.Ticks; }
        }
    }
}