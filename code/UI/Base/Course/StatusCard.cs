using Sandbox.UI;

namespace Platformer.UI;

[UseTemplate]
public partial class StatusCard : Panel
{
	// @text
	public string Icon { get; set; }
	// @text 
	public string Header { get; set; }
	// @text
	public string Message { get; set; }
	
	public bool ReverseColor { get; set; }

	public StatusCard()
	{
		AddClass( "status-card" );

		Icon = "schedule";
		Header = "WARM UP";
		Message = "0:16";

		BindClass( "reverse-color", () => ReverseColor );
	}

	public override void SetProperty( string name, string value )
	{
		if ( name == "reverse" )
		{
			ReverseColor = true;
		}
	}
}
