
using Platformer;
using Platformer.Gamemodes;
using Sandbox;
using Sandbox.UI;
using System.Collections.Generic;
using System.Linq;


namespace Platformer;

[UseTemplate]
public class Scoreboard : Panel
{

	bool Cursor;
	RealTimeSince timeSinceSorted;
	Dictionary<Client, ScoreboardEntry> Rows = new();

	public Panel Canvas { get; protected set; }
	public Panel Header { get; protected set; }

	public override void Tick()
	{
		base.Tick();

		SetClass( "open", ShouldBeOpen() );

		if ( !IsVisible )
			return;

		//
		// Clients that were added
		//
		foreach ( var client in Client.All.Except( Rows.Keys ) )
		{
			var entry = AddClient( client );
			Rows[client] = entry;
		}

		foreach ( var client in Rows.Keys.Except( Client.All ) )
		{
			if ( Rows.TryGetValue( client, out var row ) )
			{
				row?.Delete();
				Rows.Remove( client );
			}
		}


		if ( !HasClass( "open" ) ) Cursor = false;
		if ( !IsVisible ) return;


		if ( timeSinceSorted > 0.1f )
		{
			timeSinceSorted = 0;
			{
				//
				// Sort by number of kills, then number of deaths
				//
				Canvas.SortChildren<ScoreboardEntry>( ( x ) => (-x.Client.GetInt( "kills" ) * 1000) + x.Client.GetInt( "deaths" ) );
			}
		}
	}

	private bool ShouldBeOpen()
	{
		if ( Platformer.GameState == GameStates.GameEnd )
			return true;
		
		if ( Input.Down( InputButton.Score ) )
			return true;

		return false;
	}

	private ScoreboardEntry AddClient( Client entry )
	{
		var p = Canvas.AddChild<ScoreboardEntry>();
		p.Client = entry;

		if ( entry == Local.Client )
		{
			p.AddChild<Label>( "you" ).Text = "you";
		}

		return p;
	}
}
public class ScoreboardEntry : Sandbox.UI.ScoreboardEntry
{
	public ScoreboardEntry()
	{

	}

	public void OnClick()
	{
		if ( Client == Local.Client ) return;
	}
}

