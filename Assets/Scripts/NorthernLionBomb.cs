using UnityEngine;

public class NorthernLionBomb : MonoBehaviour
{
	public KMAudio Audio;
	public AudioClip[] BombStart;
	public AudioClip[] BombExploded;
	public AudioClip[] BombDefused;
	public AudioClip[] ModuleSolved;
	public AudioClip[] Strike;
	public AudioClip[] FiveSecondsLeft;
	public AudioClip CloseAlmost;
	public AudioClip Loading;
	public AudioClip ThirtySeconds;
	public AudioClip OverOneMinuteLeft;
	public AudioClip TooEasy;
	public AudioClip FirstSolved;

	private float _startTime;
	private bool _played30Second;
	private bool _playedAboutToExplode;
	private int _strikes;
	private int _solvedModules;
	private bool _bombStarted;

	public void Start ()
	{
		var bombInfo = GetComponent<KMBombInfo>();
		bombInfo.OnBombExploded += OnBombExploded;
		bombInfo.OnBombSolved += OnBombSolved;

		Audio.HandlePlaySoundAtTransform(Loading.name, transform);

		_startTime = bombInfo.GetTime();
		_bombStarted = false;
		_played30Second = false;
		_playedAboutToExplode = false;
		_strikes = 0;
		_solvedModules = 0;
	}

	private void OnBombSolved()
	{
		var timeLeft = GetComponent<KMBombInfo>().GetTime();
		var playSpecial = Random.Range(0, 6) == 0;
		if (timeLeft > 60 && playSpecial)
		{
			Audio.HandlePlaySoundAtTransform(OverOneMinuteLeft.name, transform);
		}
		else if (timeLeft > 180 && playSpecial)
		{
			Audio.HandlePlaySoundAtTransform(TooEasy.name, transform);
		}
		else if (timeLeft < 5 && playSpecial)
		{
			Audio.HandlePlaySoundAtTransform(CloseAlmost.name, transform);
		}
		else
		{
			var i = Random.Range(0, BombDefused.Length);
			Audio.HandlePlaySoundAtTransform(BombDefused[i].name, transform);
		}
	}

	private void OnBombExploded()
	{
		var i = Random.Range(0, BombExploded.Length);
		Audio.HandlePlaySoundAtTransform(BombExploded[i].name, transform);
	}

	public void Update () {
		var bombInfo = GetComponent<KMBombInfo>();
		var time = bombInfo.GetTime();

		PlayBombStarted(time);
//		Play30SecondSound(time);
		Play5SecondsLeftSound(time);
		CheckForStrike();
		CheckForModuleSolved();
	}

	private void PlayBombStarted(float time)
	{
		if (!(_startTime - time > 2) || _bombStarted) return;

		_bombStarted = true;
		var i = Random.Range(0, BombStart.Length);
		Audio.HandlePlaySoundAtTransform(BombStart[i].name, transform);
	}

	private void CheckForModuleSolved()
	{
		var currentSolvedModules = GetComponent<KMBombInfo>().GetSolvedModuleNames().Count;
		if (currentSolvedModules > _solvedModules)
		{
			_solvedModules = currentSolvedModules;
			OnSolvedModule();
		}
	}

	private void OnSolvedModule()
	{
		if (_solvedModules == GetComponent<KMBombInfo>().GetSolvableModuleNames().Count) return;
		var i = Random.Range(0, ModuleSolved.Length);
		if (i == 0 && _solvedModules == 1)
		{
			Audio.HandlePlaySoundAtTransform(FirstSolved.name, transform);
		}
		else
		{
			Audio.HandlePlaySoundAtTransform(ModuleSolved[i].name, transform);
		}
	}

	private void CheckForStrike()
	{
		var currentStrikes = GetComponent<KMBombInfo>().GetStrikes();
		if (currentStrikes > _strikes)
		{
			_strikes = currentStrikes;
			OnStrike();
		}
	}

	private void OnStrike()
	{
		var i = Random.Range(0, Strike.Length);
        Audio.HandlePlaySoundAtTransform(Strike[i].name, transform);
	}

	private void Play5SecondsLeftSound(float timeRemaining)
	{
		if (timeRemaining > 5 || _playedAboutToExplode) return;

		var bombInfo = GetComponent<KMBombInfo>();
		var totalModules = bombInfo.GetSolvableModuleNames().Count;
		var solvedModules = bombInfo.GetSolvedModuleNames().Count;

		if (totalModules - solvedModules <= 2) return;

		_playedAboutToExplode = true;
		var i = Random.Range(0, FiveSecondsLeft.Length);
		Audio.HandlePlaySoundAtTransform(FiveSecondsLeft[i].name, transform);
	}

	private void Play30SecondSound(float timeRemaining)
	{
		if (_startTime - timeRemaining < 30 || _played30Second) return;

		_played30Second = true;
		Audio.HandlePlaySoundAtTransform(ThirtySeconds.name, transform);
	}
}
