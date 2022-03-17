
using Sandbox;

namespace Platformer
{
	public class PlatformerOrbitCamera : CameraMode
	{

		private float distance;
		private float targetDistance = 250f;
		private Vector3 targetPosition;

		public float MinDistance => 120.0f;
		public float MaxDistance => 350.0f;
		public float DistanceStep => 60.0f;

		public override void Update()
		{
			var pawn = Local.Pawn as PlatformerPawn;

			if ( pawn == null ) return;

			UpdateViewBlockers( pawn );

			distance = distance.LerpTo( targetDistance, 5f * Time.Delta );
			targetPosition = Vector3.Lerp( targetPosition, pawn.Position, 8f * Time.Delta );

			var distanceA = distance.LerpInverse( MinDistance, MaxDistance );
			var height = 48f.LerpTo( 128f, distanceA );
			var center = targetPosition + Vector3.Up * height;
			var targetPos = center + Input.Rotation.Forward * -distance;

			var tr = Trace.Ray( center, targetPos )
				.Ignore( pawn )
				.Radius( 8 )
				.Run();

			var endpos = tr.EndPosition;

			Position = endpos;
			Rotation = Input.Rotation;
			Rotation *= Rotation.FromPitch( distanceA * 10f );

			var rot = pawn.Rotation.Angles() * .015f;
			rot.yaw = 0;

			Rotation *= Rotation.From( rot );

			var spd = pawn.Velocity.WithZ( 0 ).Length / 350f;
			var fov = 70f.LerpTo( 80f, spd );

			FieldOfView = FieldOfView.LerpTo( fov, Time.Delta );

			Viewer = null;
		}

		public override void Activated()
		{
			base.Activated();

			FieldOfView = 70;
			targetPosition = Local.Pawn.Position;
		}

		private void UpdateViewBlockers( PlatformerPawn pawn )
		{
			var traces = Trace.Sphere( 3f, CurrentView.Position, pawn.Position + Vector3.Up * 16 ).RunAll();

			if ( traces == null ) return;
		}

		public override void BuildInput( InputBuilder input )
		{
			base.BuildInput( input );

			if ( Input.MouseWheel != 0 )
			{
				targetDistance += -Input.MouseWheel * DistanceStep;
				targetDistance = targetDistance.Clamp( MinDistance, MaxDistance );
			}
		}

	}
}
