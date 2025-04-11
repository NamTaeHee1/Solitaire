public class Managers
{
	public static InputManager Input { get { return InputManager.Instance; } }
	
	public static PointManager Point { get { return PointManager.Instance; } }

	public static GameManager Game { get { return GameManager.Instance; } }

    public static SoundManager Sound { get { return SoundManager.Instance; } }

    public static UIManager UI { get { return UIManager.Instance; } }
}
