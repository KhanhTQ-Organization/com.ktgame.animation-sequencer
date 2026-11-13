#if DOTWEEN_ENABLED
using System;
using System.Collections;
using System.Collections.Generic;
#if UNITASK_ENABLED
using System.Threading;
using Cysharp.Threading.Tasks;
#endif
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace BrunoMikoski.AnimationSequencer
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

        [SerializeReference] 
        private AnimationStepBase[] animationSteps = Array.Empty<AnimationStepBase>();
        public AnimationStepBase[] AnimationSteps => animationSteps;

        [SerializeField] 
        private UpdateType updateType = UpdateType.Normal;
        [SerializeField] 
        private bool timeScaleIndependent = false;
        [SerializeField] 
        private AutoplayType autoplayMode = AutoplayType.Awake;
        [SerializeField] 
        protected bool startPaused;
        [SerializeField] 
        private float playbackSpeed = 1f;
        public float PlaybackSpeed => playbackSpeed;
        [SerializeField] 
        protected PlayType playType = PlayType.Forward;
        [SerializeField] 
        private int loops = 0;
        [SerializeField] 
        private LoopType loopType = LoopType.Restart;
        [SerializeField] 
        private bool autoKill = true;

        [SerializeField] 
        private UnityEvent onStartEvent = new UnityEvent();

        public UnityEvent OnStartEvent
        {
            get => onStartEvent;
            protected set => onStartEvent = value;
        }

        [SerializeField] 
        private UnityEvent onFinishedEvent = new UnityEvent();

        public UnityEvent OnFinishedEvent
        {
            get => onFinishedEvent;
            protected set => onFinishedEvent = value;
        }

        [SerializeField] 
        private UnityEvent onProgressEvent = new UnityEvent();
        public UnityEvent OnProgressEvent => onProgressEvent;

        private Sequence playingSequence;
        public Sequence PlayingSequence => playingSequence;
        private PlayType playTypeInternal = PlayType.Forward;

        private readonly List<UnityAction> pendingCompleteCallbacks = new List<UnityAction>();

        private static readonly HashSet<AnimationSequencerController> generatingControllers = new HashSet<AnimationSequencerController>();

        private static readonly HashSet<AnimationSequencerController> estimatingControllers = new HashSet<AnimationSequencerController>();

        public bool HasActiveSequence => playingSequence != null && playingSequence.IsActive();

        public event Action<int, AnimationStepBase> StepStarted;
#if UNITY_EDITOR
        private bool requiresReset = false;
#endif

        public bool IsPlaying => playingSequence != null && playingSequence.IsActive() && playingSequence.IsPlaying();
        public bool IsPaused => playingSequence != null && playingSequence.IsActive() && !playingSequence.IsPlaying();

        [SerializeField, Range(0, 1)] 
        private float progress = -1;

        protected virtual void Awake()
        {
            progress = -1;
            if (autoplayMode != AutoplayType.Awake)
                return;

            Autoplay();
        }

        protected virtual void OnEnable()
        {
            if (autoplayMode != AutoplayType.OnEnable)
                return;

            Autoplay();
        }

        private void Autoplay()
        {
            Play();
            if (startPaused)
                playingSequence.Pause();
        }

        protected virtual void OnDisable()
        {
            if (autoplayMode != AutoplayType.OnEnable)
                return;

            if (playingSequence == null)
                return;

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
            playTypeInternal = playType;

            ClearPlayingSequence();

            AddOneShotCompleteCallback(onCompleteCallback);

            playingSequence = GenerateSequence();

            switch (playTypeInternal)
            {
                case PlayType.Backward:
                    GotoEnd(playingSequence);
                    playingSequence.PlayBackwards();
                    break;

                case PlayType.Forward:
                    playingSequence.PlayForward();
                    break;

                default:
                    playingSequence.Play();
                    break;
            }
        }

        public virtual void PlayForward(bool resetFirst = true, Action onCompleteCallback = null)
        {
            if (!HasActiveSequence)
                Play();

            playTypeInternal = PlayType.Forward;

            AddOneShotCompleteCallback(onCompleteCallback);

            if (resetFirst)
                SetProgress(0);

            playingSequence.PlayForward();
        }

        public virtual void PlayBackwards(bool completeFirst = true, Action onCompleteCallback = null)
        {
            if (!HasActiveSequence)
                Play();

            playTypeInternal = PlayType.Backward;

            AddOneShotCompleteCallback(onCompleteCallback);

            if (completeFirst)
                SetProgress(1);

            playingSequence.PlayBackwards();
        }

        public virtual void SetTime(float seconds, bool andPlay = true)
        {
            if (!HasActiveSequence)
                Play();

            playingSequence.Goto(seconds, andPlay);
        }

        public virtual void SetProgress(float targetProgress, bool andPlay = true)
        {
            if (!HasActiveSequence)
                Play();
            
            targetProgress = Mathf.Clamp01(targetProgress);
            
            float duration = playingSequence.Duration();
            float finalTime = targetProgress * duration;
            SetTime(finalTime, andPlay);
        }

        public virtual void TogglePause()
        {
            if (!HasActiveSequence)
                return;

            playingSequence.TogglePause();
        }

        public virtual void Pause()
        {
            if (!IsPlaying)
                return;

            playingSequence.Pause();
        }

        public virtual void Resume()
        {
            if (!HasActiveSequence)
                return;

            playingSequence.Play();
        }

        public virtual void Restart(bool includeDelay = true)
        {
            if (!HasActiveSequence)
            {
                Play();
                return;
            }

            playingSequence.Restart(includeDelay);
        }

        public void SetPlaybackSpeed(float targetPlaybackSpeed)
        {
            playbackSpeed = Mathf.Max(0f, targetPlaybackSpeed);
            if (HasActiveSequence)
                playingSequence.timeScale = playbackSpeed;
        }


        public virtual void Complete(bool withCallbacks = true)
        {
            if (!HasActiveSequence)
                return;
            
            // Prepare for Complete().
            for (int i = 0; i < animationSteps.Length; i++)
            {
                AnimationStepBase animationStepBase = animationSteps[i];
                if (animationStepBase == null)
                    continue;

                animationStepBase.IsSkippingToEnd = true;
                if (animationStepBase is InvokeCallbackAnimationStep invokeCallbackStep)
                {
                    invokeCallbackStep.AllowCallbacks = withCallbacks;
                }
            }

            // Always fire callbacks so the Set steps are always fired.
            playingSequence.Complete(withCallbacks: true);

            // Reset.
            for (int i = 0; i < animationSteps.Length; i++)
            {
                AnimationStepBase animationStepBase = animationSteps[i];
                if (animationStepBase == null)
                    continue;

                animationStepBase.IsSkippingToEnd = false;
                if (animationStepBase is InvokeCallbackAnimationStep invokeCallbackStep)
                {
                    invokeCallbackStep.AllowCallbacks = true;
                }
            }
        }

        public virtual void Rewind(bool includeDelay = true)
        {
            if (!HasActiveSequence)
                return;

            playingSequence.Rewind(includeDelay);
        }

        public virtual void Kill(bool complete = false)
        {
            if (!HasActiveSequence)
                return;

            Sequence sequence = playingSequence;
            sequence.Kill(complete);
            if (playingSequence == sequence)
            {
                playingSequence = null;
                ClearPendingCompleteCallbacks();
            }
        }

        public virtual IEnumerator PlayEnumerator()
        {
            Play();
            Sequence sequence = playingSequence;
            Func<bool> finished = TrackFinished(sequence);

            while (!IsSequenceOver(sequence, finished))
                yield return null;
        }

        private static void GotoEnd(Sequence sequence)
        {
            float end = sequence.Duration();
            if (float.IsInfinity(end))
                end = sequence.Duration(false);

            sequence.Goto(end, false);
        }

        private static Func<bool> TrackFinished(Sequence sequence)
        {
            bool finished = false;
            sequence.onComplete += () => finished = true;
            sequence.onRewind += () =>
            {
                if (sequence.IsBackwards())
                    finished = true;
            };
            return () => finished;
        }

        private bool IsSequenceOver(Sequence sequence, Func<bool> finished)
        {
            return finished() || !sequence.IsActive() || sequence.IsComplete() || playingSequence != sequence;
        }

        public virtual Sequence GenerateSequence()
        {
            Sequence sequence = DOTween.Sequence();

            if (!generatingControllers.Add(this))
            {
                Debug.LogError($"[AnimationSequencer] {name} plays itself through a Play Sequence step (directly or via another controller). The nested step is skipped to avoid infinite recursion.", this);
                return sequence;
            }

            try
            {
                BuildSequence(sequence);
            }
            finally
            {
                generatingControllers.Remove(this);
            }

            return sequence;
        }

        private void BuildSequence(Sequence sequence)
        {
            // Various edge cases exists with OnStart() and OnComplete(), some of which can be solved with OnRewind(),
            // but it still leaves callbacks unfired when reversing direction after natural completion of the animation.
            // Rather than using the in-built callbacks, we simply bookend the Sequence with AppendCallback to ensure
            // a Start and Finish callback is always fired.
            sequence.AppendCallback(() =>
            {
                if (playTypeInternal == PlayType.Forward)
                {
                    onStartEvent.Invoke();
                }
                else
                {
                    onFinishedEvent.Invoke();
                }
            });

            float lastInsertPosition = 0f;
            List<(float time, int index)> stepStarts = StepStarted != null ? new List<(float, int)>() : null;

            for (int i = 0; i < animationSteps.Length; i++)
            {
                AnimationStepBase animationStepBase = animationSteps[i];
                if (animationStepBase == null || animationStepBase.IsMuted)
                    continue;

                if (stepStarts != null && animationStepBase.AddsToSequence)
                {
                    float insertPosition = animationStepBase.FlowType == FlowType.Join ? lastInsertPosition : sequence.Duration(false);
                    lastInsertPosition = insertPosition;
                    stepStarts.Add((insertPosition + Mathf.Max(0f, animationStepBase.Delay), i));
                }

                animationStepBase.AddTweenToSequence(sequence);
            }

            if (stepStarts != null)
            {
                for (int i = 0; i < stepStarts.Count; i++)
                {
                    int stepIndex = stepStarts[i].index;
                    AnimationStepBase step = animationSteps[stepIndex];
                    sequence.InsertCallback(stepStarts[i].time, () => StepStarted?.Invoke(stepIndex, step));
                }
            }

            sequence.SetTarget(this);
            sequence.SetAutoKill(autoKill);
            sequence.SetUpdate(updateType, timeScaleIndependent);
            sequence.OnUpdate(() => { onProgressEvent.Invoke(); });
            // See comment above regarding bookending via AppendCallback.
            sequence.AppendCallback(() =>
            {
                if (playTypeInternal == PlayType.Forward)
                {
                    onFinishedEvent.Invoke();
                }
                else
                {
                    onStartEvent.Invoke();
                }
            });

            int targetLoops = loops;

            if (!Application.isPlaying)
            {
                if (loops == -1)
                {
                    targetLoops = 10;
                    Debug.LogWarning("Infinity sequences on editor can cause issues, using 10 loops while on editor.");
                }
            }

            sequence.SetLoops(targetLoops, loopType);
            sequence.timeScale = playbackSpeed;
        }

        public virtual void ResetToInitialState()
        {
            progress = -1.0f;
            for (int i = animationSteps.Length - 1; i >= 0; i--)
            {
                AnimationStepBase step = animationSteps[i];
                if (step == null || step.IsMuted)
                    continue;

                step.ResetToInitialState();
            }
        }

        public void ClearPlayingSequence()
        {
            DOTween.Kill(this);
            DOTween.Kill(playingSequence);
            playingSequence = null;
            ClearPendingCompleteCallbacks();
        }

        private void AddOneShotCompleteCallback(Action onCompleteCallback)
        {
            if (onCompleteCallback == null)
                return;

            UnityAction wrapper = null;
            wrapper = () =>
            {
                onFinishedEvent.RemoveListener(wrapper);
                pendingCompleteCallbacks.Remove(wrapper);
                onCompleteCallback();
            };

            pendingCompleteCallbacks.Add(wrapper);
            onFinishedEvent.AddListener(wrapper);
        }

        private void ClearPendingCompleteCallbacks()
        {
            for (int i = 0; i < pendingCompleteCallbacks.Count; i++)
            {
                onFinishedEvent.RemoveListener(pendingCompleteCallbacks[i]);
            }

            pendingCompleteCallbacks.Clear();
        }

        public void SetAutoplayMode(AutoplayType autoplayType)
        {
            autoplayMode = autoplayType;
        }

        [Obsolete("Has no effect. Use SetAutoplayMode(AutoplayType) instead.")]
        public void SetPlayOnAwake(bool targetPlayOnAwake)
        {
        }

        public void SetPauseOnAwake(bool targetPauseOnAwake)
        {
            startPaused = targetPauseOnAwake;
        }

        public void SetTimeScaleIndependent(bool targetTimeScaleIndependent)
        {
            timeScaleIndependent = targetTimeScaleIndependent;
        }

        public void SetPlayType(PlayType targetPlayType)
        {
            playType = targetPlayType;
        }

        public void SetUpdateType(UpdateType targetUpdateType)
        {
            updateType = targetUpdateType;
        }

        public void SetAutoKill(bool targetAutoKill)
        {
            autoKill = targetAutoKill;
        }

        public void SetLoops(int targetLoops)
        {
            loops = targetLoops;
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (progress == -1.0f)
                return;

            SetProgress(progress);
        }
#endif

#if UNITY_EDITOR
        // Unity Event Function called when component is added or reset.
        private void Reset()
        {
            requiresReset = true;
        }

        // Used by the CustomEditor so it knows when to reset to the defaults.
        public bool IsResetRequired()
        {
            return requiresReset;
        }

        // Called by the CustomEditor once the reset has been completed 
        public void ResetComplete()
        {
            requiresReset = false;
        }
#endif
        public float EstimateDuration(bool includeLoops = true)
        {
            float single = EstimateStepTimings(null);
            if (!includeLoops)
                return single;

            if (loops < 0)
                return Application.isPlaying ? float.PositiveInfinity : single * 10;

            return single * Mathf.Max(1, loops);
        }

        public (float start, float end)[] GetEstimatedStepTimings()
        {
            var timings = new (float start, float end)[animationSteps.Length];
            EstimateStepTimings(timings);
            return timings;
        }

        private float EstimateStepTimings((float start, float end)[] timings)
        {
            if (!estimatingControllers.Add(this))
            {
                if (timings != null)
                {
                    for (int i = 0; i < timings.Length; i++)
                        timings[i] = (-1f, -1f);
                }

                return 0f;
            }

            try
            {
                float sequenceEnd = 0f;
                float lastInsertPosition = 0f;

                for (int i = 0; i < animationSteps.Length; i++)
                {
                    AnimationStepBase step = animationSteps[i];
                    if (step == null || step.IsMuted || !step.AddsToSequence)
                    {
                        if (timings != null)
                            timings[i] = (-1f, -1f);
                        continue;
                    }

                    float insertPosition = step.FlowType == FlowType.Join ? lastInsertPosition : sequenceEnd;
                    lastInsertPosition = insertPosition;

                    float length = Mathf.Max(0f, step.GetEstimatedLength());
                    float start = insertPosition + Mathf.Min(Mathf.Max(0f, step.Delay), length);
                    float end = insertPosition + length;

                    if (timings != null)
                        timings[i] = (start, end);

                    sequenceEnd = Mathf.Max(sequenceEnd, end);
                }

                return sequenceEnd;
            }
            finally
            {
                estimatingControllers.Remove(this);
            }
        }

        public bool TryGetStepAtIndex<T>(int index, out T result) where T : AnimationStepBase
        {
            if (index < 0 || index > animationSteps.Length - 1)
            {
                result = null;
                return false;
            }

            result = animationSteps[index] as T;
            return result != null;
        }

        public void ReplaceTarget<T>(GameObject targetGameObject) where T : GameObjectAnimationStep
        {
            for (int i = animationSteps.Length - 1; i >= 0; i--)
            {
                AnimationStepBase animationStepBase = animationSteps[i];
                if (animationStepBase == null)
                    continue;

                if (animationStepBase is not T gameObjectAnimationStep)
                    continue;

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
            for (int i = animationSteps.Length - 1; i >= 0; i--)
            {
                AnimationStepBase animationStepBase = animationSteps[i];
                if (animationStepBase == null)
                    continue;
                
                if(animationStepBase is not GameObjectAnimationStep gameObjectAnimationStep)
                    continue;

                if (gameObjectAnimationStep.Target == originalTarget)
                    gameObjectAnimationStep.SetTarget(newTarget);
            }
        }

#if UNITASK_ENABLED
        public UniTask PlayAsync()
        {
            return PlayAsync(CancellationToken.None);
        }

        public async UniTask PlayAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Play();
            Sequence sequence = playingSequence;
            Func<bool> finished = TrackFinished(sequence);

            try
            {
                await WaitForSequenceEnd(sequence, finished, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                KillIfStillPlaying(sequence);
                throw;
            }
        }

        public async UniTask<bool> TryPlayAsync(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
                return false;

            Play();
            Sequence sequence = playingSequence;

            Func<bool> finished = TrackFinished(sequence);

            try
            {
                await WaitForSequenceEnd(sequence, finished, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                KillIfStillPlaying(sequence);
                return false;
            }

            return finished();
        }

        private UniTask WaitForSequenceEnd(Sequence sequence, Func<bool> finished, CancellationToken cancellationToken)
        {
            return UniTask.WaitUntil(() => IsSequenceOver(sequence, finished), cancellationToken: cancellationToken);
        }

        private void KillIfStillPlaying(Sequence sequence)
        {
            if (playingSequence == sequence)
                Kill();
            else if (sequence.IsActive())
                sequence.Kill();
        }
#endif
    }
}
#endif