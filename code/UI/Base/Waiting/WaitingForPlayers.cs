using Platformer.Gamemodes;
using Sandbox;
using Sandbox.UI;

namespace Platformer.UI;

public partial class WaitingForPlayers : Panel
{

	public static WaitingForPlayers Instance;
	public float timesince = 0;
	public Label newwaitingforplayers;

	public WaitingForPlayers()
	{
		StyleSheet.Load( "/ui/base/Waiting/WaitingForPlayers.scss" );

		newwaitingforplayers = AddChild<Label>( "waitingforplayers" );

		Instance = this;
	}

	public override void Tick()
	{
		base.Tick();

		if ( Time.Now - timesince < 5 && Tag.Current != null )
		{
			AddClass( "visible" );
		}
		else
		{
			RemoveClass( "visible" );
		}
	}

	public static void ShowWaitingForPlayers( string title )
	{
		if ( Instance == null ) 
			return;

		Instance.newwaitingforplayers.SetText( title );
		Instance.timesince = Time.Now;

	}
}
