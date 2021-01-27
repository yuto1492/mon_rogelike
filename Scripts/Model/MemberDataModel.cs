using System.Collections.Generic;
using System.Linq;
using Serializable;

namespace Model
{
    /// <summary>
    /// 戦闘参加中の仲間、パーティメンバー
    /// </summary>
    public sealed class MemberDataModel
    {
        private static MemberDataModel _instance = new MemberDataModel();

        public static MemberDataModel Instance => _instance;

        ///////////////////////////////////
        /// メンバ
        ///////////////////////////////////
        private List<MemberSerializable> _data = new List<MemberSerializable>();

        public void Add(BattlerSerializable battler, int memberIndex = 0)
        {
            MemberSerializable member = new MemberSerializable();
            member.index = battler.index;
            member.memberIndex = memberIndex;
            member.uniqId = battler.uniqId;
            _data.Add(member);
            Sort();
        }

        private void Sort()
        {
            _data.Sort((a, b) => (b.memberIndex - a.memberIndex));
            foreach (var x in _data.Select((item, index) => new {item, index}))
            {
                x.item.memberIndex = x.index;
            }
        }

        public void Remove(int index)
        {
            _data.Remove(_data[index]);
        }

        /// <summary>
        /// メンバーのデータを取得
        /// </summary>
        /// <returns></returns>
        public List<BattlerSerializable> GetActorData()
        {
            ActorDataModel actorData = ActorDataModel.Instance;
            var data = actorData.Data.Where(x => GetMemberIndexs().Contains(x.index));
            return data.ToList();
        }

        public List<int> UniqIds()
        {
            List<int> uniqIds = new List<int>();
            _data.ForEach(x =>
            {
                uniqIds.Add(x.uniqId);
            });
            return uniqIds;
        }

        /// <summary>
        /// メンバーのアクターインデックスを取得する
        /// </summary>
        /// <returns></returns>
        public List<int> GetMemberIndexs()
        {
            List<int> list = new List<int>();
            _data.ForEach(x =>
            {
                list.Add(x.index);
            });
            return list;
        }

        ///////////////////////////////////
        /// プロパティ
        ///////////////////////////////////

        public List<MemberSerializable> Data
        {
            get => _data;
            set => _data = value;
        }
    }
}