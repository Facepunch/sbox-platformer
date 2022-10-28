
using Platformer.Utility;
using Sandbox;
using System.Linq;
using System.Numerics;

namespace Platformer.Gamemodes;

internal partial class TagPlayer : PlatformerPawn
{

	private Particles TagArrowParticle;

	public bool Tagged => Tag.Current?.Tagged.Contains( this ) ?? false;
	private Vector3 TagMins => new( -64, -64, 0 );
	private Vector3 TagMaxs => new( 64, 64, 64 );

	public TagPlayer( Client cl ) : base( cl ) { }
	public TagPlayer() : base() { }

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		if ( Tagged )
		{
			TagNearbyPlayers();
			cl.SetInt( "tagged", 1 );
		}
	}

	private void TagNearbyPlayers()
	{
		var bbox = new BBox( Position + TagMins, Position + TagMaxs );
		var nearby = FindInBox( bbox );

		foreach(var ent in nearby )
		{
			if ( ent == this || ent is not TagPlayer pl ) continue;
			if ( pl.Tagged ) continue;

			Tag.Current.TagPlayer( pl );
			Client.AddInt( "kills" );
		}
	}

	[Event.Frame]
	private void EnsureTagParticle()
	{
		var create = Tagged && TagArrowParticle == null;
		var destroy = !Tagged && TagArrowParticle != null;
		var color = Tagged ? Color.Red : Color.White;

		if ( create )
		{
			TagArrowParticle = Particles.Create( "particles/gameplay/player/tag_arrow/tag_arrow.vpcf", this );
			TagArrowParticle.SetPosition( 6, Color.Red * 255 );
		}

		if ( destroy )
		{
			TagArrowParticle.Destroy();
			TagArrowParticle = null;
		}

		this.SetRenderColorRecursive( color );
	}

}
