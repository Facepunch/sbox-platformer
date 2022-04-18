using Sandbox;
using System;

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

		public Vector3 LastPosition;
		public override void Update()
		{
			var pawn = Local.Pawn as PlatformerPawn;

			if ( pawn == null ) return;

			UpdateViewBlockers( pawn );

			var distanceA = distance.LerpInverse( MinDistance, MaxDistance );

			distance = distance.LerpTo( targetDistance, 10f * Time.Delta );
			CalculateTargetPosition();

			var height = 48f.LerpTo( 96f, distanceA );
			var center = targetPosition + Vector3.Up * height;
			center += Input.Rotation.Backward * 8f;
			var cameraTargetPos = center + Input.Rotation.Backward * targetDistance;

			var tr = Trace.Ray( center, cameraTargetPos )
				.Ignore( pawn )
				.Radius( 8 )
				.Run();

			if ( tr.Hit )
			{
				distance = Math.Min( distance, tr.Distance );
			}

			var endpos = center + Input.Rotation.Backward * distance;

			Position = endpos;
			Rotation = Input.Rotation;

			var fov = 80.0f;

			FieldOfView = FieldOfView.LerpTo( fov, Time.Delta );
			ZNear = 6;
			Viewer = null;

			LastPosition = Position;
		}

		public void CalculateTargetPosition()
		{
			// How far can our target can be from the player
			// Jump height works well enough for this
			float maxDistance = 40.0f;

			targetPosition += Local.Pawn.Velocity.WithZ( 0 ) * Time.Delta * 1.1f;

			// Fixme:
			// If player is rotating the camera manually, we should
			// Focus on center again

			// Clamp distance
			if ( Local.Pawn.Position.Distance( targetPosition ) > maxDistance )
			{
				Vector3 distanceNormal = (targetPosition - Local.Pawn.Position).Normal;
				targetPosition = Local.Pawn.Position + ( distanceNormal * maxDistance );
			}

		}

		public override void Activated()
		{
			base.Activated();

			FieldOfView = 70;
			targetPosition = Local.Pawn.Position;
			LastPosition = Local.Pawn.Position;
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
