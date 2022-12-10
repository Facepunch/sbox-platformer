
using Sandbox;

namespace Platformer
{
	public class PlatformerCamera : BaseCamera
	{

		private float distance = 250.0f;

		public float MinDistance => 100.0f;
		public float MaxDistance => 350.0f;
		public float DistanceStep => 60.0f;

		public PlatformerCamera()
		{
			Camera.FieldOfView = 70;
		}

		public override void Update()
		{
			var pawn = Game.LocalPawn as PlatformerPawn;

			if ( pawn == null ) return;

			UpdateViewBlockers( pawn );

			var center = pawn.Position + Vector3.Up * 76;
			//var distance = 150.0f * pawn.Scale;
			var targetPos = center + pawn.ViewAngles.Forward * -distance;

			var tr = Trace.Ray( center, targetPos )
				.Ignore( pawn )
				.Radius( 8 )
				.Run();

			var endpos = tr.EndPosition;


			Camera.Position = endpos;
			Camera.Rotation = pawn.ViewAngles.ToRotation();

			var rot = pawn.Rotation.Angles() * .015f;
			rot.yaw = 0;

			Camera.Rotation *= Rotation.From( rot );

			var spd = pawn.Velocity.WithZ( 0 ).Length / 350f;
			var fov = 70f.LerpTo( 80f, spd );

			Camera.FieldOfView = Camera.FieldOfView.LerpTo( fov, Time.Delta );

			Camera.FirstPersonViewer = null;
		}

		private void UpdateViewBlockers( PlatformerPawn pawn )
		{
			var traces = Trace.Sphere( 3f, Camera.Position, pawn.Position + Vector3.Up * 16 ).RunAll();

			if ( traces == null ) return;
		}

		[Event.Client.BuildInput]
		public void BuildInput()
		{
			if ( Input.MouseWheel != 0 )
			{
				distance = distance.LerpTo( distance - Input.MouseWheel * DistanceStep, Time.Delta * 10, true ).Clamp( MinDistance, MaxDistance );
			}
		}

	}
}
