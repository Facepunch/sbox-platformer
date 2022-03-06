
public class ColorEffect : BaseEffect
{

	private Color a;
	private Color b;
	private Color c;

	public ColorEffect( Color a, Color b, Color c )
	{
		this.a = a;
		this.b = b;
		this.c = c;
	}

	public override void OnTick()
	{
		Target.SetColor( Lerp3( a, b, c, T ) );
	}

}
