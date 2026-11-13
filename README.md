# com.ktgame.animation-sequencer

Visual tool to build sequences of tweens (DOTween) in the Inspector and preview them in edit mode.

Fork of [Animation Sequencer](https://github.com/brunomikoski/Animation-Sequencer) by Bruno Mikoski (MIT, see `LICENSE`). The namespace `BrunoMikoski.AnimationSequencer` is kept so existing prefabs and code keep working.

## Requirements

- DOTween with its `asmdef` created (DOTween Utility Panel > Setup DOTween > Create ASMDEF).
- The `DOTWEEN_ENABLED` scripting define. It is added automatically on every common platform (see Editor); without it the package compiles to nothing on that platform.
- Optional: TextMeshPro actions (`TMP Text`, `TMP Fade`) are enabled automatically with `com.unity.textmeshpro` or `com.unity.ugui` 2.0+ (Unity 6).
- Optional: `PlayAsync` needs `com.ktgame.unitask`.

## Usage

Add `UI/Animation Sequencer Controller` to a GameObject, add steps in the Inspector and press Play to preview.

```csharp
using BrunoMikoski.AnimationSequencer;

[SerializeField] private AnimationSequencerController showAnimation;

// Play with a one-shot completion callback (runs once, then is removed)
showAnimation.Play(() => Debug.Log("Shown"));

// Await (UniTask). Cancelling kills the sequence and throws OperationCanceledException.
await showAnimation.PlayAsync(destroyCancellationToken);

// Control
showAnimation.PlayForward();
showAnimation.PlayBackwards();
showAnimation.SetProgress(0.5f);
showAnimation.Pause();
showAnimation.Kill();      // also kills a paused sequence
showAnimation.Complete();

// Reuse the same animation on another object
showAnimation.ReplaceTargets(originalTarget, newTarget);

// Await without exceptions: true if it reached the end, false if killed / replaced / cancelled
bool finished = await showAnimation.TryPlayAsync(destroyCancellationToken);

// Replay the same sequence from the start (no new tweens when AutoKill is off)
showAnimation.SetAutoKill(false);
showAnimation.Restart();

// Change speed while playing
showAnimation.SetPlaybackSpeed(1.5f);

// React when each step starts (subscribe before Play)
showAnimation.StepStarted += (index, step) => Debug.Log($"Step {index} started");

// Duration without generating tweens
float seconds = showAnimation.EstimateDuration();
```

## Editor

- Each step header shows its estimated start → end time, and the Steps header shows the total duration.
- Configuration issues (missing target, target without the component an action needs, no actions, missing step type, Play Sequence pointing to itself) are shown as a red badge; hover it to see the details.
- Tick **Muted** on a step to skip it without deleting it (muted steps are not reset either).
- `DOTWEEN_ENABLED` is added automatically for Standalone, Android, iOS, WebGL and the selected platform when DOTween (with its asmdef) is in the project.

## Notes

- Callbacks passed to `Play`, `PlayForward` and `PlayBackwards` run **once** when the sequence finishes. If the sequence is replaced (calling `Play` again) before finishing, the pending callback is dropped. Listeners added to `OnFinishedEvent` directly still run every time.
- After an auto-killed sequence finished, `PlayForward`, `PlayBackwards`, `SetProgress` and `SetTime` generate a new sequence.
- A `Play Sequence` step that points back to its own controller (directly or through another controller) is skipped with an error instead of overflowing the stack.

## Changes compared to the original 1.0.x fork

- Fixed: completion callbacks accumulated and ran again on every later play.
- Fixed: actions kept animating the previous target after `ReplaceTargets` / `SetTarget`.
- Fixed: TextMeshPro actions were missing on Unity 6 (`com.unity.ugui` 2.0).
- Fixed: `Play Particle System` step delayed the whole sequence and ignored `Join`.
- Fixed: `PlayForward` / `PlayBackwards` / `SetProgress` did nothing after an auto-killed sequence finished.
- Fixed: `Kill()` ignored paused sequences.
- Fixed: `Play()` with Play Type `Backward` (and autoplay) did nothing: no events and `PlayAsync` / `TryPlayAsync` / `PlayEnumerator` never finished.
- Fixed: `Set Game Object Active` threw without a target. It now always adds itself (its delay always counts) and checks the active state when it runs.
- Fixed: missing steps/actions (renamed classes), missing target or sequencer threw exceptions.
- `PlayAsync(CancellationToken)` no longer depends on a coroutine on the controller GameObject.
- `Update` only exists in the Editor (progress preview), not in builds.
- `SetPlayOnAwake` is marked obsolete (it never had an effect); use `SetAutoplayMode`.
