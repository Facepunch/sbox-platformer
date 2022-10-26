using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;

namespace Platformer.UI
{
	public partial class PlatformerChatBox : Panel
	{
		static PlatformerChatBox Current;

		public Panel Canvas { get; protected set; }
		public TextEntry Input { get; protected set; }
		public TextEntry InputHint { get; protected set; }

		public Label SendButton;

		public bool IsOpen
		{
			get => HasClass( "open" );
			set
			{
				SetClass( "open", value );
				if ( value )
				{
					Input.Focus();
					Input.Text = string.Empty;
					Input.Label.SetCaretPosition( 0 );
				}
			}
		}

		public PlatformerChatBox()
		{
			Current = this;

			StyleSheet.Load( "/ui/base/Chatbox/PlatformerChatBox.scss" );

			Canvas = Add.Panel( "chat_canvas" );
			Canvas.PreferScrollToBottom = true;


			Input = Add.TextEntry( "" );
			Input.AddEventListener( "onsubmit", () => Submit() );
			Input.AddEventListener( "onblur", () => Close() );
			Input.AcceptsFocus = true;
			Input.AllowEmojiReplace = true;

			SendButton = Input.Add.Label( "send", "sendbutton" );
		}

		public override void Tick()
		{
			if ( Sandbox.Input.Pressed( InputButton.Chat ) )
			{
				Open();
			}
			Input.Placeholder = string.IsNullOrEmpty( Input.Text ) ? "Enter your message..." : string.Empty;

			base.Tick();
		}

		protected override void OnClick( MousePanelEvent e )
		{
			base.OnClick( e );

			Submit();
		}

		void Open()
		{
			AddClass( "open" );
			Input.Focus();
			Canvas.TryScrollToBottom();
		}

		void Close()
		{
			RemoveClass( "open" );
			Input.Blur();
		}

		void Submit()
		{
			Close();

			var msg = Input.Text.Trim();
			Input.Text = "";

			if ( string.IsNullOrWhiteSpace( msg ) )
				return;

			Say( msg );
		}

		public void AddEntry( string name, string message, long playerId = 0, string lobbyState = null, bool isMessage = true )
		{
			var e = Canvas.AddChild<PlatformerChatEntry>();
			e.IsChatMessage = isMessage;

			var player = Local.Pawn;
			if ( player == null ) return;
			if ( Local.Pawn is PlatformerPawn pl )
			{
				if ( playerId > 0 )
					e.PlayerId = playerId;

				e.Message = message;
				e.Name = $"{name}";

				e.SetClass( "noname", string.IsNullOrEmpty( name ) );
				e.SetClass( "noavatar", playerId == 0 );
			}

			if ( Local.Pawn is PlatformerDeadPawn dpl )
			{
				e.Message = message;
				e.Name = $"{name}";

				if ( playerId > 0 )
					e.PlayerId = playerId;

				e.SetClass( "noname", string.IsNullOrEmpty( name ) );
				e.SetClass( "noavatar", playerId == 0 );
			}

			if ( lobbyState == "ready" || lobbyState == "staging" )
			{
				e.SetClass( "is-lobby", true );
			}

			Canvas.TryScrollToBottom();
		}

		[ConCmd.Client( "plat_chat_add", CanBeCalledFromServer = true )]
		public static void AddChatEntry( string name, string message, string playerId = "0", string lobbyState = null, bool isMessage = true )
		{
			Current?.AddEntry( name, message, long.Parse( playerId ), lobbyState, isMessage );

			// Only log clientside if we're not the listen server host
			if ( !Global.IsListenServer )
			{
				Log.Info( $"{name}: {message}" ); 
			}
		}

		public static void AddChatEntry( To target, string name, string message, long playerId = 0, string lobbyState = null, bool isMessage = true)
		{
			AddChatEntry( target, name, message, playerId.ToString(), lobbyState, isMessage);
		}

		[ConCmd.Admin( "plat_debug_chat_msg" )]
		public static void DebugMsg()
		{
			var cl = ConsoleSystem.Caller;

			PlatformerChatBox.AddChatEntry( To.Everyone, cl.Name, "has joined the game", cl.PlayerId, null, false );
		}

		[ConCmd.Admin( "plat_debug_chat_other" )]
		public static void DebugMsgOther()
		{
			PlatformerChatBox.AddChatEntry( To.Everyone, "Eagle One Development Team", "has joined the game", 76561197967441886, null, false );
			PlatformerChatBox.AddChatEntry( To.Everyone, "Eagle One Development Team", "what's up", 76561197967441886, null, true );
		}

		[ConCmd.Client( "plat_chat_addinfo", CanBeCalledFromServer = true )]
		public static void AddInformation( string message, long playerId = 0 )
		{
			Current?.AddEntry( null, message, playerId );
		}

		[ConCmd.Server( "plat_say" )]
		public static void Say( string message )
		{
			Assert.NotNull( ConsoleSystem.Caller );

			// todo - reject more stuff
			if ( message.Contains( '\n' ) || message.Contains( '\r' ) )
				return;

			Log.Info( $"{ConsoleSystem.Caller}: {message}" );
			AddChatEntry( To.Everyone, ConsoleSystem.Caller.Name, message, ConsoleSystem.Caller.PlayerId );
		}

	}
}
