using Sandbox;
using Sandbox.UI;
using Platformer.Gamemodes;

namespace Platformer.UI;

[UseTemplate]
public partial class PlayerCard : Panel
{
	// @ref
	public BlockBar HealthBar { get; set; }
	// @ref
	public Panel CoinsPanel { get; set; }
	// @ref
	public Panel LivesPanel { get; set; }

	public string Lives { get; set; }
	public string Coins { get; set; }

	public override void Tick()
	{
		var pawn = Local.Pawn as PlatformerPawn;
		if ( !pawn.IsValid() ) return;

		HealthBar.MaxBlocks = pawn.MaxHealth.CeilToInt();
		HealthBar.CurrentBlocks = pawn.Health.CeilToInt();

		var wantCoinsAndLives = Tag.Current.IsValid() || Brawl.Current.IsValid();
		CoinsPanel.SetClass( "tag", wantCoinsAndLives );
		LivesPanel.SetClass( "tag", wantCoinsAndLives );

		Lives = $"{pawn.NumberLife}";
		Coins = $"{pawn.Coin}";
	}
}
