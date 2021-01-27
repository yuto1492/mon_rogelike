using Consts.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Modal.MonsterStatusModal
{
    public class MonsterStatusModalDescriptionBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public GameObject toObject;
        [TextArea]
        public string text;

        [SerializeField] public StatusTypeEnum.StatusType type;
        
        public void OnPointerEnter( PointerEventData eventData )
        {
            /*gameObject.GetComponent<TextMeshProUGUI>();
            MonsterStatusModalPresenter.GetInstance().ShowDescription(type);
            toObject.GetComponent<TextMeshProUGUI>().text = text;*/
        }

        public void OnPointerExit( PointerEventData eventData )
        {
        }
    }
}