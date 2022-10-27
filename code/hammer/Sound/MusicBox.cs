using SandboxEditor;
using Sandbox;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Platformer;

[Library( "plat_musicbox", Description = "Music Box" )]
[EditorSprite( "materials/editor/musicbox/musicbox.vmat" )]
[Display( Name = "Music Box", GroupName = "Platformer", Description = "Platformer Soundscape" ), Category( "Sound" ), Icon( "speaker" )]
[HammerEntity]
partial class MusicBox : Entity
{
	/// <summary>
	/// Name of the sound to play.
	/// </summary>
	[Property( "soundName" ), FGDType( "sound" )]
	[Net] public string SoundName { get; set; }

	/// <summary>
	/// Name of the sound to play.
	/// </summary>
	[Property( "bgsoundName" ), FGDType( "sound" )]
	[Net] public string BGSoundName { get; set; }

	[Net] public float Volume { get; set; }

	public Sound PlayingSound { get; protected set; }
	public Sound BGPlayingSound { get; protected set; }


	public override void Spawn()
	{
		base.Spawn();

		Transmit = TransmitType.Always;
	}

	public override void ClientSpawn()
	{
		//	OnStartSound();
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
		PlayingSound = Sound.FromScreen( SoundName ).SetVolume( .5f );
		BGPlayingSound = Sound.FromScreen( BGSoundName ).SetVolume( .1f );
	}

	[ClientRpc]
	public void UpdateVolume(float vol)
	{
		PlayingSound.SetVolume( 1 - vol );
		BGPlayingSound.SetVolume(  vol );
	}
}
