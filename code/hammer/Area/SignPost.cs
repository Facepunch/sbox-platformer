using System;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Sandbox;
using SandboxEditor;
using Sandbox.UI.Construct;

[Library( "plat_sign", Description = "Minigolf Sign Pole" )]
[DrawAngles]
[EditorSprite( "materials/editor/plat_sign/plat_sign.vmat" )]
[Display( Name = "Sign Post", GroupName = "Platformer", Description = "A sign post that displays a location." ), Category( "Gameplay" ), Icon( "signpost" )]
[HammerEntity]
public partial class SignPost : Entity
{

	WorldSignPanel WorldPanel;

	[Net]
	[Property( "Top Text", Title = "Top Text" )]
	public string TopText { get; set; }

	[Net]
	[Property( "Bottom Text", Title = "Bottom Text" )]
	public string BottomText { get; set; }

	public override void Spawn()
	{
		base.Spawn();

		Transmit = TransmitType.Always;

	}

	public override void ClientSpawn()
	{
		base.ClientSpawn();

		WorldPanel = new();
		WorldPanel.Transform = Transform;

		WorldPanel.Style.Opacity = 0;

		var ttext = TopText;
		var btext = BottomText;

		// Bring it out the smallest amount from the sign
		WorldPanel.Transform = WorldPanel.Transform.WithPosition( WorldPanel.Transform.Position + WorldPanel.Transform.Rotation.Forward * 0.05f );

		WorldPanel.Add.Label( $"{ttext}", "top" );
		WorldPanel.Add.Label( "___________", "name" );
		WorldPanel.Add.Label( $"{btext}", "bottom" );

	}


	[ClientRpc, Input]
	public void DisplayText()
	{
		WorldPanel.Style.Opacity = 1;
	}

	[ClientRpc,Input]
	public void HideText()
	{
		WorldPanel.Style.Opacity = 0;
	}
}
