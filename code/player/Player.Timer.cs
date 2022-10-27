
using Platformer.Gamemodes;
using Platformer.UI;
using Sandbox;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Platformer;

partial class PlatformerPawn
{

	[Net, Predicted]
	public TimerState TimerState { get; set; }

	[Net]
	public TimeSince TimeSinceStart { get; set; }

	[Net]
	public float BestTime { get; set; } = defaultBestTime;

	public void StartCourse()
	{
		TimeSinceStart = 0;
		TimerState = TimerState.Live;
		Velocity = Velocity.ClampLength( 240 );
	}

	public bool CourseIncomplete => BestTime == defaultBestTime;

	private const float defaultBestTime = 3600f;

	public void ResetBestTime()
	{
		BestTime = defaultBestTime;
	}

	public async Task CompleteCourseAsync()
	{
		if( Coop.Current != null )
		{
			Coop.Current.CoopTimerState = TimerState.Finished;

			var span = TimeSpan.FromSeconds( (Coop.Current.TimeCoopStart * 60).Clamp( 0, float.MaxValue ) );
			var formattedTime = span.ToString( @"hh\:mm\:ss" );

			PlatformerChatBox.AddChatEntry( To.Everyone, Client.Name, $"Completed the course in {formattedTime}", Client.PlayerId );
		}

		if( Competitive.Current != null )
		{
			TimerState = TimerState.Finished;

			if ( !IsServer ) return;

			var span = TimeSpan.FromSeconds( TimeSinceStart );
			var formattedTime = span.ToString( @"mm\:ss" );

			ClearCheckpoints();
			PlatformerChatBox.AddChatEntry( To.Everyone, Client.Name,$"Completed the course in {formattedTime}", Client.PlayerId );
			Celebrate();

			if( this is CompetitivePlayer pl )
			{
				pl.KeysPlayerHas.Clear();
				pl.NumberOfKeys = 0;
			}

			if ( TimeSinceStart < BestTime )
			{
				BestTime = TimeSinceStart;

				Platformer.SubmitScore( "Time", Client, BestTime.CeilToInt() );
			}
		}
	}

	[ConCmd.Admin( "plat_debug_coursecompleted" )]
	public static void DebugMsgOther()
	{
		PlatformerChatBox.AddChatEntry( To.Everyone, "Eagle One Development Team", "Completed the course in 54:40!", 76561197967441886 );
		//PlatformerKillfeed.AddEntryOnClient( To.Everyone, $"Eagle One Development Team has completed the course in 54:40", 1 );
	}


		public void ResetTimer()
	{
		TimerState = TimerState.InStartZone;
		TimeSinceStart = 0;

		if ( IsServer )
		{
			ClearCheckpoints();
		}
	}

	public void ClearCheckpoints()
	{
		Host.AssertServer();

		Checkpoints.Clear();
	}

	public void TrySetCheckpoint( Checkpoint checkpoint, bool overridePosition = false )
	{
		Host.AssertServer();

		if ( Checkpoints.Contains( checkpoint ) )
		{

			if ( overridePosition )
			{
				for ( int i = Checkpoints.Count - 1; i >= 0; i-- )
				{
					if ( Checkpoints[i] != checkpoint )
						Checkpoints.RemoveAt( i );

				}

			}
			return;
		}

		Checkpoints.Add( checkpoint );
	}

	public void GotoBestCheckpoint()
	{
		Host.AssertServer();

		var cp = Checkpoints.LastOrDefault( x => x.RespawnPoint || x.IsStart );
		if ( !cp.IsValid() )
		{
			cp = Entity.All.FirstOrDefault( x => x is Checkpoint c && c.IsStart ) as Checkpoint;
			if ( cp == null ) return;
		}

		cp.GetSpawnPoint( out Vector3 position, out Rotation rotation );

		Position = position + Vector3.Up * 5;
		Rotation = rotation;
		Velocity = Vector3.Zero;

		//SetRotationOnClient( Rotation );
		ResetInterpolation();
	}

	[ClientRpc]
	private void Celebrate()
	{
		if ( !IsLocalPawn ) return;
		Particles.Create( "particles/finish/finish_effect.vpcf" );
		Sound.FromScreen( "course.complete" );

	}

}

public enum TimerState
{
	InStartZone,
	Live,
	Finished
}
