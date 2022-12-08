using Sandbox;
using System;

namespace Platformer
{
	public class PlatformerOrbitCamera : BaseCamera
	{

		private float distance;
		private float targetDistance = 250f;
		private Vector3 targetPosition;

		public float MinDistance => 120.0f;
		public float MaxDistance => 350.0f;
		public float DistanceStep => 60.0f;

		public Vector3 ViewNormal;
		public Vector3 LastPosition;

		public Rotation LastCameraRotation;

		public PlatformerOrbitCamera()
		{
			Camera.FieldOfView = 70;
		}

		public override void Update()
		{
			var pawn = Local.Pawn as PlatformerPawn;

			if ( pawn == null ) return;

			UpdateViewBlockers( pawn );
			var distanceA = distance.LerpInverse( MinDistance, MaxDistance );

			distance = distance.LerpTo( targetDistance, 5f * Time.Delta );
			targetPosition = Vector3.Lerp( targetPosition, pawn.Position, 8f * Time.Delta );

			var height = 48f.LerpTo( 96f, distanceA );
			var center = targetPosition + Vector3.Up * height;
			center += -pawn.ViewAngles.Forward * 8f;
			var targetPos = center + -pawn.ViewAngles.Forward * targetDistance;

			var tr = Trace.Ray( center, targetPos )
				.Ignore( pawn )
				.WithAnyTags( "world", "solid" )
				.WithoutTags("player" )
				.Radius( 8 )
				.Run();

			if ( tr.Hit )
			{
				distance = Math.Min( distance, tr.Distance );
			}

			var endpos = center + -pawn.ViewAngles.Forward * distance;

			Camera.Position = endpos;
			Camera.Rotation = pawn.ViewAngles.ToRotation();
			Camera.Rotation *= Rotation.FromPitch( distanceA * 10f );

			var rot = pawn.Rotation.Angles() * .015f;
			rot.yaw = 0;

			Camera.Rotation *= Rotation.From( rot );

			var spd = pawn.Velocity.WithZ( 0 ).Length / 350f;
			var fov = 70f.LerpTo( 80f, spd );

			Camera.FieldOfView = Camera.FieldOfView.LerpTo( fov, Time.Delta );
			Camera.ZNear = 6;
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
				targetDistance += -Input.MouseWheel * DistanceStep;
				targetDistance = targetDistance.Clamp( MinDistance, MaxDistance );
			}
		}

	}
}
