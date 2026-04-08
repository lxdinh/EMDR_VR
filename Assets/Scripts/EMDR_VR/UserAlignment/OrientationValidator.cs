using System;
using UnityEngine;
using UnityEngine.Events;

namespace EMDR_VR.UserAlignment
{
    [DisallowMultipleComponent]
    public sealed class OrientationValidator : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private Transform headsetForward;

        [SerializeField]
        private Transform therapeuticZone;

        [Header("Sampling")]
        [SerializeField]
        [Min(0.02f)]
        private float sampleEverySeconds = 0.1f;

        [Header("Alignment")]
        [SerializeField]
        [Range(-1f, 1f)]
        private float minForwardDot = 0.96f;

        [Header("Events")]
        public UnityEvent OnAligned;
        public UnityEvent OnLookedAway;

        public event Action Aligned;
        public event Action LookedAway;

        private float sampleTimer;

        private void Reset()
        {
            if (headsetForward == null && Camera.main != null)
                headsetForward = Camera.main.transform;
        }

        private void Awake()
        {
            if (headsetForward == null && Camera.main != null)
                headsetForward = Camera.main.transform;
        }

        private void Update()
        {
            if (headsetForward == null || therapeuticZone == null)
                return;

            sampleTimer += Time.deltaTime;
            if (sampleTimer < sampleEverySeconds)
                return;

            sampleTimer = 0f;
            ValidateOrientation();
        }

        public bool IsAligned()
        {
            if (headsetForward == null || therapeuticZone == null)
                return false;

            float dot = Vector3.Dot(headsetForward.forward.normalized, therapeuticZone.forward.normalized);
            return dot >= minForwardDot;
        }

        private void ValidateOrientation()
        {
            bool isAligned = IsAligned();

            if (isAligned)
            {
                OnAligned?.Invoke();
                Aligned?.Invoke();
            }
            else
            {
                OnLookedAway?.Invoke();
                LookedAway?.Invoke();
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (headsetForward == null || therapeuticZone == null)
                return;

            Gizmos.color = Color.green;
            Gizmos.DrawLine(headsetForward.position, headsetForward.position + headsetForward.forward * 1.0f);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(therapeuticZone.position, therapeuticZone.position + therapeuticZone.forward * 1.0f);
        }
#endif
    }
}
