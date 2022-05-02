
using Platformer.Movement;
using Sandbox.UI;

namespace Platformer;

[UseTemplate]
internal class ControlEntry : Panel
{

	public ControlEntry( BaseMoveMechanic mechanic )
	{
		Name = mechanic.Name;
		Description = mechanic.Description;
	}

	public string Name { get; set; }
	public string Description { get; set; }

}
