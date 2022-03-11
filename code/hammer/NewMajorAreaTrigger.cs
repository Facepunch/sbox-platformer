
using Platformer.UI;
using Sandbox;
using Sandbox.UI;

namespace Platformer
{
	/// <summary>
	/// A simple trigger volume that fires once and then removes itself.
	/// </summary>
	[Library( "plat_landmark" )]
	[Hammer.AutoApplyMaterial( "materials/editor/landmark/landmark.vmat" )]
	[Hammer.Solid]
	public partial class NewMajorAreaTrigger : TriggerOnce
	{

		[Property( "landmarkname", Title = "Land" )]
		public string LandMarkName { get; set; } = "";
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

		public override void OnTouchStart( Entity other )
		{
			base.OnTouchStart( other );

			if ( !other.IsServer ) return;
			if ( other is not PlatformerPawn pl ) return;

			if ( other != null )
			{
				NewAreaAlert( To.Single( other ), $"NEW AREA : {UpperCase} " );

			}

		}

	}
}
