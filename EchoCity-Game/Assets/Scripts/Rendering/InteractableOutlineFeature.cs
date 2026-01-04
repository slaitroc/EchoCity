using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace EchoCity
{
    public class InteractableOutlineFeature : ScriptableRendererFeature
    {
        private OutlinePass _pass;

        [Header("Outline Settings")]
        [SerializeField] private LayerMask outlineLayers = (1 << 6) | (1 << 9);
        [SerializeField] private Color outlineColor = Color.white;
        [SerializeField, Range(0.001f, 0.1f)] private float outlineThickness = 0.02f;

        private Material _material;

        public override void Create()
        {
            var shader = Shader.Find("Hidden/EchoCity/InteractableOutline");
            if (shader != null)
                _material = CoreUtils.CreateEngineMaterial(shader);

            _pass = new OutlinePass(outlineLayers, outlineColor, outlineThickness, _material)
            {
                renderPassEvent = RenderPassEvent.AfterRenderingTransparents
            };
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (_pass == null || _material == null) return;
            renderer.EnqueuePass(_pass);
        }

        private class OutlinePass : ScriptableRenderPass
        {
            private readonly LayerMask _outlineLayers;
            private readonly Color _outlineColor;
            private readonly float _outlineThickness;
            private readonly Material _material;

            private class PassData
            {
                public Material material;
                public Camera camera;
                public LayerMask outlineLayers;
                public Color outlineColor;
                public float outlineThickness;
            }

            public OutlinePass(LayerMask outlineLayers, Color outlineColor, float outlineThickness, Material material)
            {
                _outlineLayers = outlineLayers;
                _outlineColor = outlineColor;
                _outlineThickness = outlineThickness;
                _material = material;
            }

            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                if (_material == null) return;

                var cameraData = frameData.Get<UniversalCameraData>();
                var resources = frameData.Get<UniversalResourceData>();

                using var pass = renderGraph.AddRasterRenderPass<PassData>("Interactable Outline", out var passData);
                passData.material = _material;
                passData.camera = cameraData.camera;
                passData.outlineLayers = _outlineLayers;
                passData.outlineColor = _outlineColor;
                passData.outlineThickness = _outlineThickness;

                pass.SetRenderAttachment(resources.activeColorTexture, 0, AccessFlags.Write);
                pass.SetRenderAttachmentDepth(resources.activeDepthTexture, AccessFlags.Read);
                pass.AllowGlobalStateModification(true);
                pass.AllowPassCulling(false);
                pass.SetRenderFunc((PassData data, RasterGraphContext ctx) =>
                {
                    if (data.camera == null) return;
                    if (data.camera.cameraType is CameraType.Preview or CameraType.Reflection or CameraType.SceneView)
                        return;

                    data.material.SetColor("_OutlineColor", data.outlineColor);
                    data.material.SetFloat("_Thickness", data.outlineThickness);
                    ctx.cmd.SetViewProjectionMatrices(data.camera.worldToCameraMatrix, data.camera.projectionMatrix);
                    InteractableOutlineRenderer.ForEachActive(data.outlineLayers, r => ctx.cmd.DrawRenderer(r, data.material, 0, 0));
                });
            }
        }
    }
}
