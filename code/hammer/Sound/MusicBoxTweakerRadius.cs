
using SandboxEditor;
using Sandbox;
using System.ComponentModel.DataAnnotations;
using System;
using System.Reflection;

namespace Platformer;

[Library( "plat_musicboxtweakerradius", Description = "Music Box Tweaker Radius" )]
[EditorSprite( "materials/editor/musicboxtweaker/musicboxtweaker.vmat" )]
[Display( Name = "Music Box Tweaker Radius", GroupName = "Platformer", Description = "Platformer Soundscape" ), Category( "Sound" ), Icon( "speaker" )]
[HammerEntity]
[Sphere( "radius" )]
partial class MusicBoxTweakerRadius : ModelEntity
{
	[Net, Property, FGDType( "target_destination" )] public string TargetMusicBox { get; set; } = "";
	[Net] public float Volume { get; set; } = 1;
	[Net, Property]
	public float Radius { get; set; } = 128.0f;

	private MusicBox MusicBox;

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

		var pos = CurrentView.Position + new Vector3( 0, 0, 48 );
		if ( Local.Pawn.IsValid() )
		{
			pos = Local.Pawn.Position + new Vector3( 0, 0, 48 );
		}

		var dist = Position.Distance( pos );
		if ( dist > Radius )
			return;
		var vol = 1 - (dist / Radius);

		MusicBox.UpdateVolume( vol );

		if ( BasePlayerController.Debug )
		{
			DebugOverlay.Text( vol.ToString(), Position );
			DebugOverlay.Line( Position, pos, 0f, false );
			DebugOverlay.Sphere( Position, Radius, Color.Red);
		}
	}

}
