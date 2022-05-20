
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

			Say( msg );
		}

		public void AddEntry( string name, string message, string avatar, string lobbyState = null )
		{
			var e = Canvas.AddChild<PlatformerChatEntry>();

			var player = Local.Pawn;
			if ( player == null ) return;

			if ( Local.Pawn is PlatformerPawn pl )

			{
				var CurrentA = pl.CurrentArea.ToUpper();

				e.Message.Text = message;
				e.NameLabel.Text = $"{name} : {CurrentA} :";
				e.Avatar.SetTexture( avatar );


				e.SetClass( "noname", string.IsNullOrEmpty( name ) );
				e.SetClass( "noavatar", string.IsNullOrEmpty( avatar ) );
			}

			if ( Local.Pawn is PlatformerDeadPawn dpl )

			{
				var CurrentA = dpl.CurrentArea.ToUpper();

				e.Message.Text = message;
				e.NameLabel.Text = $"{name} : {CurrentA} :";
				e.Avatar.SetTexture( avatar );


				e.SetClass( "noname", string.IsNullOrEmpty( name ) );
				e.SetClass( "noavatar", string.IsNullOrEmpty( avatar ) );
			}

			if ( lobbyState == "ready" || lobbyState == "staging" )
			{
				e.SetClass( "is-lobby", true );
			}
		}


		[ConCmd.Client( "plat_chat_add", CanBeCalledFromServer = true )]
		public static void AddChatEntry( string name, string message, string avatar = null, string lobbyState = null )
		{
			Current?.AddEntry( name, message, avatar, lobbyState );

			// Only log clientside if we're not the listen server host
			if ( !Global.IsListenServer )
			{
				Log.Info( $"{name}: {message}" ); 
			}
		}

		[ConCmd.Client( "plat_chat_addinfo", CanBeCalledFromServer = true )]
		public static void AddInformation( string message, string avatar = null )
		{
			Current?.AddEntry( null, message, avatar );
		}

		[ConCmd.Server( "plat_say" )]
		public static void Say( string message )
		{
			Assert.NotNull( ConsoleSystem.Caller );

			// todo - reject more stuff
			if ( message.Contains( '\n' ) || message.Contains( '\r' ) )
				return;

			Log.Info( $"{ConsoleSystem.Caller}: {message}" );
			AddChatEntry( To.Everyone, ConsoleSystem.Caller.Name, message, $"avatar:{ConsoleSystem.Caller.PlayerId}" );
		}

	}
}
