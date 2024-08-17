using UnityEngine;

namespace Beanc16.Common.General
{
    public class Follow : MonoBehaviour
    {
        [SerializeField]
        protected GameObject objToFollow;

        public GameObject ObjectToFollow
        {
            get => objToFollow;
            set
            {
                objToFollow = value;
            }
        }

        private void FixedUpdate()
        {
            this.transform.position = objToFollow.transform.position;
        }
    }
}
