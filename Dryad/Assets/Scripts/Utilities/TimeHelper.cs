class TimeHelper
{
	public static float GameTime
	{
		get
		{
			return TimeManager.GetTime(TimeType.Gameplay);
		}
	}

	public static float EngineTime
	{
		get
		{
			return TimeManager.GetTime(TimeType.Engine);
		}
	}
}
