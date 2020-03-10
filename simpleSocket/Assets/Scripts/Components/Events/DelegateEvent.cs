using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class DelegateEvent<T> : ScriptableObject
{

	public delegate void delegateEvent( T parama );
	public event delegateEvent scriptableEvent;

	/// <summary>
	/// shortcut to invoke event
	/// </summary>
	/// <param name="value"></param>
	public void Invoke( T value )
	{
		scriptableEvent?.Invoke( value );
	}

}
