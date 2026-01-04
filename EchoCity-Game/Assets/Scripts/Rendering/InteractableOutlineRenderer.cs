using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace EchoCity
{
    public static class InteractableOutlineRenderer
    {
        private static readonly HashSet<Renderer> HoveredRenderers = new();

        public static void Register(IEnumerable<Renderer> renderers)
        {
            if (renderers == null) return;
            foreach (var renderer in renderers)
                if (renderer != null)
                    HoveredRenderers.Add(renderer);
        }

        public static void Unregister(IEnumerable<Renderer> renderers)
        {
            if (renderers == null) return;
            foreach (var renderer in renderers)
                HoveredRenderers.Remove(renderer);
        }

        public static void ForEachActive(LayerMask mask, System.Action<Renderer> action)
        {
            if (action == null || HoveredRenderers.Count == 0) return;

            var toRemove = ListPool<Renderer>.Get();
            foreach (var r in HoveredRenderers)
            {
                if (r == null || !r.enabled || !r.gameObject.activeInHierarchy)
                {
                    toRemove.Add(r);
                    continue;
                }

                if ((mask.value & (1 << r.gameObject.layer)) != 0)
                    action.Invoke(r);
            }

            foreach (var r in toRemove)
                HoveredRenderers.Remove(r);

            ListPool<Renderer>.Release(toRemove);
        }

        public static void SetHovered(IEnumerable<Renderer> renderers)
        {
            HoveredRenderers.Clear();
            if (renderers == null) return;
            foreach (var r in renderers)
                if (r != null)
                    HoveredRenderers.Add(r);
        }

        public static void ClearHovered() => HoveredRenderers.Clear();
    }
}
