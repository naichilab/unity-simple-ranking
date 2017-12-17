using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace naichilab
{
	public interface IScore
	{

		string TextForDisplay{ get; }

		string TextForSave{ get; }

		double Value{ get; }
	}
}