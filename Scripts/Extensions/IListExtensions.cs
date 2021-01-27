using System.Collections.Generic;
namespace Extensions
{
    public static class IListExtensions
    {
        /// <summary>
        /// 指定されたインデックスに要素が存在する場合に true を返します
        /// </summary>
        public static bool IsDefinedAt<T>( IList<T> self, int index )
        {
            return index < self.Count;
        }
    }
}