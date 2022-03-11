
using Sandbox;

namespace Platformer;

[Library( "plat_jumppad" )]
[Hammer.EntityTool( "Jump Pad", "Platformer", "A pad that launches players toward a target entity" )]
[Hammer.AutoApplyMaterial( "materials/editor/jumppad/jumppad.vmat" )]
[Hammer.Line( "targetname", "targetentity" )]
public partial class Jumppad : BaseTrigger
{
	[Net, Property, FGDType( "target_destination" )] public string TargetEntity { get; set; } = "";
	[Net, Property] public float VerticalBoost { get; set; } = 200f;
	[Net, Property] public float Force { get; set; } = 1000f;

	public override void Spawn()
	{
		if ( Force == 0f )
		{
			Force = 1000f;
		}

		base.Spawn();
	}

	public override void Touch( Entity other )
	{
		if ( !other.IsServer ) return;
		if ( other is not PlatformerPawn pl ) return;
		var target = FindByName( TargetEntity );

		if ( target.IsValid() )
		{
			var direction = (target.Position - other.Position).Normal;
			pl.ApplyForce( new Vector3( 0f, 0f, VerticalBoost ) );
			pl.ApplyForce( direction * Force );

			DebugOverlay.Line( Position, target.Position, Color.Green, 10, false );
			DebugOverlay.Axis( Position, Rotation,20,10 );
			DebugOverlay.Axis( target.Position, target.Rotation, 20,10 );
		}

		base.Touch( other );
	}
}
