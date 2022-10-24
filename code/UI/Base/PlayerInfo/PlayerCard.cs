using Sandbox;
using Sandbox.UI;

namespace Platformer.UI;

[UseTemplate]
public partial class PlayerCard : Panel
{
	// @ref
	public BlockBar HealthBar { get; set; }

	public string Lives { get; set; }
	public string Coins { get; set; }

	public override void Tick()
	{
		var pawn = Local.Pawn as PlatformerPawn;
		if ( !pawn.IsValid() ) return;

		HealthBar.MaxBlocks = pawn.MaxHealth.CeilToInt();
		HealthBar.CurrentBlocks = pawn.Health.CeilToInt();

		Lives = $"{pawn.NumberLife}";
		Coins = $"{pawn.Coin}";
	}
}
