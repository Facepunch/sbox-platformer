
using SandboxEditor;
using Sandbox;
using System.ComponentModel.DataAnnotations;
using System;

namespace Platformer;

[Library( "plat_musicboxtweaker", Description = "Music Box Tweaker" )]
[EditorSprite( "materials/editor/musicboxtweaker/musicboxtweaker.vmat" )]
[Display( Name = "Music Box Tweaker", GroupName = "Platformer", Description = "Platformer Soundscape" ), Category( "Sound" ), Icon( "speaker" )]
[HammerEntity]
[BoundsHelper( "mins", "maxs", false, true )]
partial class MusicBoxTweaker : ModelEntity
{
	[Net, Property, FGDType( "target_destination" )] public string TargetMusicBox { get; set; } = "";
	[Net] public float Volume { get; set; } = 1;

	[Property( "mins", Title = "Tweaker mins" )]
	[Net]
	public Vector3 Mins { get; set; } = new Vector3( -32, -32, 0 );

	[Property( "maxs", Title = "Tweaker maxs" )]
	[Net]
	public Vector3 Maxs { get; set; } = new Vector3( 32, 32, 64 );

	private MusicBox MusicBox;
	private Vector3[] DirectionLut = new Vector3[]
	{
		Vector3.Left,
		Vector3.Right,
		Vector3.Forward,
		Vector3.Backward
	};

	public override void Spawn()
	{
		base.Spawn();

		Transmit = TransmitType.Always;
	}

	[Event.Frame]
	public void OnFrame()
	{
		MusicBox ??= FindByName( TargetMusicBox ) as MusicBox;
		if ( !MusicBox.IsValid() ) return;

		var pos = CurrentView.Position;
		if ( Local.Pawn.IsValid() )
		{
			pos = Local.Pawn.Position;
		}

		var bbox = new BBox( Position + Mins, Position + Maxs );
		var playerBbox = new BBox( pos - new Vector3( 8, 8, 0 ), pos + new Vector3( 8, 8, 64 ) );

		if ( !bbox.Overlaps( playerBbox ) )
			return;

		var dist = ShortestDistanceToSurface( bbox, pos ) - 8.0f;
		var vol = Math.Max( dist.LerpInverse( 0f, 64f ), 0.1f );

		if ( BasePlayerController.Debug )
		{
			DebugOverlay.Box( bbox, Color.Green );
			DebugOverlay.Text( vol.ToString(), bbox.Center, 0, 3000 );
		}

		MusicBox.UpdateVolume( vol );
	}

	private float ShortestDistanceToSurface( BBox bbox, Vector3 position )
	{
		var result = float.MaxValue;
		var point = Vector3.Zero;
		foreach ( var dir in DirectionLut )
		{
			var closetsPoint = bbox.ClosestPoint( position + dir * 10000 );
			var dist = Vector3.DistanceBetween( closetsPoint, position.WithZ( closetsPoint.z ) );
			if( dist < result )
			{
				point = closetsPoint;
				result = dist;
			}
		}

		if ( BasePlayerController.Debug )
		{
			DebugOverlay.Sphere( point, 3f, Color.Red, 0, false );
			DebugOverlay.Line( point, position, 0f, false );
		}

		return result;
	}

}
