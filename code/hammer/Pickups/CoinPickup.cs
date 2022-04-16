﻿
using Hammer;
using Sandbox;
using System.ComponentModel.DataAnnotations;

namespace Platformer;

[Library( "plat_coin", Description = "Coin Pickup" )]
[Model( Model = "models/citizen_props/coin01.vmdl" )]
[Display( Name = "Coin Pickup", GroupName = "Platformer", Description = "Coin Pickup." )]
internal partial class CoinPickup : BaseCollectible
{

	protected override bool OnCollected( PlatformerPawn pl )
	{
		base.OnCollected( pl );

		pl.Coin++;
		pl.PickedUpItem( Color.Yellow );

		pl.Client.AddInt( "kills" );

		return true;
	}

	protected override void OnCollectedEffect()
	{
		Sound.FromEntity( "life.pickup", this );
		Particles.Create( "particles/gameplay/player/coincollected/coincollected.vpcf", this );
	}

}