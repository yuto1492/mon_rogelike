using System.Collections.Generic;
using Item.Logic;
using Modal.DungeonEventModal;
using Model.Person;
using Model.Structs;
using UniRx;

namespace Dungeon.Logic
{
    public class DungeonEventLogic
    {
        public static void TreasureEvent(AsyncSubject<Unit> subject)
        {
            var eventModal = new DungeonEventModal();
            eventModal.EventText("何かを見つけた");
            eventModal.AddChoice("獲得する").Subscribe(_ =>
            {
                subject.OnNext(Unit.Default);
                subject.OnCompleted();
            });
            var loots = LootLogic.DungeonTreasureLootChoice();
            List<ItemStruct> items = new List<ItemStruct>();
            loots.ForEach(loot =>
            {
                var item = ItemLogic.CreateItemFromLoot(loot);
                items.Add(item);
            });
            eventModal.AddLootImages(loots);
            StockItemDataModel.GetInstance().Data.AddRange(items);
        }

    }
}