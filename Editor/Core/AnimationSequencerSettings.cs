using UnityEditor;
using UnityEngine;

namespace com.ktgame.animation_sequencer.editor
{
	public sealed class AnimationSequencerSettings : ScriptableObjectForPreferences<AnimationSequencerSettings>
	{
		public bool AutoHideStepsWhenPreviewing => _autoHideStepsWhenPreviewing;
		public bool DrawTimingsWhenPreviewing   => _drawTimingsWhenPreviewing;
		
		[SerializeField] private bool _autoHideStepsWhenPreviewing = true;
		[SerializeField] private bool _drawTimingsWhenPreviewing = true;

		[SettingsProvider]
		private static SettingsProvider SettingsProvider()
		{
			return CreateSettingsProvider("Animation Sequencer", null);
		}
	}
}