
using Sandbox;
using Sandbox.UI;

namespace Platformer.UI
{
	[Library]
	public partial class PlatformerHud : HudEntity<RootPanel>
	{

		public PlatformerHud()
		{
			if ( !IsClient )
				return;

			RootPanel.StyleSheet.Load( "/ui/PlatformerHud.scss" );
			RootPanel.StyleSheet.Load( "/ui/NewArea.scss" );

			RootPanel.AddChild<PlatformerScoreboard>();

			RootPanel.AddChild<PlatfotmerNameTags>();
			RootPanel.AddChild<HealthCurrent>();
			RootPanel.AddChild<LifeCurrent>();
			RootPanel.AddChild<CoinCurrent>();
			RootPanel.AddChild<KeysCollected>();
			RootPanel.AddChild<NewMajorArea>();
			RootPanel.AddChild<NewArea>();
			RootPanel.AddChild<PlatformerChatBox>();
			RootPanel.AddChild<PlatformerKillfeed>();
			RootPanel.AddChild<RoundTimer>();
			RootPanel.AddChild<EnergyCurrent>();

		}
	}
}
