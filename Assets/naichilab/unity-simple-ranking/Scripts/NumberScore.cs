using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace naichilab
{
	public class NumberScore : IScore
	{
		private double score;
		private string format;


		public NumberScore (double score, string format = "")
		{
			this.score = score;			
			this.format = format;
		}

		public string TextForDisplay {
			get {
				if (!string.IsNullOrEmpty (this.format)) {
					return this.score.ToString (this.format);
				} else {
					return this.score.ToString ();
				}
			}
		}

		public string TextForSave { 
			get {
				return this.score.ToString ();
			}
		}

		public double Value {
			get {
				return this.score;
			}
		}
	}
}