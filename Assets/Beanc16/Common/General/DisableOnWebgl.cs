using UnityEngine;

namespace Beanc16.Common.General
{
    public class DisableOnWebgl : MonoBehaviour
    {
        private void Start()
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
