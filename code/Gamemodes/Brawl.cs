
using Platformer.UI;
using Sandbox;

namespace Platformer.Gamemodes;

internal class Brawl : BaseGamemode
{
	public static Brawl Current => Instance as Brawl;

	public override PlatformerPawn CreatePlayerInstance( IClient cl ) => new BrawlPlayer( cl );

	public override void Spawn()
	{
		base.Spawn();

		EnablePvP = true;
	}

	public override void ClientSpawn()
	{
		base.ClientSpawn();

		Game.RootPanel.AddChild<BrawlHud>();
	}

}
