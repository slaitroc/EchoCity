using System.Collections.Generic;
using UnityEngine;

namespace EchoCity
{
    public static class MaterialSwitcher
    {
        private class StaticLog { }
        private static Dictionary<Renderer, Material[]> _originalMaterials;
        private static bool _hasStoredMaterials;

        public static void StoreOriginalMaterials()
        {
            _originalMaterials = new Dictionary<Renderer, Material[]>();

            var renderers = Object.FindObjectsByType<Renderer>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );
            foreach (var r in renderers)
            {
                if (r == null) continue;
                _originalMaterials[r] = r.sharedMaterials;
            }

            _hasStoredMaterials = true;
        }

        public static void ApplyOverrideMaterial(Material overrideMaterial)
        {
            if (!_hasStoredMaterials)
                StoreOriginalMaterials();

            if (overrideMaterial == null)
            {
                Log.ELazy<StaticLog>(() => "overrideMaterial is NULL!");
                return;
            }

            foreach (var kvp in _originalMaterials)
            {
                var r = kvp.Key;
                if (r == null) continue;

                var newMats = new Material[r.sharedMaterials.Length];
                for (var i = 0; i < newMats.Length; i++)
                    newMats[i] = overrideMaterial;

                r.sharedMaterials = newMats;
            }
        }

        public static void RestoreOriginalMaterials()
        {
            if (!_hasStoredMaterials) return;

            foreach (var kvp in _originalMaterials)
            {
                var r = kvp.Key;
                var mats = kvp.Value;

                if (r != null)
                    r.sharedMaterials = mats;
            }

            _originalMaterials.Clear();
            _hasStoredMaterials = false;
        }
    }
}
