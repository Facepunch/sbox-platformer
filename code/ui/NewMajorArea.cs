
using Sandbox;
using Sandbox.UI;

namespace Platformer.UI
{
	public class NewMajorArea : Panel
	{
		//public static PlatformerKillfeed Current;

		public static NewMajorArea Instance;
		public float timesince = 0;
		public Label newlandmark;
		public NewMajorArea()
		{
			StyleSheet.Load( "/ui/NewMajorArea.scss" );
			//usetext.SetText( "Interact [E]" )

			newlandmark = AddChild<Label>( "newlandmark" );

			Instance = this;
		}
		public override void Tick()
		{
			base.Tick();

			if ( Time.Now - timesince < 5 )
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
			Instance.newlandmark.SetText( title );
			Instance.timesince = Time.Now;

		}

		[ClientCmd( "plat_killfeed_add", CanBeCalledFromServer = true )]
		public static void AddEntryOnClient( string message, int clientId )
		{
			PlatformerKillfeed.Current?.AddEntry( message, clientId );
		}
	}
}
