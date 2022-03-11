using Sandbox;
using Sandbox.UI;

[UseTemplate]
internal class PlatformerKillfeedEntry : Panel
{

	private TimeSince timeSinceCreated = 0;

	public Label Message { get; set; }

	public PlatformerKillfeedEntry( string message )
	{
		Message.Text = message;
	}

	public override void Tick()
	{
		base.Tick();

		if( timeSinceCreated > 5f )
		{
			Delete();
		}
	}

}
