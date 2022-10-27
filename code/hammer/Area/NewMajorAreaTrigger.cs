
using Platformer.UI;
using Sandbox;
using SandboxEditor;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Platformer
{
	/// <summary>
	/// A simple trigger volume that fires once and then removes itself.
	/// </summary>
	[Library( "plat_landmark" )]
	[AutoApplyMaterial( "materials/editor/landmark/landmark.vmat" )]
	[Solid]
	[Display( Name = "Landmark", GroupName = "Platformer", Description = "Landmark" ), Category( "Triggers" ), Icon( "landscape" )]
	[HammerEntity]
	public partial class NewMajorAreaTrigger : TriggerOnce
	{

		[Property( "landmarkname", Title = "Land" )]
		public string LandMarkName { get; set; } = "";

		public string LandMarkMessage;
		public string UpperCase;

		public override void Spawn()
		{
			base.Spawn();

			UpperCase = LandMarkName.ToUpper();
			EnableTouchPersists = true;

		}

		[ClientRpc]
		public static void NewAreaAlert( string Title )
		{
			NewMajorArea.ShowLandmark( Title );
		}

		public static void NewAreaHud( string Location, int clint )
		{
			PlatformerKillfeed.AddEntryOnClient( To.Everyone, Location, clint );
		}	
	}
}
