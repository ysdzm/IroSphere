using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
	/// <summary>
	/// 2‚Â‚Ìfloat‚Ì·‚ª\•ª¬‚³‚¢‚©‚Ç‚¤‚©‚Ì”»’è
	/// </summary>
	/// <param name="a"></param>
	/// <param name="b"></param>
	/// <returns></returns>
	public static bool IsEqual(float a, float b)
	{
		//1/256‚µ‚½’lˆÈ‰º‚ÍŒë·‚Æ‚µ‚ÄØ‚èÌ‚Ä
		return MathF.Abs(a - b) <= 0.004f;
	}

}
