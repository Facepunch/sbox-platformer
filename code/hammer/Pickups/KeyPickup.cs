
using SandboxEditor;
using Sandbox;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Platformer.Gamemodes;

namespace Platformer;

[Library( "plat_key", Description = "Key Pickup" )]
[EditorModel( "models/editor/collectables/collectables.vmdl", FixedBounds = true )]
[Display( Name = "Key Pickup", GroupName = "Platformer", Description = "Key Pickup" ), Category( "Gameplay" ), Icon( "vpn_key" )]
[HammerEntity]
internal partial class KeyPickup : AnimatedEntity
{
	public enum ModelType
	{
		FoamFinger,
		Ball,
		IceCream
	}

	/// <summary>
	/// This will set the model and icon for the HUD.
	/// </summary>
	[Property( "model_type", Title = "Model Type" ), Net]
	public ModelType ModelTypeList { get; set; } = ModelType.FoamFinger;

	[Net]
	public string KeyIcon { get; set; }

	[Property]
	[Net]
	public int KeyNumber { get; set; } = 1;

	public override void Spawn()
	{
		base.Spawn();

		if ( ModelTypeList == ModelType.FoamFinger )
		{
			SetModel( "models/citizen_props/foamhand.vmdl" );
			KeyIcon = ("pan_tool_alt");
		}

		if ( ModelTypeList == ModelType.Ball )
		{
			SetModel( "models/citizen_props/beachball.vmdl" );
			KeyIcon = ("sports_basketball");
		}
		
		if ( ModelTypeList == ModelType.IceCream )
		{
			SetModel( "models/citizen_props/icecreamcone01.vmdl" );
			KeyIcon = ("icecream");
			Scale = 1.5f;
		}

		SetupPhysicsFromModel( PhysicsMotionType.Keyframed );
		Tags.Add( "trigger" );
		EnableSolidCollisions = false;
		EnableAllCollisions = true;
		Transmit = TransmitType.Always;
	}

	public override void StartTouch( Entity other )
	{
		base.StartTouch( other );

		if ( other is not CompetitivePlayer pl ) 
			return;

		if ( pl.KeysPlayerHas.Contains( KeyNumber ) ) 
			return;

		pl.PickedUpItem( Color.Yellow );
		pl.KeysPlayerHas.Add( KeyNumber );
		pl.NumberOfKeys++;

		CollectedHealthPickup( To.Single( other.Client ) );

		if ( Coop.Current.IsValid() )
		{
			if ( Coop.Current.KeysAllPlayerHas.Contains( KeyNumber ) ) 
				return;

			Coop.Current.KeysAllPlayerHas.Add( KeyNumber );
			Coop.Current.NumberOfKeys++;

			CollectedHealthPickup( To.Single( other.Client ) );
		}
	}

	[ClientRpc]
	private void CollectedHealthPickup()
	{
		Sound.FromEntity( "life.pickup", this );
		Particles.Create( "particles/gameplay/player/collectpickup/collectpickup.vpcf", this );

	}

	[Event.Tick.Server]
	public void Tick()
	{
		Rotation = Rotation.FromYaw( Rotation.Yaw() + 500 * Time.Delta );
	}

	[Event.Tick.Client]
	private void ClientTick()
	{
		if ( Platformer.Mode == Platformer.GameModes.Competitive )
		{
			var a = ShouldRender() ? 1 : 0;
			RenderColor = RenderColor.WithAlpha( a );
		}
		if ( Platformer.Mode == Platformer.GameModes.Coop )
		{
			var b = ShouldRenderAll() ? 1 : 0;
			RenderColor = RenderColor.WithAlpha( b );
		}
	}

	private bool ShouldRender()
	{
		if ( !Local.Pawn.IsValid() ) return true;
		if ( Local.Pawn is not CompetitivePlayer pl ) return true;

		return !pl.KeysPlayerHas.Contains( KeyNumber );
	}

	private bool ShouldRenderAll()
	{
		return !Coop.Current?.KeysAllPlayerHas?.Contains( KeyNumber ) ?? false;
	}
}
