using System.Collections.Generic;
using System.Linq.Expressions;
using Dictionary;
using Michsky.UI.ModernUIPack;
using Model;
using Serializable;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Modal.MonsterStatusModal.View
{
    public class MonsterStatusModalView
    {
        private GameObject _modal;
        private List<GameObject> _monsterSelectTabs;

        public MonsterStatusModalView(GameObject modal)
        {
            _modal = modal;
        }
        
        public GameObject Open()
        {
            _modal = Object.Instantiate(
                (GameObject) Resources.Load("Prefabs/Modal/MonsterStatus/MonsterStatusModal"));
            _modal.GetComponent<Canvas>().worldCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
            Refresh();
            return _modal;
        }

        private void Refresh()
        {
            _modal.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
            _modal.GetComponent<Canvas>().worldCamera = GameObject.Find("MainCamera").GetComponent<Camera>();

            var manager = _modal.transform.Find("Content/Content/MonsterStatusWindowManager")
                .GetComponent<WindowManager>();
            MemberDataModel.Instance.GetActorData().ForEach(battler =>
            {
                var button = CreateButton(battler);
                manager.windows.Add(CreateStatusWindow(button, battler));
                //button.transform.SetParent(buttons,false);
            });
        }

        private GameObject CreateButton(BattlerSerializable battler)
        {
            var buttons = _modal.transform.Find("Content/Content/MonsterStatusWindowManager/Buttons");
            var button =
                Object.Instantiate((GameObject) Resources.Load("Prefabs/Modal/MonsterStatus/MonsterButton"),
                    buttons);
            button.transform.Find("Normal/Text").GetComponent<TextMeshProUGUI>().text = battler.name;
            button.transform.Find("Pressed/Text").GetComponent<TextMeshProUGUI>().text = battler.name;
            var manager = _modal.transform.Find("Content/Content/MonsterStatusWindowManager")
                .GetComponent<WindowManager>();
            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                manager.OpenPanel(battler.uniqId.ToString());
            });
            return button;
        }

        private WindowManager.WindowItem CreateStatusWindow(GameObject button, BattlerSerializable battler)
        {
            var windows = _modal.transform.Find("Content/Content/MonsterStatusWindowManager/Windows");
            var statusWindow = Object.Instantiate(
                (GameObject) Resources.Load("Prefabs/Modal/MonsterStatus/MonsterStatus"),
                windows);
            WindowManager.WindowItem item = new WindowManager.WindowItem
            {
                buttonObject = button,
                windowName = battler.uniqId.ToString(),
                windowObject = statusWindow
            };
            statusWindow.GetComponent<MonsterStatus>().Refresh(battler);
            return item;
        }

    }
}