using System.Security.Cryptography.X509Certificates;
using UnityEngine;

namespace Modal
{
    public class ModalBase
    {
        protected GameObject Modal;

        protected virtual void Close()
        {
            GameObject.Destroy(Modal);
            Modal = null;
        }
    }
}