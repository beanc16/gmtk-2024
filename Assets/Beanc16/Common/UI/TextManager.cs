using UnityEngine;
using TMPro;

namespace Beanc16.Common.UI
{
    [RequireComponent(typeof (TextMeshProUGUI))]
    public class TextManager : MonoBehaviour
    {
        protected TextMeshProUGUI textComponent;

        private void Awake()
        {
            InitializeComponents();
        }

        protected void InitializeComponents()
        {
            textComponent = GetComponent<TextMeshProUGUI>();
        }

        public void SetText(string text)
        {
            textComponent.text = text;
        }

        public void SetText(int number)
        {
            textComponent.text = number.ToString();
        }
    }
}
