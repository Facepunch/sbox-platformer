
using Platformer.UI;
using Sandbox;

namespace Platformer.Gamemodes;

internal class Brawl : BaseGamemode
{

	public override PlatformerPawn CreatePlayerInstance( Client cl ) => new BrawlPlayer( cl );

	public override void Spawn()
	{
		base.Spawn();

		EnablePvP = true;
	}

	public override void ClientSpawn()
	{
		base.ClientSpawn();

		Local.Hud.AddChild<BrawlHud>();
	}

}
