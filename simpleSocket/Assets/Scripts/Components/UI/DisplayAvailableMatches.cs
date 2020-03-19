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
	private CurrentLobbyProtocol currentLobbyData;

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

		currentLobbyData = protocol as CurrentLobbyProtocol;

		print( "Display match..." + currentLobbyData.GetJson(out int _) ); ;


		BuildMatchSelect( );

	}

	private void BuildMatchSelect( )
	{

		for ( int i = 0; i < maxButtons; i++ )
		{
			
			if (i >= currentLobbyData.LobbyCount )
				SetButtonText( ( maxButtons - (i+1) ), "-" );
			else
				SetButtonText( ( maxButtons - (i+1) ), string.Format( "(Lobby-{0}) {1} [{2}/{3} slots available] ", currentLobbyData.lobby_ids[i],		 currentLobbyData.level_names[i],
																													currentLobbyData.current_players[i], currentLobbyData.max_players[i] )
																													);

		}

		

	}

	private void ButtonAction( int buttonId )
	{

		int matchId = ( maxButtons - 1 ) - buttonId;

		if ( matchId < currentLobbyData.LobbyCount )
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
		if ( selectedMatchId < 0 || selectedMatchId > currentLobbyData.LobbyCount )
		{
			Debug.LogError( "Nothing is selected" );
			return;
		}

		JoinLobbyProtocol joinGame = new JoinLobbyProtocol();
		joinGame.lobby_id = currentLobbyData.lobby_ids[ selectedMatchId ];

		SocketClient.ActiveGameData.SetGameStatus( GameData.GameStatus.Joining );
		SocketClient.ActiveSocket.QueueMessage( joinGame );


	}

	private void OnDestroy ()
	{

		HandleProtocol.Inst.Unbind( 'g', SetAvailableGames );
		SelectMatch -= ButtonAction;

	}

}
