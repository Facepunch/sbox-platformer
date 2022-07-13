
using Sandbox;
using SandboxEditor;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Platformer;

[Model]
[Library( "plat_jumppad_model" )]
[Display( Name = "Model Jump Pad", GroupName = "Platformer", Description = "A pad that launches players toward a target entity" ), Category( "Gameplay" ), Icon( "sports_gymnastics" )]
[Line( "targetname", "targetentity" )]
[HammerEntity]
public partial class Jumppadmodel : AnimatedEntity
{
	[Net, Property, FGDType( "target_destination" )] public string TargetEntity { get; set; } = "";
	[Net, Property] public float VerticalBoost { get; set; } = 200f;
	[Net, Property] public float Force { get; set; } = 1000f;

	[Property( "Fling_Sound", Title = "Fling_Sound" ), FGDType( "sound" ), Category( "Sounds" )]
	public string FlingSound { get; set; } = "";

	public override void Spawn()
	{
		SetupPhysicsFromModel( PhysicsMotionType.Static );
		Tags.Add( "trigger" );
		//SetInteractsExclude( CollisionLayer.STATIC_LEVEL | CollisionLayer.WORLD_GEOMETRY | CollisionLayer.PLAYER_CLIP );
		EnableAllCollisions = false;
		EnableTouch = true;

		if ( Force == 0f )
		{
			Force = 1000f;
		}

		base.Spawn();
	}

	public override void StartTouch( Entity other )
	{
		if ( other is not PlatformerPawn pl ) return;
		var target = FindByName( TargetEntity );

		if ( target.IsValid() )
		{
			SetAnimParameter( "Fling", true );
			PlaySound(FlingSound);
			var direction = (target.Position - other.Position).Normal;
			pl.ApplyForce( new Vector3( 0f, 0f, VerticalBoost ) );
			pl.ApplyForce( direction * Force );
		}

		base.StartTouch( other );
	}
}
