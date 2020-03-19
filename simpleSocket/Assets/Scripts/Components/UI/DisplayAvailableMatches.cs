using System.Collections;
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

		CurrentLobbyProtocol gamesRequest = protocol as CurrentLobbyProtocol;

		print( "Display match..." + gamesRequest.GetJson(out int _) ); ;

		BuildMatchSelect( gamesRequest );

	}

	private void BuildMatchSelect( CurrentLobbyProtocol games )
	{

		for ( int i = 0; i < maxButtons; i++ )
		{
			
			if (i >= games.level_names.Length )
				SetButtonText( ( maxButtons - (i+1) ), "-" );
			else
				SetButtonText( ( maxButtons - (i+1) ), string.Format( "(Lobby-{0}) {1} [{2}/{3} slots available] ", games.lobby_ids[i], games.level_names[i], games.current_players[i], games.max_players[i]) );

		}

		

	}

	private void ButtonAction( int buttonId )
	{

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

	public void JoinSelectedGame()
	{
		if ( selectedMatchId < 0 || selectedMatchId > matchNames.Length )
		{
			Debug.LogError( "Nothing is selected" );
			return;
		}

		JoinLobbyProtocol joinGame = new JoinLobbyProtocol();
		joinGame.match_name = matchNames[ selectedMatchId ];

		SocketClient.ActiveGameData.SetGameStatus( GameData.GameStatus.Joining );
		SocketClient.ActiveSocket.QueueMessage( joinGame );

		print( "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<htygfffdgfhgfhdf" );

	}

	private void OnDestroy ()
	{

		HandleProtocol.Inst.Unbind( 'g', SetAvailableGames );
		SelectMatch -= ButtonAction;

	}

}
