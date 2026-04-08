using System.Collections;
using UnityEngine;

namespace EMDR_VR.TherapeuticAlgorithms
{
    [DisallowMultipleComponent]
    public sealed class BilateralStimulator : MonoBehaviour
    {
        [Header("Motion")]
        [SerializeField]
        [Min(0f)]
        private float swingWidth = 0.6f;

        [SerializeField]
        [Min(0.01f)]
        private float wobbleHz = 1f;

        [SerializeField]
        private Vector3 wobbleAxis = Vector3.right;

        [Header("Stop behavior")]
        [SerializeField]
        [Min(0.01f)]
        private float snapBackTime = 0.35f;

        [SerializeField]
        private AnimationCurve easeBackCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        private Vector3 homePos;
        private Vector3 slideDirection;
        private Coroutine moveRoutine;
        private bool isRunning;

        private void Awake()
        {
            homePos = transform.localPosition;
            slideDirection = wobbleAxis.sqrMagnitude > Mathf.Epsilon
                ? wobbleAxis.normalized
                : Vector3.right;
        }

        private void OnValidate()
        {
            if (wobbleAxis.sqrMagnitude < Mathf.Epsilon)
                wobbleAxis = Vector3.right;
        }

        public void ToggleStimulation(bool state)
        {
            if (state == isRunning)
                return;

            isRunning = state;

            if (moveRoutine != null)
            {
                StopCoroutine(moveRoutine);
                moveRoutine = null;
            }

            if (isRunning)
                moveRoutine = StartCoroutine(OscillateRoutine());
            else
                moveRoutine = StartCoroutine(ReturnToCenterRoutine());
        }

        public bool IsStimulationActive => isRunning;

        private IEnumerator OscillateRoutine()
        {
            float angularFrequency = wobbleHz * Mathf.PI * 2f;
            float halfWidth = swingWidth * 0.5f;

            while (isRunning)
            {
                float t = Time.time * angularFrequency;
                float offset = Mathf.Sin(t) * halfWidth;
                transform.localPosition = homePos + slideDirection * offset;
                yield return null;
            }

            moveRoutine = null;
        }

        private IEnumerator ReturnToCenterRoutine()
        {
            Vector3 start = transform.localPosition;
            float duration = snapBackTime;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float u = Mathf.Clamp01(elapsed / duration);
                float eased = easeBackCurve.Evaluate(u);
                transform.localPosition = Vector3.LerpUnclamped(start, homePos, eased);
                yield return null;
            }

            transform.localPosition = homePos;
            moveRoutine = null;
        }

        public void CaptureRestPositionFromCurrentTransform()
        {
            homePos = transform.localPosition;
        }
    }
}
