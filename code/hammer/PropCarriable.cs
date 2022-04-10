
using Sandbox;

namespace Platformer;

[Library("plat_prop_carriable")]
internal class PropCarriable : Prop, IUse
{

	public bool IsUsable( Entity user )
	{
		return true;
	}

	public bool OnUse( Entity user )
	{
		Log.Error( "Used a bitch" );
		return true;
	}

}
