using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace com.ktgame.animation_sequencer
{
    [DisallowMultipleComponent]
    [AddComponentMenu("UI/Animation Sequencer Controller", 200)]
    public class AnimationSequencerController : MonoBehaviour
    {
        public enum PlayType
        {
            Forward,
            Backward
        }

        public enum AutoplayType
        {
            Awake,
            OnEnable,
            Nothing
        }
        public AnimationStepBase[] AnimationSteps => _animationSteps;
        public float PlaybackSpeed => _playbackSpeed;
        
        [SerializeReference] private AnimationStepBase[] _animationSteps = Array.Empty<AnimationStepBase>();
        [SerializeField] private UpdateType _updateType = UpdateType.Normal;
        [SerializeField] private bool _timeScaleIndependent = false;
        [SerializeField] private AutoplayType _autoplayMode = AutoplayType.Awake;
        [SerializeField] protected bool _startPaused;
        [SerializeField] private float _playbackSpeed = 1f; 
        [SerializeField] protected PlayType _playType = PlayType.Forward;
        [SerializeField] private int _loops = 0;
        [SerializeField] private LoopType _loopType = LoopType.Restart;
        [SerializeField] private bool _autoKill = true;
        [SerializeField] private UnityEvent _onStartEvent = new UnityEvent();

        public UnityEvent OnStartEvent
        {
            get => _onStartEvent;
            protected set => _onStartEvent = value;
        }

        [SerializeField] private UnityEvent _onFinishedEvent = new UnityEvent();

        public UnityEvent OnFinishedEvent
        {
            get => _onFinishedEvent;
            protected set => _onFinishedEvent = value;
        }

        [SerializeField] private UnityEvent _onProgressEvent = new UnityEvent();
        public UnityEvent OnProgressEvent => _onProgressEvent;

        private Sequence _playingSequence;
        public Sequence PlayingSequence => _playingSequence;
        private PlayType _playTypeInternal = PlayType.Forward;
#if UNITY_EDITOR
        private bool _requiresReset = false;
#endif

        public bool IsPlaying => _playingSequence != null && _playingSequence.IsActive() && _playingSequence.IsPlaying();
        public bool IsPaused => _playingSequence != null && _playingSequence.IsActive() && !_playingSequence.IsPlaying();

        [SerializeField, Range(0, 1)] 
        private float _progress = -1;

        protected virtual void Awake()
        {
            _progress = -1;
            if (_autoplayMode != AutoplayType.Awake)
            {
                return;
            }

            Autoplay();
        }

        protected virtual void OnEnable()
        {
            if (_autoplayMode != AutoplayType.OnEnable)
            {
                return;
            }

            Autoplay();
        }

        private void Autoplay()
        {
            Play();
            if (_startPaused)
            {
                _playingSequence.Pause();
            }
        }

        protected virtual void OnDisable()
        {
            if (_autoplayMode != AutoplayType.OnEnable)
            {
                return;
            }

            if (_playingSequence == null)
            {
                return;
            }

            ClearPlayingSequence();
            // Reset the object to its initial state so that if it is re-enabled the start values are correct for
            // regenerating the Sequence.
            ResetToInitialState();
        }

        protected virtual void OnDestroy()
        {
            ClearPlayingSequence();
        }

        public virtual void Play()
        {
            Play(null);
        }

        public virtual void Play(Action onCompleteCallback)
        {
            _playTypeInternal = _playType;

            ClearPlayingSequence();

            if (onCompleteCallback != null)
            {
                _onFinishedEvent.AddListener(onCompleteCallback.Invoke);
            }

            _playingSequence = GenerateSequence();

            switch (_playTypeInternal)
            {
                case PlayType.Backward:
                    _playingSequence.PlayBackwards();
                    break;

                case PlayType.Forward:
                    _playingSequence.PlayForward();
                    break;

                default:
                    _playingSequence.Play();
                    break;
            }
        }

        public virtual void PlayForward(bool resetFirst = true, Action onCompleteCallback = null)
        {
            if (_playingSequence == null)
            {
                Play();
            }

            _playTypeInternal = PlayType.Forward;

            if (onCompleteCallback != null)
            {
                _onFinishedEvent.AddListener(onCompleteCallback.Invoke);
            }

            if (resetFirst)
            {
                SetProgress(0);
            }

            _playingSequence.PlayForward();
        }

        public virtual void PlayBackwards(bool completeFirst = true, Action onCompleteCallback = null)
        {
            if (_playingSequence == null)
            {
                Play();
            }

            _playTypeInternal = PlayType.Backward;

            if (onCompleteCallback != null)
            {
                _onFinishedEvent.AddListener(onCompleteCallback.Invoke);
            }

            if (completeFirst)
            {
                SetProgress(1);
            }

            _playingSequence.PlayBackwards();
        }

        public virtual void SetTime(float seconds, bool andPlay = true)
        {
            if (_playingSequence == null)
            {
                Play();
            }

            _playingSequence.Goto(seconds, andPlay);
        }

        public virtual void SetProgress(float targetProgress, bool andPlay = true)
        {
            if (_playingSequence == null)
            {
                Play();
            }

            targetProgress = Mathf.Clamp01(targetProgress);
            
            float duration = _playingSequence.Duration();
            float finalTime = targetProgress * duration;
            SetTime(finalTime, andPlay);
        }

        public virtual void TogglePause()
        {
            if (_playingSequence == null)
            {
                return;
            }

            _playingSequence.TogglePause();
        }

        public virtual void Pause()
        {
            if (!IsPlaying)
            {
                return;
            }

            _playingSequence.Pause();
        }

        public virtual void Resume()
        {
            if (_playingSequence == null)
            {
                return;
            }

            _playingSequence.Play();
        }


        public virtual void Complete(bool withCallbacks = true)
        {
            if (_playingSequence == null)
            {
                return;
            }

            _playingSequence.Complete(withCallbacks);
        }

        public virtual void Rewind(bool includeDelay = true)
        {
            if (_playingSequence == null)
            {
                return;
            }

            _playingSequence.Rewind(includeDelay);
        }

        public virtual void Kill(bool complete = false)
        {
            if (!IsPlaying)
            {
                return;
            }

            _playingSequence.Kill(complete);
        }

        public virtual IEnumerator PlayEnumerator()
        {
            Play();
            yield return _playingSequence.WaitForCompletion();
        }

        public virtual Sequence GenerateSequence()
        {
            Sequence sequence = DOTween.Sequence();

            // Various edge cases exists with OnStart() and OnComplete(), some of which can be solved with OnRewind(),
            // but it still leaves callbacks unfired when reversing _direction after natural completion of the animation.
            // Rather than using the in-built callbacks, we simply bookend the Sequence with AppendCallback to ensure
            // a Start and Finish callback is always fired.
            sequence.AppendCallback(() =>
            {
                if (_playTypeInternal == PlayType.Forward)
                {
                    _onStartEvent.Invoke();
                }
                else
                {
                    _onFinishedEvent.Invoke();
                }
            });

            for (int i = 0; i < _animationSteps.Length; i++)
            {
                AnimationStepBase animationStepBase = _animationSteps[i];
                animationStepBase.AddTweenToSequence(sequence);
            }

            sequence.SetTarget(this);
            sequence.SetAutoKill(_autoKill);
            sequence.SetUpdate(_updateType, _timeScaleIndependent);
            sequence.OnUpdate(() => { _onProgressEvent.Invoke(); });
            // See comment above regarding bookending via AppendCallback.
            sequence.AppendCallback(() =>
            {
                if (_playTypeInternal == PlayType.Forward)
                {
                    _onFinishedEvent.Invoke();
                }
                else
                {
                    _onStartEvent.Invoke();
                }
            });

            int targetLoops = _loops;

            if (!Application.isPlaying)
            {
                if (_loops == -1)
                {
                    targetLoops = 10;
                    Debug.LogWarning("Infinity sequences on editor can cause issues, using 10 _loops while on editor.");
                }
            }

            sequence.SetLoops(targetLoops, _loopType);
            sequence.timeScale = _playbackSpeed;
            return sequence;
        }

        public virtual void ResetToInitialState()
        {
            _progress = -1.0f;
            for (int i = _animationSteps.Length - 1; i >= 0; i--)
            {
                _animationSteps[i].ResetToInitialState();
            }
        }

        public void ClearPlayingSequence()
        {
            DOTween.Kill(this);
            DOTween.Kill(_playingSequence);
            _playingSequence = null;
        }

        public void SetAutoplayMode(AutoplayType autoplayType)
        {
            _autoplayMode = autoplayType;
        }

        public void SetPlayOnAwake(bool targetPlayOnAwake)
        {
        }

        public void SetPauseOnAwake(bool targetPauseOnAwake)
        {
            _startPaused = targetPauseOnAwake;
        }

        public void SetTimeScaleIndependent(bool targetTimeScaleIndependent)
        {
            _timeScaleIndependent = targetTimeScaleIndependent;
        }

        public void SetPlayType(PlayType targetPlayType)
        {
            _playType = targetPlayType;
        }

        public void SetUpdateType(UpdateType targetUpdateType)
        {
            _updateType = targetUpdateType;
        }

        public void SetAutoKill(bool targetAutoKill)
        {
            _autoKill = targetAutoKill;
        }

        public void SetLoops(int targetLoops)
        {
            _loops = targetLoops;
        }

        private void Update()
        {
            if (_progress == -1.0f)
                return;

            SetProgress(_progress);
        }

#if UNITY_EDITOR
        // Unity Event Function called when component is added or reset.
        private void Reset()
        {
            _requiresReset = true;
        }

        // Used by the CustomEditor so it knows when to reset to the defaults.
        public bool IsResetRequired()
        {
            return _requiresReset;
        }

        // Called by the CustomEditor once the reset has been completed 
        public void ResetComplete()
        {
            _requiresReset = false;
        }
#endif
        public bool TryGetStepAtIndex<T>(int index, out T result) where T : AnimationStepBase
        {
            if (index < 0 || index > _animationSteps.Length - 1)
            {
                result = null;
                return false;
            }

            result = _animationSteps[index] as T;
            return result != null;
        }

        public void ReplaceTarget<T>(GameObject targetGameObject) where T : GameObjectAnimationStep
        {
            for (int i = _animationSteps.Length - 1; i >= 0; i--)
            {
                AnimationStepBase animationStepBase = _animationSteps[i];
                if (animationStepBase == null)
                {
                    continue;
                }

                if (animationStepBase is not T gameObjectAnimationStep)
                {
                    continue;
                }

                gameObjectAnimationStep.SetTarget(targetGameObject);
            }
        }

        public void ReplaceTargets(params (GameObject original, GameObject target)[] replacements)
        {
            for (int i = 0; i < replacements.Length; i++)
            {
                (GameObject original, GameObject target) replacement = replacements[i];
                ReplaceTargets(replacement.original, replacement.target);
            }
        }

        public void ReplaceTargets(GameObject originalTarget, GameObject newTarget)
        {
            for (int i = _animationSteps.Length - 1; i >= 0; i--)
            {
                AnimationStepBase animationStepBase = _animationSteps[i];
                if (animationStepBase == null)
                {
                    continue;
                }

                if(animationStepBase is not GameObjectAnimationStep gameObjectAnimationStep)
                {
                    continue;
                }

                if (gameObjectAnimationStep.Target == originalTarget)
                {
                    gameObjectAnimationStep.SetTarget(newTarget);
                }
            }
        }
        
        public async UniTask PlayAsync()
        {
            await PlayEnumerator().ToUniTask(this);
        }
    }
}