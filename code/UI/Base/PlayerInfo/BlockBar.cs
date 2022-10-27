using Sandbox;
using Sandbox.UI;

namespace Platformer.UI;

public partial class BlockBar : Panel
{
	public int MaxBlocks { get; set; } = 4;
	public int CurrentBlocks { get; set; } = 4;
	public Panel Layout { get; set; }
	public Label Icon { get; set; }
	public Label Progress { get; set; }

	public BlockBar()
	{
		Icon = AddChild<Label>( "icon" );
		Icon.Text = "favorite";
		Layout = Add.Panel( "layout" );
		Progress = AddChild<Label>( "progress" );

		StyleSheet.Load( "/UI/Base/PlayerInfo/BlockBar.scss" );
	}

	public override void SetProperty( string name, string value )
	{
		base.SetProperty( name, value );

		if ( name == "max" )
			MaxBlocks = value.ToInt( 4 );
		if ( name == "current" )
			CurrentBlocks = value.ToInt( 4 );
		if ( name == "icon" )
			SetIcon( value );
	}

	int hash;
	public override void Tick()
	{
		if ( GetProgress().GetHashCode() != hash )
		{
			Rebuild();
		}
	}

	public string GetProgress()
	{
		return $"{CurrentBlocks}/{MaxBlocks}";
	}

	public void Rebuild()
	{
		Layout.DeleteChildren( true );

		for ( int i = 0; i < MaxBlocks; i++ )
		{
			var block = Layout.Add.Panel( "block" );
			block.SetClass( "active", CurrentBlocks > i );
		}

		Progress.Text = GetProgress();
		hash = Progress.Text.GetHashCode();
	}

	public void SetIcon( string icon )
	{
		Icon.Text = icon;
	}

}
