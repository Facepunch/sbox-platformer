
using Sandbox;

namespace Platformer;

[Library("plat_prop_carriable")]
internal class PropCarriable : Prop, IUse
{
	public override void Spawn()
	{
		base.Spawn();

		Transmit = TransmitType.Always;

	}

	public void Throw()
	{
		if ( !Parent.IsValid() ) return;

		Velocity = Parent.Velocity + Parent.Rotation.Forward * 300 + Parent.Rotation.Up * 100;
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

		return true;
	}

	[Event.Tick]
	private void OnTick()
	{
		if ( !Parent.IsValid() ) return;
		if ( Parent is not PlatformerPawn p ) return;

		Position = p.Position + p.Rotation.Forward * 60;
		Position += Vector3.Up * 20;
		Rotation = Rotation.LookAt( p.Rotation.Forward );
	}

}
