using SandboxEditor;
using Sandbox;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Platformer;

[Library( "plat_tagspawn", Description = "Defines a checkpoint where the player will respawn after falling" )]
[Model( Model = "models/dev/playerstart_tint.vmdl" )]
[Display( Name = "Tag Player Checkpoint", GroupName = "Platformer", Description = "Defines a checkpoint where the player will respawn after falling" ), Category( "Player" ), Icon( "sports_handball" )]
[HammerEntity]
internal partial class TaggerSpawn : Entity
{
	public override void Spawn()
	{
		base.Spawn();

		Transmit = TransmitType.Always;

	}
}
