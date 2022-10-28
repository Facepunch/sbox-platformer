
using SandboxEditor;
using Sandbox;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using Sandbox.UI;

namespace Platformer;

[Library( "plat_sndbox", Description = "Sound Box" )]
[EditorSprite( "materials/editor/musicboxtweaker/musicboxtweaker.vmat" )]
[Display( Name = "Sound Box", GroupName = "Platformer", Description = "Platformer Soundscape" ), Category( "Sound" ), Icon( "speaker" )]
[HammerEntity]
[BoundsHelper( "mins", "maxs", true, false )]
partial class SndBox : ModelEntity
{
	/// <summary>
	/// Name of the sound to play.
	/// </summary>
	[Property( "soundName" ), FGDType( "sound" )]
	[Net] public string SoundName { get; set; }


	[Property( "mins", Title = "Tweaker mins" )]
	[Net]
	[DefaultValue( "-32 -32 -32" )]
	public Vector3 Mins { get; set; } = new Vector3( -32, -32, -32 );

	[Property( "maxs", Title = "Tweaker maxs" )]
	[Net]
	[DefaultValue( "32 32 32" )]
	public Vector3 Maxs { get; set; } = new Vector3( 32, 32, 32 );
	
	public Sound PlayingSound { get; protected set; }
	
	[Net]
	public BBox Inner { get; private set; }

	public Vector3 SndPos { get; private set; }

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

		Inner = new BBox( Position + Mins, Position + Maxs );
		DebugOverlay.Box( Inner, Color.Green );
	}

	[Event.Tick.Client]
	public void Tick()
	{
		if ( PlayingSound.Index <= 0 )
		{
			OnStartSound();
		}
	}
	
	[ClientRpc]
	protected void OnStartSound()
	{
		PlayingSound = Sound.FromWorld( SoundName, SndPos ).SetVolume( 1 );
	}

	[Event.Frame]
	public void OnFrame()
	{
		PlayingSound.SetPosition( SndPos );
		var pos = CurrentView.Position;
		if ( Local.Pawn.IsValid() )
		{
			pos = Local.Pawn.Position;
		}

		ShortestDistanceToSurface( Inner, pos );
	}

	private float ShortestDistanceToSurface( BBox bbox, Vector3 position )
	{
		var result = float.MaxValue;
		var point = Vector3.Zero;
		
		//This all could prob be simpliefied.
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
			
			DebugOverlay.Line( innerclosetsPoint, Inner.Center, 0f, false );

			DebugOverlay.Box( Inner, Color.Yellow );
		}
		SndPos = innerclosetsPoint;
		return result;
	}

}
