using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtendString
{ 
	public static string Capitalize(this string str)
	{

		// seperate the string into individual words
		string[] words = str.Split( ' ' );

		// replace the first letter of each word with a capital
		for ( int i = 0; i < words.Length; i++ )
			if (string.IsNullOrWhiteSpace( words[i] ) )
			{
				continue;
			}
			else if ( words[ i ].Length == 1 )
			{
				words[ i ] = words[ i ].Substring( 0, 1 ).ToUpper();
			}
			else
			{
				words[ i ] = string.Concat( words[ i ].Substring( 0, 1 ).ToUpper(), words[ i ].Substring( 1 ) );
			}

		return string.Join(" ", words);

	}
}
