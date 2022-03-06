
using Sandbox;

public class EntityJuiceTarget : IJuiceTarget
{

	private Entity entity;

	public EntityJuiceTarget( Entity entity )
	{
		this.entity = entity;
	}

	public bool IsValid()
	{
		return entity.IsValid();
	}

	public void SetScale( float scale )
	{
		entity.LocalScale = scale;
	}

	public void SetColor( Color color )
	{
		if ( entity is not ModelEntity m ) return;
		m.RenderColor = color;

		foreach( var child in m.Children )
		{
			if ( child is not ModelEntity mc ) continue;
			mc.RenderColor = color;
		}
	}

	public void SetAlpha( float a )
	{
		if ( entity is not ModelEntity ent ) return;
		ent.RenderColor = ent.RenderColor.WithAlpha( a );
	}

}

