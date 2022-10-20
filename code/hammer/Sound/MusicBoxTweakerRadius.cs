
using SandboxEditor;
using Sandbox;
using System.ComponentModel.DataAnnotations;
using System;

namespace Platformer;

[Library( "plat_musicboxtweakerradius", Description = "Music Box Tweaker Radius" )]
[EditorSprite( "editor/ent_logic.vmat" )]
[Display( Name = "Music Box Tweaker", GroupName = "Platformer", Description = "Platformer Soundscape" ), Category( "Sound" ), Icon( "speaker" )]
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

		var pos = CurrentView.Position;
		if ( Local.Pawn.IsValid() )
		{
			pos = Local.Pawn.Position;
		}

		var dist = Position.Distance( pos );
		if ( dist > Radius )
			return;

		var vol = (Radius - dist).LerpInverse( 0, 64f );
		vol = Math.Max( 0.1f, vol );

		MusicBox.UpdateVolume( vol );

		if ( BasePlayerController.Debug )
		{
			DebugOverlay.Text( vol.ToString(), Position );
		}
	}

}
