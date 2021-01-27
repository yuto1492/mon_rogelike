using UnityEngine;

namespace Dictionary
{
    public class MaterialDictionary
    {
        public static Material GetDefaultMaterial()
        {
#if UNITY_EDITOR
            Material spriteDefaultMaterial =
                UnityEditor.AssetDatabase.GetBuiltinExtraResource<Material>("Sprites-Default.mat");
#else
            Material spriteDefaultMaterial = Resources.GetBuiltinResource<Material>("Sprites-Default.mat");
#endif
            return spriteDefaultMaterial;
        }
    }
}