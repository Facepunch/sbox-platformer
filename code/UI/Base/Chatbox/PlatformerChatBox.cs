using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

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

			SendButton = Add.Label("send","sendbutton" );

			Input = Add.TextEntry( "" );
			Input.AddEventListener( "onsubmit", () => Submit() );
			Input.AddEventListener( "onblur", () => Close() );
			Input.AcceptsFocus = true;
			Input.AllowEmojiReplace = true;

		}

		public override void Tick()
		{
			SendButton.Focus();
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

		public void AddEntry( string name, string message, string avatar, string lobbyState = null, bool isMessage = true )
		{
			var e = new PlatformerChatEntry( isMessage );
			Canvas.AddChild( e );

			var player = Local.Pawn;
			if ( player == null ) return;

			if ( Local.Pawn is PlatformerPawn pl )
			{
				var CurrentA = pl.CurrentArea.ToUpper();

				e.Message.Text = message;
				e.NameLabel.Text = $"{name}";
				e.Avatar.SetTexture( avatar );

				e.SetClass( "noname", string.IsNullOrEmpty( name ) );
				e.SetClass( "noavatar", string.IsNullOrEmpty( avatar ) );
			}

			if ( Local.Pawn is PlatformerDeadPawn dpl )
			{
				var CurrentA = dpl.CurrentArea.ToUpper();

				e.Message.Text = message;
				e.NameLabel.Text = $"{name}";
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
