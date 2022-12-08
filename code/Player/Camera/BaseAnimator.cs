
using Sandbox;
using Platformer;

public class BaseAnimator
{

	protected AnimatedEntity Pawn;

	public BaseAnimator( Sandbox.AnimatedEntity p )
	{
		Pawn = p;
	}

	public virtual void Simulate()
	{

	}

	public virtual void OnEvent( string eventName )
	{

	}

}
