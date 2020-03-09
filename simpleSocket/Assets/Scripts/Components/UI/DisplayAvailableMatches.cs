using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Protocol;

class DisplayAvailableMatches : MonoBehaviour
{

	[SerializeField] private RectTransform buttonHold;
	[SerializeField] private Button buttonPrefab;
	[SerializeField] private Rect buttonSize;   // and spacing
	[SerializeField] private int maxButtons = 4;

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
		
		// instancate buttons.
		for ( int i = 0; i < maxButtons; i++ )
		{
			Button butt = Instantiate<Button>( buttonPrefab, buttonHold );

			print( i + " :: "+ buttonHold.rect.height + " :: " + buttonPosition.position );

			butt.transform.localPosition = buttonPosition.position - new Vector2( 0, buttonSize.position.y);

			buttonPosition.position += new Vector2( 0, buttonSize.position.y + buttonSize.size.y );

			butt.onClick.AddListener( () => ButtonAction( i ) ); 

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
				SetButtonText( i, "-" );
			else
				SetButtonText( i, string.Format( " {0} [{1} slots available] ", matchNames[i], games.available_games) );

		}

		

	}

	private void ButtonAction( int buttonId )
	{

		if ( buttonId < matchNames.Length )
			selectedMatchId = maxButtons - buttonId;

	}

	private void SetButtonText( int buttonId, string text )
	{
		buttons[ buttonId ].GetComponentInChildren<TMPro.TextMeshProUGUI>().SetText( text );

	}

	private void OnDestroy ()
	{

		HandleProtocol.Inst.Unbind( 'g', SetAvailableGames );

	}

}
