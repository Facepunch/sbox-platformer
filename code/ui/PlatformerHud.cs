
using Sandbox;
using Sandbox.UI;

namespace Platformer.UI
{
	[Library]
	public partial class PlatformerHud : HudEntity<RootPanel>
	{
		public static PlatformerHud Current;

		public PlatformerHud()
		{
			if ( !IsClient )
				return;

			Current = this;

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
			RootPanel.AddChild<CourseTimer>();

		}

		public void Tick()
		{

			RootPanel.SetClass( "game-end", Platformer.CurrentState == Platformer.GameStates.GameEnd );
			RootPanel.SetClass( "game-warmup", Platformer.CurrentState == Platformer.GameStates.Warmup );
		}
	}
}
