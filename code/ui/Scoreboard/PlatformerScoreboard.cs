using Sandbox;
using Sandbox.UI.Construct;

namespace Platformer;

public class PlatformerScoreboard : Sandbox.UI.Scoreboard<ScoreboardEntry>
{

	protected override void AddHeader()
	{
		Header = Add.Panel( "header" );
		Header.Add.Label( "", "name" );
		Header.Add.Label( "points", "points" );
		Header.Add.Label( "deaths", "deaths" );
		Header.Add.Label( "ping", "ping" );
	}

	RealTimeSince timeSinceSorted;

	public override void Tick()
	{
		base.Tick();

		if ( !IsVisible ) return;

		if ( timeSinceSorted > 0.1f )
		{
			timeSinceSorted = 0;

			//
			// Sort by number of kills, then number of deaths
			//
			Canvas.SortChildren<ScoreboardEntry>( ( x ) => (-x.Client.GetInt( "points" ) * 1000) + x.Client.GetInt( "deaths" ) );
		}
	}

	public override bool ShouldBeOpen()
	{
		if ( Platformer.CurrentState == Platformer.GameStates.GameEnd )
			return true;

		return base.ShouldBeOpen();
	}
}

public class ScoreboardEntry : Sandbox.UI.ScoreboardEntry
{

}
