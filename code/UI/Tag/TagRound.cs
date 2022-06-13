
using Platformer.Gamemodes;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Platformer.UI;

[UseTemplate]
public class TagRound : Panel
{

	public int CurrentRound { get; set; }

	public override void Tick()
	{
		base.Tick();

		if ( !Tag.Current.IsValid() ) 
			return;

		CurrentRound = Tag.Current.RoundNumber;
	}

}
