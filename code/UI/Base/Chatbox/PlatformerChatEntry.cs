
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Platformer.UI
{
	public partial class PlatformerChatEntry : Panel
	{
		// @ref
		public Label NameLabel { get; internal set; }
		// @ref
		public Label Message { get; internal set; }
		// @ref
		public Image Avatar { get; internal set; }

		public long PlayerId { get; protected set; }

		public RealTimeSince TimeSinceBorn = 0;

		public void SetPlayerId( long playerId )
		{
			PlayerId = playerId;
			Avatar.SetTexture( $"avatar:{playerId}" );

			SetClass( "not-me", Local.PlayerId != playerId );
		}

		public PlatformerChatEntry( bool isChat = false )
		{
			SetClass( "not-message", !isChat );

			// TODO - use razor, this is wank
			if ( isChat )
			{
				SetTemplate( "/UI/Base/Chatbox/PlatformerChatEntry.html" );
			}
			else
			{
				SetTemplate( "/UI/Base/Chatbox/PlatformerMessageEntry.html" );
			}
		}

		public override void Tick() 
		{
			base.Tick();

			SetClass( "faded", TimeSinceBorn > 60f );
		}
	}
}
