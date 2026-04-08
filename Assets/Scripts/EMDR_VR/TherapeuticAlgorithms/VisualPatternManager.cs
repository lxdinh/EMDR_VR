using System.Collections;
using UnityEngine;

namespace EMDR_VR.TherapeuticAlgorithms
{
    [DisallowMultipleComponent]
    public sealed class VisualPatternManager : MonoBehaviour
    {
        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        private static readonly int ColorId = Shader.PropertyToID("_Color");

        [Header("Target")]
        [SerializeField]
        private Renderer envRenderer;

        [Header("Patterns")]
        [SerializeField]
        private Material[] calmMats = System.Array.Empty<Material>();

        [Header("Transition")]
        [SerializeField]
        [Min(0.01f)]
        private float blendTime = 2f;

        [SerializeField]
        private AnimationCurve blendCurve;

        private MaterialPropertyBlock block;
        private Coroutine blendRoutine;
        private Color currentTint;

        private void Awake()
        {
            block = new MaterialPropertyBlock();
            GrabColorFromEnvironment();
        }

        private void OnEnable()
        {
            if (envRenderer != null)
                GrabColorFromEnvironment();
        }

        public void TriggerCalmingPattern(int patternIndex)
        {
            if (envRenderer == null)
            {
                Debug.LogWarning($"{nameof(VisualPatternManager)}: Assign environment renderer.", this);
                return;
            }

            if (calmMats == null || calmMats.Length == 0)
            {
                Debug.LogWarning($"{nameof(VisualPatternManager)}: Assign at least one calming material.", this);
                return;
            }

            if (patternIndex < 0 || patternIndex >= calmMats.Length)
            {
                Debug.LogWarning($"{nameof(VisualPatternManager)}: patternIndex {patternIndex} out of range (0..{calmMats.Length - 1}).", this);
                return;
            }

            Material source = calmMats[patternIndex];
            if (source == null)
            {
                Debug.LogWarning($"{nameof(VisualPatternManager)}: calmMats[{patternIndex}] is null.", this);
                return;
            }

            Color target = GetColorFromMaterial(source);
            StartBlend(target);
        }

        public void TriggerCalmingColor(Color targetColor)
        {
            if (envRenderer == null)
            {
                Debug.LogWarning($"{nameof(VisualPatternManager)}: Assign environment renderer.", this);
                return;
            }

            StartBlend(targetColor);
        }

        private void StartBlend(Color targetColor)
        {
            if (blendRoutine != null)
            {
                StopCoroutine(blendRoutine);
                blendRoutine = null;
            }

            envRenderer.GetPropertyBlock(block);
            if (block.HasColor(BaseColorId) || block.HasColor(ColorId))
                currentTint = ReadColorFromBlock(block);
            else if (envRenderer.sharedMaterial != null)
                currentTint = GetColorFromMaterial(envRenderer.sharedMaterial);
            else
                currentTint = Color.gray;

            blendRoutine = StartCoroutine(BlendRoutine(currentTint, targetColor));
        }

        private IEnumerator BlendRoutine(Color from, Color to)
        {
            float duration = blendTime;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float u = Mathf.Clamp01(elapsed / duration);
                if (blendCurve != null && blendCurve.keys.Length > 0)
                    u = blendCurve.Evaluate(u);

                Color blended = Color.LerpUnclamped(from, to, u);
                ApplyColorToEnvironment(blended);
                yield return null;
            }

            ApplyColorToEnvironment(to);
            currentTint = to;
            blendRoutine = null;
        }

        private void GrabColorFromEnvironment()
        {
            if (envRenderer == null)
                return;

            envRenderer.GetPropertyBlock(block);
            if (block.HasColor(BaseColorId) || block.HasColor(ColorId))
                currentTint = ReadColorFromBlock(block);
            else if (envRenderer.sharedMaterial != null)
                currentTint = GetColorFromMaterial(envRenderer.sharedMaterial);
            else
                currentTint = Color.gray;
        }

        private void ApplyColorToEnvironment(Color color)
        {
            envRenderer.GetPropertyBlock(block);
            if (block.HasColor(BaseColorId))
                block.SetColor(BaseColorId, color);
            else if (block.HasColor(ColorId))
                block.SetColor(ColorId, color);
            else
            {
                block.SetColor(BaseColorId, color);
                block.SetColor(ColorId, color);
            }

            envRenderer.SetPropertyBlock(block);
        }

        private static Color ReadColorFromBlock(MaterialPropertyBlock block)
        {
            if (block.HasColor(BaseColorId))
                return block.GetColor(BaseColorId);
            if (block.HasColor(ColorId))
                return block.GetColor(ColorId);
            return Color.gray;
        }

        private static Color GetColorFromMaterial(Material material)
        {
            if (material.HasProperty(BaseColorId))
                return material.GetColor(BaseColorId);
            if (material.HasProperty(ColorId))
                return material.GetColor(ColorId);
            return Color.gray;
        }
    }
}
