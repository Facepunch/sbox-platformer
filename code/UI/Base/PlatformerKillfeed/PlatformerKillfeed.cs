using Sandbox;
using Sandbox.UI;
using System.Linq;

[UseTemplate]
internal partial class PlatformerKillfeed : Panel
{

	public static PlatformerKillfeed Current;

	public Panel Canvas { get; set; }

	public PlatformerKillfeed()
	{
		Current = this;
	}

	public void AddEntry( string message, int clientId )
	{
		var entry = new PlatformerKillfeedEntry( message );
		entry.Parent = Current.Canvas;

		if( clientId == Local.Client.NetworkIdent )
		{
			entry.AddClass( "local" );
		}

		var count = Current.Canvas.ChildrenCount;

		if( count > 5 )
		{
			foreach(var child in Current.Canvas.Children.Take( count - 5 ) )
			{
				child.Delete();
			}
		}
	}

	[ConCmd.Client( "plat_killfeed_add", CanBeCalledFromServer = true)]
	public static void AddEntryOnClient( string message, int clientId )
	{
		Current?.AddEntry( message, clientId );
	}
}
