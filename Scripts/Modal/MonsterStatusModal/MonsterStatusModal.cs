using Modal.MonsterStatusModal.View;

namespace Modal.MonsterStatusModal
{
    public class MonsterStatusModal : ModalBase
    {
        private static MonsterStatusModal _singleton = new MonsterStatusModal();

        public static MonsterStatusModal GetInstance()
        {
            return _singleton;
        }

        private MonsterStatusModalView _view;

        MonsterStatusModal()
        {
            _view = new MonsterStatusModalView(Modal);
        }

        public void Open()
        {
            if (Modal == null)
            {
                Modal = _view.Open();
            }
            else
            {
                Modal.SetActive(true);
            }
        }

        protected override void Close()
        {
            Modal.SetActive(false);
        }
    }
}