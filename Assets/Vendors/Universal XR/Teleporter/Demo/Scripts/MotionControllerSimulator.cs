using UnityEngine;
using UnityEngine.XR;

namespace Demonixis.UniversalXR.Demo
{
    public class MotionControllerSimulator : MonoBehaviour
    {
        private Transform m_Transform = null;

        [SerializeField]
        private bool m_RotateBody = false;

        private void Start()
        {
            m_Transform = transform;

            if (XRSettings.enabled)
                Destroy(this);
        }

        private void Update()
        {
            m_Transform.Rotate(-Input.GetAxis("Mouse Y"), 0, 0);

            if (m_RotateBody)
                m_Transform.root.Rotate(0, Input.GetAxis("Mouse X"), 0);
        }
    }
}