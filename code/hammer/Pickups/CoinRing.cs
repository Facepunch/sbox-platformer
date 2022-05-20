
using SandboxEditor;
using Sandbox;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Platformer;

[Library( "plat_coinring", Description = "Coin Ring" )]
//[Model( Model = "models/gameplay/collect/coin/coin01.vmdl" )]
[EditorModel( "models/gameplay/coinring/coinring.vmdl", FixedBounds = true )]
[Display( Name = "Coin Ring", GroupName = "Platformer", Description = "Coin Ring." ), Category( "Gameplay" ), Icon( "currency_bitcoin" )]
[HammerEntity]
internal partial class CoinRing : BaseCollectible
{
	private Particles CollectedPart;

	public override void Spawn()
	{
		base.Spawn();

		Transmit = TransmitType.Always;

		SetModel( "models/gameplay/coinring/coinring.vmdl" );

	}

	public override void OnFrameEvent()
	{
		var render = !PlayersWhoCollected.Contains( Local.Pawn );
		var alpha = render ? 1 : 0;

		RenderColor = RenderColor.WithAlpha( alpha );
	}


	[Event.Tick.Server]
	public void Tick()
	{
		Position = Position.WithZ( Position.z + MathF.Sin( Time.Now ) / 40 );
	}

	protected override bool OnCollected( PlatformerPawn pl )
	{
		base.OnCollected( pl );


		pl.Coin+= 5;
		pl.PickedUpItem( Color.Yellow );
		pl.Client.AddInt( "kills" );

		//Juice.Scale( 1, 0, 0 )
		//	.WithDuration( .3f )
		//	.WithEasing( EasingType.EaseOut )
		//	.WithTarget( this );

		return true;
	}


	protected override async void OnCollectedEffect()
	{
		await Task.DelayRealtimeSeconds( .1f );

		Sound.FromEntity( "life.pickup", this );
		CollectedPart = Particles.Create( "particles/gameplay/ring_coins/ring_coins.vpcf", this );
	}
}
