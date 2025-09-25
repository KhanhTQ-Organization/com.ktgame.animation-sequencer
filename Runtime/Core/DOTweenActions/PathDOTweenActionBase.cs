using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Core.PathCore;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace com.ktgame.animation_sequencer
{
    [Serializable]
    public abstract class PathDOTweenActionBase : DOTweenActionBase
    {
        public override Type TargetComponentType => typeof(Transform);

        [SerializeField] protected bool _isLocal;
        public bool IsLocal
        {
            get => _isLocal;
            set => _isLocal = value;
        }

        [SerializeField] private Color _gizmoColor;
        public Color GizmoColor
        {
            get => _gizmoColor;
            set => _gizmoColor = value;
        }

        [SerializeField] private int _resolution = 10;
        public int Resolution
        {
            get => _resolution;
            set => _resolution = value;
        }

        [SerializeField] private PathMode _pathMode = PathMode.Full3D;
        public PathMode PathMode
        {
            get => _pathMode;
            set => _pathMode = value;
        }

        [SerializeField] private PathType _pathType = PathType.CatmullRom;
        public PathType PathType
        {
            get => _pathType;
            set => _pathType = value;
        }

        private Transform _previousTarget;
        private Vector3 _previousPosition;

        protected override Tweener GenerateTween_Internal(GameObject target, float duration)
        {
            TweenerCore<Vector3, Path, PathOptions> tween;

            _previousTarget = target.transform;
            if (!_isLocal)
            {
                tween = target.transform.DOPath(GetPathPositions(), duration, _pathType, _pathMode, _resolution, _gizmoColor);
                _previousPosition = target.transform.position;
            }
            else
            {
                tween = target.transform.DOLocalPath(GetPathPositions(), duration, _pathType, _pathMode, _resolution, _gizmoColor);
                _previousPosition = target.transform.localPosition;
            }

            return tween;
        }


        protected abstract Vector3[] GetPathPositions();
        public override void ResetToInitialState()
        {
            if (_previousTarget == null)
            {
                return;
            }

            if (_isLocal)
            {
                _previousTarget.transform.localPosition = _previousPosition;
            }
            else
            {
                _previousTarget.transform.position = _previousPosition;
            }
        }
        
    }
}