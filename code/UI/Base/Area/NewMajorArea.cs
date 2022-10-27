using Platformer.Gamemodes;
using Sandbox;
using Sandbox.UI;

namespace Platformer.UI;

public partial class NewMajorArea : Panel
{

	public static NewMajorArea Instance;
	public float timesince = 0;
	public Label newlandmark;

	public NewMajorArea()
	{
		StyleSheet.Load( "/ui/base/area/NewMajorArea.scss" );

		newlandmark = AddChild<Label>( "newlandmark" );

		Instance = this;
	}

	public override void Tick()
	{
		base.Tick();

		if ( Time.Now - timesince < 5 && Platformer.GameState == GameStates.Live )
		{
			AddClass( "visible" );
		}
		else
		{
			RemoveClass( "visible" );
		}
	}

	public static void ShowLandmark( string title )
	{
		if ( Instance == null ) 
			return;

		Instance.newlandmark.SetText( title );
		Instance.timesince = Time.Now;

	}
}
