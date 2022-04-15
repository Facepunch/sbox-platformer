
using Sandbox;
using System.ComponentModel.DataAnnotations;

namespace Platformer;

[Library("plat_prop_carriable")]
[Display( Name = "Prop Carriable", GroupName = "Platformer", Description = "A model the player can carry." )]
internal partial class PropCarriable : Prop, IUse
{
	public enum PropType
	{
		Wood,
		Cardboard
	}

	[Property( "model_properties", Title = "Break Type" ), Net]
	public PropType BreakType { get; set; } = PropType.Wood;

	public string SoundBreak = "break.wood";

	public string ParticleBreak = "particles/break/break.wood.vpcf";

	public override void Spawn()
	{
		base.Spawn();

		Transmit = TransmitType.Always;

		if ( BreakType == PropType.Wood )
		{
			SoundBreak = "break.wood";
			ParticleBreak = "particles/break/break.wood.vpcf";
		}

		if ( BreakType == PropType.Cardboard )
		{
			SoundBreak = "break.cardboard";
			ParticleBreak = "particles/break/break.cardboard.vpcf";
		}
	}

	public void Drop( Vector3 velocity )
	{
		if ( !Parent.IsValid() ) return;

		Velocity = velocity;
		EnableAllCollisions = true;

		SetParent( null );
	}

	public bool IsUsable( Entity user ) => !Parent.IsValid();

	public bool OnUse( Entity user )
	{
		if ( user is not PlatformerPawn p ) return false;
		if ( p.HeldBody.IsValid() ) return false;

		SetParent( p );

		p.HeldBody = this;
		EnableAllCollisions = false;

		LocalPosition = Vector3.Up * 30 + Vector3.Forward * Model.RenderBounds.Size.x * 1.1f;
		LocalRotation = Rotation.Identity;

		return true;
	}

	public override void OnKilled()
	{
		DeathEffect();
		base.OnKilled();
	}

	public void DeathEffect()
	{
		Platformer.PropCarryBreak( Position, ParticleBreak, SoundBreak );

	}


}
