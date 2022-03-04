﻿using Hammer;
using Sandbox;
using Sandbox.Internal;
using System.Linq;
using System.Collections.Generic;

[Library( "plat_lifepickup", Description = "Addition Life" )]
[Model( Model = "models/editor/cordon_helper.vmdl" )]
[EntityTool( "Life Pickup", "Platformer", "Addition Life." )]
internal partial class LifePickup : ModelEntity
{

	[Net, Property]
	public int NumberOfLife { get; set; }

	private List<Entity> PlayerCollected { get; set; } = new List<Entity>();

	public override void Spawn()
	{
		base.Spawn();

		SetupPhysicsFromModel(PhysicsMotionType.Keyframed);
		CollisionGroup = CollisionGroup.Trigger;
		EnableSolidCollisions = false;
		EnableAllCollisions = true;

	}

	public override void ClientSpawn()
	{
		base.ClientSpawn();
	}

	public override void StartTouch( Entity other )
	{
		base.StartTouch( other );

		if ( other is not PlatformerPawn pl ) return;
		if ( PlayerCollected.Contains( pl ) ) return;
		pl.NumberLife ++;


		CollectedPickup(To.Single (other.Client) );
		PlayerCollected.Add( pl );

		//Delete();

	}

	[ClientRpc]
	private void CollectedPickup()
	{
		EnableDrawing = false;
		Particles.Create( "particles/explosion/barrel_explosion/explosion_gib.vpcf", this );

		Sound.FromEntity( "life.pickup", this );
	}

}
