
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Platformer.UI
{
	[UseTemplate]
	public partial class PlatformerChatEntry : Panel
	{
		// @ref
		public Label NameLabel { get; internal set; }
		// @ref
		public Label Message { get; internal set; }
		// @ref
		public Image Avatar { get; internal set; }

		public RealTimeSince TimeSinceBorn = 0;

		public PlatformerChatEntry( bool isMessage = false )
		{
			SetClass( "is-message", isMessage );
		}

		public override void Tick() 
		{
			base.Tick();

			SetClass( "faded", TimeSinceBorn > 60f );
		}
	}
}
