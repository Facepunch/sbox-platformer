
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Linq;

namespace Platformer.UI
{
	public class TagHud : Panel
	{

		public Label Text;
		public Image image;
		public Label State;
		public Panel BgNot;

		public Label TagText;
		public Image Tagimage;
		public Label TagState;
		public Panel TagBgNot;

		public TagHud()
		{
			BgNot = Add.Panel( "not-tagged" );
			Text = Add.Label( string.Empty, "not-tagged-amount" );
			image = Add.Image( "ui/hud/tag/not-tagged.png", "not-tagged-icon" );
			State = Add.Label( string.Empty, "not-tagged-text" );

			TagBgNot = Add.Panel( "tagged" );
			TagText = Add.Label( string.Empty, "tagged-amount" );
			Tagimage = Add.Image( "ui/hud/tag/tagged.png", "tagged-icon" );
			TagState = Add.Label( string.Empty, "tagged-text" );
		}

		public override void Tick()
		{
			base.Tick();

			SetClass( "active", Platformer.CurrentGameMode == Platformer.GameModes.Tag );

			Text.Text = Entity.All.OfType<PlatformerPawn>().Count( e => !e.Tagged ).ToString();
			TagText.Text = Entity.All.OfType<PlatformerPawn>().Count( e => e.Tagged ).ToString();
			State.Text = "Not Tagged";
			TagState.Text = "Tagged";
		}
	}
}
