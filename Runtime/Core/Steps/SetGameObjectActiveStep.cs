using System;
using DG.Tweening;
using UnityEngine;

namespace com.ktgame.animation_sequencer
{
	[Serializable]
	public sealed class SetGameObjectActiveStep : AnimationStepBase
	{
		public override string DisplayName => "Set Game Object Active";

		[SerializeField] private GameObject _targetGameObject;
		public GameObject TargetGameObject
		{
			get => _targetGameObject;
			set => _targetGameObject = value;
		}

		[SerializeField] private bool _active;
		public bool Active
		{
			get => _active;
			set => _active = value;
		}

		private bool _wasActive;

		public override void AddTweenToSequence(Sequence animationSequence)
		{
			_wasActive = _targetGameObject.activeSelf;
			if (_wasActive == _active)
			{
				return;
			}

			Sequence behaviourSequence = DOTween.Sequence();
			behaviourSequence.SetDelay(Delay);

			behaviourSequence.AppendCallback(() =>
			{
				_targetGameObject.SetActive(_active);
			});
			if (FlowType == FlowType.Join)
			{
				animationSequence.Join(behaviourSequence);
			}
			else
			{
				animationSequence.Append(behaviourSequence);
			}
		}

		public override void ResetToInitialState()
		{
			_targetGameObject.SetActive(_wasActive);
		}

		public override string GetDisplayNameForEditor(int index)
		{
			string display = "NULL";
			if (_targetGameObject != null)
			{
				display = _targetGameObject.name;
			}

			return $"{index}. Set {display} Active: {_active}";
		}    
	}
}