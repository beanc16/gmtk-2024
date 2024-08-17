using UnityEngine;

namespace Beanc16.Common.General
{
    public class FollowParent : MonoBehaviour
    {
        private Transform ParentTransform
        {
            get => this.transform.parent;
        }

        private void FixedUpdate()
        {
            this.transform.position = ParentTransform.position;
        }
    }
}
