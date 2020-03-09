﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Protocol;

class DisplayAvailableMatches : MonoBehaviour
{

	public delegate void selectMatch ( int id );
	public event selectMatch SelectMatch;

	[SerializeField] private RectTransform buttonHold;
	[SerializeField] private Button buttonPrefab;
	[SerializeField] private Rect buttonSize;   // and spacing
	[SerializeField] private int maxButtons = 4;

	[SerializeField] private Color selectedColour = Color.gray;

	private List<Button> buttons;
	private string[] matchNames;

	private int selectedMatchId = -1;

	private void Awake ()
	{

		HandleProtocol.Inst.Bind( 'g', SetAvailableGames );		

	}

	private void Start ()
	{

		buttons = new List<Button>();

		Rect buttonPosition = buttonSize;
		SelectMatch += ButtonAction;
		// instancate buttons.
		for ( int i = 0; i < maxButtons; i++ )
		{
			Button butt = Instantiate<Button>( buttonPrefab, buttonHold );

			print( i + " :: "+ buttonHold.rect.height + " :: " + buttonPosition.position );

			butt.transform.localPosition = buttonPosition.position - new Vector2( 0, buttonSize.position.y);

			buttonPosition.position += new Vector2( 0, buttonSize.position.y + buttonSize.size.y );

			int j = i;
			butt.onClick.AddListener( () => SelectMatch?.Invoke( j ) );
	

			buttons.Add( butt );
			SetButtonText( i, "Match Slot " +  ( maxButtons - i) );

		}
	}

	private void SetAvailableGames( BaseProtocol protocol )
	{

		GameRequestProtocol gamesRequest = protocol as GameRequestProtocol;

		print( "Display match..." + gamesRequest.GetJson(out int _) ); ;

		BuildMatchSelect( gamesRequest );

	}

	private void BuildMatchSelect( GameRequestProtocol games )
	{

		matchNames = games.available_games;

		for ( int i = 0; i < maxButtons; i++ )
		{
			
			if (i >= matchNames.Length )
				SetButtonText( ( maxButtons - (i+1) ), "-" );
			else
				SetButtonText( ( maxButtons - (i+1) ), string.Format( " {0} [{1} slots available] ", matchNames[i], games.available_slots[i]) );

		}

		

	}

	private void ButtonAction( int buttonId )
	{

		print( "------------------------------------- " + buttonId );

		int matchId = ( maxButtons - 1 ) - buttonId;

		if ( matchId < matchNames.Length )
		{
			if ( selectedMatchId > -1 )
				SetButtonColour( (maxButtons - 1) - selectedMatchId, Color.white );

			selectedMatchId = matchId;

			SetButtonColour( buttonId, Color.grey );

		}

	}

	void SetButtonColour( int buttonId, Color buttonColour )
	{
		ColorBlock col = buttons[ buttonId ].colors;
		col.normalColor = buttonColour;
		col.pressedColor = buttonColour;
		col.highlightedColor = buttonColour;

		buttons[ buttonId ].colors = col;
	}

	private void SetButtonText( int buttonId, string text )
	{
		buttons[ buttonId ].GetComponentInChildren<TMPro.TextMeshProUGUI>().SetText( text );

	}

	private void OnDestroy ()
	{

		HandleProtocol.Inst.Unbind( 'g', SetAvailableGames );
		SelectMatch -= ButtonAction;

	}

}
