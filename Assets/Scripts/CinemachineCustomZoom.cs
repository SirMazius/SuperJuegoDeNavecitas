using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineCustomZoom : CinemachineExtension
{
    public float lerpSpeed;
    public float speedMult;
    public float a, b, c;
    class VcamExtraState
    {
        public Vector3 m_previousFramePos;
        public float m_previousFrameSize;
    }

    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        // Cada camara virtual vendra con un estado extra (VcamExtraState).
        VcamExtraState extra = GetExtraState<VcamExtraState>(vcam);
        // Esto significa que acabamos de empezar entonces inicializamos las variables.
        if (deltaTime < 0)
        {
            extra.m_previousFramePos = state.RawPosition;
            extra.m_previousFrameSize = state.Lens.OrthographicSize;
        }

        if (stage == CinemachineCore.Stage.Body) // TODO: Limpiar esto.
        {
            Vector3 targetPos = Vector3.Lerp(extra.m_previousFramePos, state.RawPosition, lerpSpeed);
            extra.m_previousFramePos = targetPos;
            //float scalatedMultiplier = state.Lens.OrthographicSize  > extra.m_previousFrameSize ? speedMult : 1.0f / speedMult;
            float targetSize = Mathf.Lerp(extra.m_previousFrameSize, state.Lens.OrthographicSize, lerpSpeed);
            // Debug.Log(Mathf.Lerp(a, b, c));
            // Debug.Log("Previous " + extra.m_previousFrameSize + " NonScalated " + state.Lens.OrthographicSize + " Scalated " + state.Lens.OrthographicSize  * scalatedMultiplier);
            LensSettings lens = state.Lens;
            //CinemachineFramingTransposer vcamBody = vcam.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>();
            lens.OrthographicSize = extra.m_previousFrameSize = targetSize; // Mathf.Clamp(targetSize, vcamBody.m_MinimumOrthoSize, vcamBody.m_MaximumOrthoSize);
            state.Lens = lens;
        }
    }
}
