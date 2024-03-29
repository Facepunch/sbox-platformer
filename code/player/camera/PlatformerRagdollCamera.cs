﻿
using Sandbox;

namespace Platformer
{
	public class PlatformerRagdollCamera : BaseCamera
	{
		Vector3 FocusPoint;

		public PlatformerRagdollCamera()
		{
			FocusPoint = Camera.Position - GetViewOffset();
		}

		public override void Update()
		{
			var player = Game.LocalClient;
			if ( !player.IsValid() ) return;
			if ( player.Pawn is not PlatformerPawn p ) return;

			// lerp the focus point
			FocusPoint = Vector3.Lerp( FocusPoint, GetSpectatePoint(), Time.Delta * 5.0f );
			var tr = Trace.Ray( FocusPoint + Vector3.Up * 12, FocusPoint + GetViewOffset() )
				.WorldOnly()
				.Ignore( player.Pawn )
				.Radius( 6 )
				.Run();

			Camera.Position = tr.EndPosition;
			Camera.Rotation = p.ViewAngles.ToRotation();
			Camera.FieldOfView = Camera.FieldOfView.LerpTo( 65, Time.Delta * 3.0f );

			Camera.FirstPersonViewer = null;
		}

		public virtual Vector3 GetSpectatePoint()
		{
			if ( Game.LocalPawn is Player player && player.Corpse.IsValid() )
			{
				return player.Corpse.PhysicsGroup.MassCenter;
			}

			 return Game.LocalPawn.Position;
		}

		public virtual Vector3 GetViewOffset()
		{
			var player = Game.LocalClient;
			if ( player == null ) return Vector3.Zero;
			if ( player.Pawn is not PlatformerPawn p ) return Vector3.Zero;

			return p.ViewAngles.Forward * (-350 * 1) + Vector3.Up * (20 * 1);
		}
	}
}
