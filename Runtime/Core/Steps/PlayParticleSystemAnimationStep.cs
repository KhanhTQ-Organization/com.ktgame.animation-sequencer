using System;
using DG.Tweening;
using UnityEngine;

namespace com.ktgame.animation_sequencer
{
	[Serializable]
	public sealed class PlayParticleSystemAnimationStep : AnimationStepBase
	{
		[SerializeField] private ParticleSystem _particleSystem;
		public ParticleSystem ParticleSystem
		{
			get => _particleSystem;
			set => _particleSystem = value;
		}

		[SerializeField] private float _duration = 1;
		public float Duration
		{
			get => _duration;
			set => _duration = value;
		}

		[SerializeField] private bool _stopEmittingWhenOver;
		public bool StopEmittingWhenOver
		{
			get => _stopEmittingWhenOver;
			set => _stopEmittingWhenOver = value;
		}

		public override string DisplayName => "Play Particle System";

		public override void AddTweenToSequence(Sequence animationSequence)
		{
			animationSequence.SetDelay(Delay);
			animationSequence.AppendCallback(() =>
			{
				_particleSystem.Play();
			});
            
			animationSequence.AppendInterval(_duration);
			animationSequence.AppendCallback(FinishParticles);
		}

		public override void ResetToInitialState()
		{
		}

		private void FinishParticles()
		{
			if (_stopEmittingWhenOver)
			{
				_particleSystem.Stop();
			}
		}

		public void SetTarget(ParticleSystem newTarget)
		{
			_particleSystem = newTarget;
		}

		public override string GetDisplayNameForEditor(int index)
		{
			string display = "NULL";
			if (_particleSystem != null)
			{
				display = _particleSystem.name;
			}

			return $"{index}. Play {display} particle system";
		}

	}
}