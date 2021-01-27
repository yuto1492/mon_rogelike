using Consts;
using Consts.Enums;

namespace Dictionary
{
    public class GuiDictionary
    {
        /// <summary>
        /// GUI上で表示させる名前の色
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetBattlerNameColor(BattlerEnum.BattlerType type)
        {
            switch (type)
            {
                case BattlerEnum.BattlerType.Actor:
                    return GuiConsts.MEMBER_COLOR;
                case BattlerEnum.BattlerType.Enemy:
                    return GuiConsts.ENEMY_COLOR;
            }

            return GuiConsts.MEMBER_COLOR;
        }
    }
}