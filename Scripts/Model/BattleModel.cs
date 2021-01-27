using UniRx;

namespace Model
{
    public class BattleModel
    {
        public int ActiveUniqId;
        public Subject<Unit> NextTurn = new Subject<Unit>();
    }
}