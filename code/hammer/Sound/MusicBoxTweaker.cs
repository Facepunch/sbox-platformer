
using SandboxEditor;
using Sandbox;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace Platformer;

[Library( "plat_musicboxtweaker", Description = "Music Box Tweaker" )]
[EditorSprite( "materials/editor/musicboxtweaker/musicboxtweaker.vmat" )]
[Display( Name = "Music Box Tweaker", GroupName = "Platformer", Description = "Platformer Soundscape" ), Category( "Sound" ), Icon( "speaker" )]
[HammerEntity]
[BoundsHelper( "outermins", "outermaxs", false, true )]
[BoundsHelper( "innermins", "innermaxs", false, true )]
partial class MusicBoxTweaker : ModelEntity
{
	[Net, Property, FGDType( "target_destination" )] public string TargetMusicBox { get; set; } = "";
	[Net] public float Volume { get; set; } = 1;

	[Property( "outermins", Title = "Tweaker Outer Mins" )]
	[Net]
	[DefaultValue( "-64 -64 -64" )]
	public Vector3 Mins { get; set; } = new Vector3( -64, -64, -64 );

	[Property( "outermaxs", Title = "Tweaker Outer Maxs" )]
	[Net]
	[DefaultValue( "64 64 64" )]
	public Vector3 Maxs { get; set; } = new Vector3( 64, 64, 64 );

	[Property( "innermins", Title = "Tweaker Inner Mins" )]
	[Net]
	[DefaultValue( "-32 -32 -32" )]
	public Vector3 InMins { get; set; } = new Vector3( -32, -32, -32 );

	[Property( "innermaxs", Title = "Tweaker Inner Maxs" )]
	[Net]
	[DefaultValue( "32 32 32" )]
	public Vector3 InMaxs { get; set; } = new Vector3( 32, 32, 32 );

	[Net]
	public BBox Outer { get; private set; }

	[Net]
	public BBox Inner { get; private set; }
	
	private MusicBox MusicBox;
	private Vector3[] DirectionLut = new Vector3[]
	{
		Vector3.Up,
		Vector3.Down,
		Vector3.Left,
		Vector3.Right,
		Vector3.Forward,
		Vector3.Backward
	};

	public override void Spawn()
	{
		base.Spawn();

		Transmit = TransmitType.Always;

		Outer = new BBox( Position + Mins, Position + Maxs );
		Inner = new BBox( Position + InMins, Position + InMaxs );


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

		var bbox = Outer;
		var playerBbox = new BBox( pos - new Vector3( 8, 8, 0 ), pos + new Vector3( 8, 8, 64 ) );

		if ( !bbox.Overlaps( playerBbox ) )
			return;

		var dist = ShortestDistanceToSurface( bbox, pos );
		var vol = dist.Clamp( 0, 1 );

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
			var outerclosetsPoint = bbox.ClosestPoint( position + new Vector3( 0, 0, 48 ) + dir * 10000 );
			var dist2 = Vector3.DistanceBetween( outerclosetsPoint, position + new Vector3( 0, 0, 48 ) );
			if ( dist2 < result )
			{
				point = outerclosetsPoint;
				result = dist2;
			}
		}

		var innerclosetsPoint = Inner.ClosestPoint( position + new Vector3( 0, 0, 48 ) );
		var outerclosetsPoint1 = Outer.ClosestPoint( position + new Vector3( 0, 0, 48 ) );
		var maxdist = Vector3.DistanceBetween( innerclosetsPoint, point );
		var dist = result / maxdist;
		if ( dist < result )
		{
			result = dist;
		}
		
		if ( BasePlayerController.Debug )
		{
			DebugOverlay.Text( result.ToString(), bbox.Center, 0, 3000 );
			DebugOverlay.Sphere( Inner.Center, 3f, Color.Red, 0, false );
			DebugOverlay.Sphere( innerclosetsPoint, 3f, Color.Green, 0, false );
			DebugOverlay.Sphere( outerclosetsPoint1, 3f, Color.Blue, 0, false );
			DebugOverlay.Sphere( point, 3f, Color.Cyan, 0, false );
			
			DebugOverlay.Line( innerclosetsPoint, Inner.Center, 0f, false );
			DebugOverlay.Line( outerclosetsPoint1, innerclosetsPoint, 0f, false );
			DebugOverlay.Line( outerclosetsPoint1, point, 0f, false );

			DebugOverlay.Box( Outer, Color.Green );
			DebugOverlay.Box( Inner, Color.Yellow );
		}

		return result;
	}

}
