
using Sandbox.UI;
using Platformer.Gamemodes;
using Sandbox;

namespace Platformer.UI;

[UseTemplate]
public class Hud : RootPanel
{

	public Hud()
	{
		Game.RootPanel = this;
	}

	public override void Tick()
	{
		SetClass( "game-end", Platformer.GameState == GameStates.GameEnd );
		SetClass( "game-warmup", Platformer.GameState == GameStates.Warmup );
	}

}
