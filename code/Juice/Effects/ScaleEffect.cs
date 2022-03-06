
public class ScaleEffect : BaseEffect
{

	private float a;
	private float b;
	private float c;

	public ScaleEffect( float a, float b, float c )
	{
		this.a = a;
		this.b = b;
		this.c = c;
	}

	public override void OnTick()
	{
		Target.SetScale( Lerp3( a, b, c, T ) );
	}

}
