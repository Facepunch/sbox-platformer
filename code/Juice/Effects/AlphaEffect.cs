public class AlphaEffect : BaseEffect
{

	private float a;
	private float b;
	private float c;

	public AlphaEffect( float a, float b, float c )
	{
		this.a = a;
		this.b = b;
		this.c = c;
	}

	public override void OnTick()
	{
		Target.SetAlpha( Lerp3( a, b, c, T ) );
	}

}
