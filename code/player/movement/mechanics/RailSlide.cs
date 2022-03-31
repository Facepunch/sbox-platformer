
using System.Linq;
using Sandbox;

namespace Platformer.Movement;

internal class RailSlide : BaseMoveMechanic
{

	public override bool TakesOverControl => true;

	private GenericPathEntity Path;
	private int Node;
	private float Alpha;
	private TimeSince TimeSinceJump;

	public RailSlide( PlatformerController controller ) : base( controller )
	{
	}

	protected override bool TryActivate()
	{
		if ( TimeSinceJump < .3f ) return false;

		foreach ( var path in Entity.All.OfType<GenericPathEntity>() )
		{
			if ( path.PathNodes.Count < 2 ) continue;

			if ( AttachToPath( path, ctrl.Position, out Node, out Alpha ) )
			{
				Path = path;
				ctrl.SetGroundEntity( path );
				return true;
			}
		}

		return false;
	}

	public override void PostSimulate()
	{
		base.PostSimulate();

		ctrl.GroundEntity = Path;

		Log.Info( ctrl.Pawn.IsServer + ":" + Path );
	}

	public override void Simulate()
	{
		base.Simulate();

		Alpha += Time.Delta;

		if( Alpha >= 1 )
		{
			Alpha = 0;
			Node++;

			if( Node >= Path.PathNodes.Count - 1 )
			{
				IsActive = false;
				Path = null;
				return;
			}
		}

		var node = Path.PathNodes[Node];
		var nextNode = Path.PathNodes[Node + 1];
		var currentPosition = ctrl.Position;
		var nextPosition = Path.GetPointBetweenNodes( node, nextNode, Alpha );

		ctrl.Velocity = (nextPosition - currentPosition).Normal * 300f;
		ctrl.Position = nextPosition;
		ctrl.Rotation = Rotation.LookAt( ctrl.Velocity.Normal );
		ctrl.GroundEntity = Path;
		ctrl.SetTag( "skidding" );
		
		if ( Input.Pressed( InputButton.Jump ) )
		{
			TimeSinceJump = 0;
			IsActive = false;

			// todo: add velocity up from rail normal,
			// and fix getting grounded immediately so we don't have to set position
			ctrl.Velocity = ctrl.Velocity.WithZ( 320f );
			ctrl.Position = ctrl.Position.WithZ( ctrl.Position.z + 10 );
		}
	}

	private bool AttachToPath( GenericPathEntity path, Vector3 position, out int nodeIdx, out float t )
	{
		// todo: NearestPointOnPath
		// Attach and slide along path in reverse

		nodeIdx = 0;
		t = 0;

		for( int i = 0; i < path.PathNodes.Count - 1; i++ )
		{
			var nodea = path.PathNodes[i];
			var nodeb = path.PathNodes[i + 1];
			var bestDist = float.MaxValue;
			var bestA = 0f;

			for( float j = 0; j <= 1; j += .1f )
			{
				var point = path.GetPointBetweenNodes( nodea, nodeb, j );
				var dist = position.Distance( point );
				if( dist < bestDist )
				{
					bestA = j;
					bestDist = dist;
				}
			}

			if ( bestA > .8f && i == path.PathNodes.Count - 2 ) continue;

			if( bestDist < 30 )
			{
				nodeIdx = i;
				t = bestA;

				return true;
			}
		}

		return false;
	}

}

