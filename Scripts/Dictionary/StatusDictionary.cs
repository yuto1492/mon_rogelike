using Consts;
using Serializable;
using UnityEditor;
using UnityEngine;

namespace Dictionary
{
    public class StatusDictionary
    {
        public static bool IsViewStatus(string type)
        {
            switch (type)
            {
                case StatusConsts.SLEEP:
                case StatusConsts.POISON:
                    return true;
            }

            return false;
        }

        public static bool IsValueView(string type)
        {
            switch (type)
            {
                case StatusConsts.POISON:
                    return true;
            }

            return false;
        }

        public static bool IsTurnStatus(string valueTarget)
        {
            switch (valueTarget)
            {
                case StatusConsts.POISON:
                case StatusConsts.SLEEP:
                    return true;
            }

            return false;
        }

        public static string StatusIconName(string type)
        {
            switch (type)
            {
                case StatusConsts.DEAD:
                    return "Death";
                case StatusConsts.POISON:
                    return "Poison";
                case StatusConsts.SLEEP:
                    return "Sleep";
            }

            return "";
        }

        public static Color StatusValueColor(string type)
        {
            switch (type)
            {
                case StatusConsts.POISON:
                    return Color.green;
            }

            return new Color(94, 255, 0);
        }

        public static bool IsSleep(BattlerSerializable battler)
        {
            if (battler.status.Find(x => x.type == StatusConsts.SLEEP) != null)
            {
                return true;
            }

            return false;
        }
        
    }
}