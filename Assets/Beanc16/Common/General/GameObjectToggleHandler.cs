using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Beanc16.Common.General
{
    public class GameObjectToggleHandler : MonoBehaviour
    {
        [SerializeField]
        private GameObject gameObjectToToggle;



        public void Show()
        {
            this.gameObjectToToggle.SetActive(true);
        }

        public void Hide()
        {
            this.gameObjectToToggle.SetActive(false);
        }

        public void ToggleVisibility(bool shouldBeVisible)
        {
            this.gameObjectToToggle.SetActive(shouldBeVisible);
        }
    }
}
