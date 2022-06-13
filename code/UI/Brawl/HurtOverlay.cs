
using Sandbox;
using Sandbox.UI;

namespace Platformer.UI;

[UseTemplate]
internal partial class HurtOverlay : Panel
{

	public static HurtOverlay Current;

	private TimeUntil TimeUntilInactive;

	public HurtOverlay()
	{
		Current = this;
	}

	public void Flash()
	{
		SetClass( "active", true );
		TimeUntilInactive = 1f;
	}

	public override void Tick()
	{
		base.Tick();

		if ( TimeUntilInactive < 0 )
		{
			SetClass( "active", false );
		}
	}

	[ConCmd.Client( "HurtOverlay", CanBeCalledFromServer = true )]
	public static void FlashTo()
	{
		Current.Flash();
	}

}
