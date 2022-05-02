using Platformer.UI;
using Sandbox;
using Sandbox.UI;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Platformer
{
	/// <summary>
	/// A simple trigger volume that fires once and then removes itself.
	/// </summary>
	[Library( "plat_landmark" )]
	[Hammer.AutoApplyMaterial( "materials/editor/landmark/landmark.vmat" )]
	[Hammer.Solid]
	[Display( Name = "Landmark", GroupName = "Platformer", Description = "Landmark" ), Category( "Triggers" ), Icon( "landscape" )]
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

		public override void OnTouchStart( Entity other )
		{
			base.OnTouchStart( other );

			if ( !other.IsServer ) return;
			if ( other is not PlatformerPawn pl ) return;

			if ( other != null )
			{
				NewAreaHud($"{pl.Client.Name} has entered {LandMarkName}",pl.Client.NetworkIdent );
				NewAreaAlert( To.Single( other ), $"NEW AREA : {LandMarkName} " );
			}

		}		
	}
}
