using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace naichilab
{
	public class TimeScore : IScore
	{
		private TimeSpan score;
		private string format;

		public TimeScore (TimeSpan time, string format = "")
		{
			this.score = time;
			this.format = format;
		}

		public string TextForDisplay {
			get {
				if (!string.IsNullOrEmpty (this.format)) {
					return new DateTime (0).Add (this.score).ToString (this.format);
				} else {
					return new DateTime (0).Add (this.score).ToString ();
				}
			}
		}

		public string TextForSave { 
			get {
				return this.score.Ticks.ToString ();
			}
		}

		public double Value {
			get {
				return this.score.Ticks;
			}
		}

	}
}