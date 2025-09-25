using DG.Tweening;
using UnityEngine;

namespace com.ktgame.animation_sequencer.editor
{
	[CreateAssetMenu(menuName = "Animation Sequencer/Create Animation Sequencer Default", fileName = "AnimationControllerDefaults")]
	public sealed class AnimationControllerDefaults : EditorDefaultResourceSingleton<AnimationControllerDefaults>
	{
		public CustomEase DefaultEasing => _defaultEasing;
		public bool PreferUsingPreviousActionEasing => _preferUsingPreviousActionEasing;
		public DOTweenActionBase.AnimationDirection DefaultDirection => defaultDirection;
		public bool PreferUsingPreviousDirection => _preferUsingPreviousDirection;
		public bool UseRelative => _useRelative;
		public bool PreferUsingPreviousRelativeValue => _preferUsingPreviousRelativeValue;
		public AnimationSequencerController.AutoplayType AutoplayMode => _autoplayMode;
		public bool PlayOnAwake => _playOnAwake;
		public bool PauseOnAwake => _pauseOnAwake;
		public bool TimeScaleIndependent => _timeScaleIndependent;
		public AnimationSequencerController.PlayType PlayType => _playType;
		public UpdateType UpdateType => _updateType;
		public bool AutoKill => _autoKill;
		public int Loops => _loops;

		[SerializeField] private CustomEase _defaultEasing = CustomEase.InOutQuad;
		[SerializeField] private bool _preferUsingPreviousActionEasing = true;
		[SerializeField] private DOTweenActionBase.AnimationDirection defaultDirection = DOTweenActionBase.AnimationDirection.To;
		[SerializeField] private bool _preferUsingPreviousDirection = true;
		[SerializeField] private bool _useRelative = false;
		[SerializeField] private bool _preferUsingPreviousRelativeValue = true;
		[SerializeField] private AnimationSequencerController.AutoplayType _autoplayMode = AnimationSequencerController.AutoplayType.Awake;
		[SerializeField] private bool _playOnAwake = false;
		[SerializeField] private bool _pauseOnAwake = false;
		[SerializeField] private bool _timeScaleIndependent = false;
		[SerializeField] private AnimationSequencerController.PlayType _playType = AnimationSequencerController.PlayType.Forward;
		[SerializeField] private UpdateType _updateType = UpdateType.Normal;
		[SerializeField] private bool _autoKill = true;
		[SerializeField] private int _loops = 0;
	}
}
