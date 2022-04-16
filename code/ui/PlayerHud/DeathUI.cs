
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;

namespace Platformer.UI;

public class DeathUI : Panel
{
	public Image DeadImage;

	public DeathUI()
	{
		DeadImage = Add.Image( "ui/hud/death/respawn.png", "deathimage" );
	}

	public override void Tick()
	{
		var player = Local.Pawn;
		if ( player == null ) return;

		if ( Local.Pawn is not PlatformerPawn pl ) return;

		if ( pl.LifeState == LifeState.Dead )
		{
			Died();
		}
		else
		{
			Alive();
		}
	}

	public async void Died()
	{
		DeadImage.SetClass( "limbo", true );

		await Task.DelayRealtimeSeconds( 1.5f );

		DeadImage.SetClass( "alive", false );
		DeadImage.SetClass( "died",true);

	}
	public void Alive()
	{
		DeadImage.SetClass( "limbo", false );
		DeadImage.SetClass( "died", false );
		DeadImage.SetClass( "alive", true );
	}
}
