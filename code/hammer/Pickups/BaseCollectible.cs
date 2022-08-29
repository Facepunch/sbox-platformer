
using Sandbox;
using System.Collections.Generic;

namespace Platformer;

public partial class BaseCollectible : ModelEntity
{

	[Net]
	public IList<Entity> PlayersWhoCollected { get; set; }

	public override void Spawn()
	{
		base.Spawn();

		Transmit = TransmitType.Always;

		SetupPhysicsFromModel( PhysicsMotionType.Keyframed );
		Tags.Add( "trigger" );
		EnableAllCollisions = true;
		EnableSolidCollisions = false;
	}

	[Event.Frame]
	public void OnFrame()
	{
		OnFrameEvent();
	}

	public virtual void OnFrameEvent()
	{
		if ( !Local.Pawn.IsValid() ) return;

		var render = !PlayersWhoCollected.Contains( Local.Pawn );
		var alpha = render ? 1 : 0;

		RenderColor = RenderColor.WithAlpha( alpha );
	}

	public override void StartTouch( Entity other )
	{
		base.StartTouch( other );

		if ( !IsServer ) return;

		if ( other is not PlatformerPawn pl ) return;
		if ( PlayersWhoCollected.Contains( pl ) ) return;

		if( OnCollected( pl ) )
		{
			PlayersWhoCollected.Add( pl );
			OnCollectedEffect( To.Single( other.Client ) );
		}
	}

	public void Reset( Entity ent )
	{
		PlayersWhoCollected.Remove( ent );
	}

	protected virtual bool OnCollected( PlatformerPawn p ) { return true; }
	[ClientRpc]
	protected virtual void OnCollectedEffect() { }

}
