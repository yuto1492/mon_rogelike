using UnityEngine;

namespace Extensions
{
    public static class TransformExtensions
    {
        public static void SetPos(this Transform transform, float x, float y, float z)
        {
            transform.position = new Vector3(x, y, z);
        }

        public static void AddPos(this Transform transform, float x, float y, float z)
        {
            transform.position = new Vector3(transform.position.x + x,
                transform.position.y + y, transform.position.z + z);
        }

        public static void SetPosX(this Transform transform, float x)
        {
            transform.position = new Vector3(x, transform.position.y, transform.position.z);
        }

        public static void SetPosY(this Transform transform, float y)
        {
            transform.position = new Vector3(transform.position.x, y, transform.position.z);
        }

        public static void SetPosZ(this Transform transform, float z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, z);
        }

        public static void AddPosX(this Transform transform, float x)
        {
            transform.position = new Vector3(transform.position.x + x, transform.position.y, transform.position.z);
        }

        public static void AddPosY(this Transform transform, float y)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + y, transform.position.z);
        }

        public static void AddPosZ(this Transform transform, float z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + z);
        }
    }
}