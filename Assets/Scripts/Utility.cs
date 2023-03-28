using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
	public static bool IsEqual(float a, float b)
	{
		//1/256‚µ‚½’lˆÈ‰º‚ÍŒë·‚Æ‚µ‚ÄØ‚èÌ‚Ä
		return MathF.Abs(a - b) <= 0.004f;
	}

}
