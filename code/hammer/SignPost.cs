using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

[Library( "plat_sign", Description = "Minigolf Sign Pole" )]
[Hammer.DrawAngles]
[Hammer.EditorSprite( "materials/editor/plat_sign/plat_sign.vmat" )]
[Display( Name = "Sign Post", GroupName = "Platformer", Description = "A sign post that displays a location." )]
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

		//WorldPanel.StyleSheet.Load( "/hammer/SignPost.scss" );

		// Bring it out the smallest amount from the sign
		WorldPanel.Transform = WorldPanel.Transform.WithPosition( WorldPanel.Transform.Position + WorldPanel.Transform.Rotation.Forward * 0.05f );

		WorldPanel.Add.Label( $"{ttext}", "top" );
		WorldPanel.Add.Label( "___________", "name" );
		WorldPanel.Add.Label( $"{btext}", "bottom" );

	}
	/// <summary>
	/// Stop s sound event
	/// </summary>
	[ClientRpc, Input]
	public void DisplayText()
	{
		WorldPanel.Style.Opacity = 1;
	}

	/// <summary>
	/// Stop the d event
	/// </summary>
	[ClientRpc,Input]
	public void HideText()
	{
		WorldPanel.Style.Opacity = 0;
	}
}
