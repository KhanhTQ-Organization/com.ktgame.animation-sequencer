using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace com.ktgame.animation_sequencer
{
	[Serializable]
	public sealed class InvokeCallbackAnimationStep : AnimationStepBase
	{
		[SerializeField] private UnityEvent _callback = new UnityEvent();
		public UnityEvent Callback
		{
			get => _callback;
			set => _callback = value;
		}

		public override string DisplayName => "Invoke Callback";
        

		public override void AddTweenToSequence(Sequence animationSequence)
		{
			Sequence sequence = DOTween.Sequence();
			sequence.SetDelay(Delay);
			sequence.AppendCallback(() => _callback.Invoke());
            
			if (FlowType == FlowType.Append)
			{
				animationSequence.Append(sequence);
			}
			else
			{
				animationSequence.Join(sequence);
			}
		}

		public override void ResetToInitialState()
		{
		}

		public override string GetDisplayNameForEditor(int index)
		{
			string[] persistentTargetNamesArray = new string[_callback.GetPersistentEventCount()];
			for (int i = 0; i < _callback.GetPersistentEventCount(); i++)
			{
				if (_callback.GetPersistentTarget(i) == null)
				{
					continue;
				}

				if (string.IsNullOrWhiteSpace(_callback.GetPersistentMethodName(i)))
				{
					continue;
				}

				persistentTargetNamesArray[i] = $"{_callback.GetPersistentTarget(i).name}.{_callback.GetPersistentMethodName(i)}()";
			}
            
			var persistentTargetNames = $"{string.Join(", ", persistentTargetNamesArray).Truncate(45)}";
            
			return $"{index}. {DisplayName}: {persistentTargetNames}";
		}
	}
}