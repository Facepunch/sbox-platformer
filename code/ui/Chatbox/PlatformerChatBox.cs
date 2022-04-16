
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

		public PlatformerChatBox()
		{
			Current = this;

			StyleSheet.Load( "/ui/Chatbox/PlatformerChatBox.scss" );

			Canvas = Add.Panel( "chat_canvas" );

			Input = Add.TextEntry( "" );
			Input.AddEventListener( "onsubmit", () => Submit() );
			Input.AddEventListener( "onblur", () => Close() );
			Input.AcceptsFocus = true;
			Input.AllowEmojiReplace = true;

			Sandbox.Hooks.Chat.OnOpenChat += Open;

		}

		void Open()
		{
			AddClass( "open" );
			Input.Focus();
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

			if ( Global.Lobby != null )
			{
				Log.Info( "Send Chat" );
				Global.Lobby?.SendChat( msg );
			}
			else
			{
				Say( msg );
			}
		}

		public void AddEntry( string name, string message, string avatar, string lobbyState = null )
		{
			var e = Canvas.AddChild<PlatformerChatEntry>();

			var player = Local.Pawn;
			if ( player == null ) return;

			if ( Local.Pawn is not PlatformerPawn pl ) return;

			var CurrentA = pl.CurrentArea.ToUpper();

			e.Message.Text = message;
			e.NameLabel.Text =  $"{name} : {CurrentA} :";
			e.Avatar.SetTexture( avatar );

			e.SetClass( "noname", string.IsNullOrEmpty( name ) );
			e.SetClass( "noavatar", string.IsNullOrEmpty( avatar ) );

			if ( lobbyState == "ready" || lobbyState == "staging" )
			{
				e.SetClass( "is-lobby", true );
			}
		}


		[ClientCmd( "plat_chat_add", CanBeCalledFromServer = true )]
		public static void AddChatEntry( string name, string message, string avatar = null, string lobbyState = null )
		{
			Current?.AddEntry( name, message, avatar, lobbyState );

			// Only log clientside if we're not the listen server host
			if ( !Global.IsListenServer )
			{
				Log.Info( $"{name}: {message}" ); 
			}
		}

		[ClientCmd( "plat_chat_addinfo", CanBeCalledFromServer = true )]
		public static void AddInformation( string message, string avatar = null )
		{
			Current?.AddEntry( null, message, avatar );
		}

		[ServerCmd( "plat_say" )]
		public static void Say( string message )
		{
			Assert.NotNull( ConsoleSystem.Caller );

			// todo - reject more stuff
			if ( message.Contains( '\n' ) || message.Contains( '\r' ) )
				return;

			Log.Info( $"{ConsoleSystem.Caller}: {message}" );
			AddChatEntry( To.Everyone, ConsoleSystem.Caller.Name, message, $"avatar:{ConsoleSystem.Caller.PlayerId}" );
		}

		[Event( "lobby.chat" )]
		public static void LobbyChat( Friend friend, string message )
		{
			if ( !Host.IsServer ) return;

			Log.Info( $"Lobby Chat: {message}" );
			AddChatEntry( To.Everyone, friend.Name, message, $"avatar:{friend.Id}", Global.Lobby?.GetMemberData( friend, "status" ) );
		}

	}
}

namespace Sandbox.Hooks
{
	public static partial class Chat
	{
		public static event Action OnOpenChat;

		[ClientCmd( "openchat" )]
		internal static void MessageMode()
		{
			OnOpenChat?.Invoke();
		}

	}
}
