using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helpers 
{

	public static string GetTime( int seconds )
	{
		System.TimeSpan time = new System.TimeSpan(0, 0, seconds);

		return time.ToString();

	}

}
