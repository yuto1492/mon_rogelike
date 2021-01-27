using UnityEngine;

namespace Utils
{
    public static class MaterialUtils
    {
        public static Material CloneMaterial(Material material)
        {
            //対象のシェーダー情報を取得
            Shader sh = material.shader;
            //取得したシェーダーを元に新しいマテリアルを作成
            Material mat = new Material(sh);
            return mat;
        }
    }
}