using Dictionary;
using Serializable;
using UniRx;

namespace Battle.AI
{
    interface IAiInterface
    {
        AiSelectSkillResultSerializableData SelectSkill(int uniqId);
    }
}