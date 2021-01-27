using System.Collections.Generic;
using Serializable;

namespace Model.Master
{
    public sealed class MasterSkillDataModel
    {
        private static MasterSkillDataModel _instance = new MasterSkillDataModel();

        private List<SkillsSerializable> _data;

        public void CreateData(List<SkillsSerializable> data)
        {
            _data = data;
        }

        public List<SkillsSerializable> Data
        {
            get => _data;
        }
        
        public static MasterSkillDataModel Instance => _instance;
    }
}