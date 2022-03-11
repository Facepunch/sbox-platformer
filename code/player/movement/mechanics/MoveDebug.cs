
using Sandbox;

namespace Platformer.Movement
{
	class MoveDebug : BaseMoveMechanic
	{

		public override bool AlwaysSimulate => true;

		public MoveDebug( PlatformerController ctrl )
			: base( ctrl )
		{

		}

		public override void PostSimulate()
		{
			if ( BasePlayerController.Debug )
			{
				var boxColor = Host.IsServer ? Color.Red : Color.Green;
				DebugOverlay.Box( ctrl.Position + ctrl.TraceOffset, ctrl.Mins, ctrl.Maxs, boxColor );
				DebugOverlay.Box( ctrl.Position, ctrl.Mins, ctrl.Maxs, boxColor );

				var lineOffset = Host.IsServer ? 10 : 0;
				var printTime = Host.IsServer ? 0f : 0.04f;
				// todo: print this shit so it doesn't flicker
				DebugOverlay.ScreenText( lineOffset + 0, $"        Position: {ctrl.Position}", printTime );
				DebugOverlay.ScreenText( lineOffset + 1, $"        Velocity: {ctrl.Velocity}", printTime );
				DebugOverlay.ScreenText( lineOffset + 2, $"           Speed: {ctrl.Velocity.Length}", printTime );
				DebugOverlay.ScreenText( lineOffset + 3, $"    BaseVelocity: {ctrl.BaseVelocity}", printTime );
				DebugOverlay.ScreenText( lineOffset + 4, $"    GroundEntity: {ctrl.GroundEntity} [{ctrl.GroundEntity?.Velocity}]", printTime );
				DebugOverlay.ScreenText( lineOffset + 5, $"    WishVelocity: {ctrl.WishVelocity}", printTime );
			}
		}

	}
}
