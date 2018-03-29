using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR;

namespace Assets.Vendors.Universal_XR.Teleporter
{
    public class SetCorrectHeight : MonoBehaviour
    {
        [SerializeField]
        private Transform m_TrackingSpace = null;
        [SerializeField]
        private TrackingSpaceType m_TrackingSpaceType = TrackingSpaceType.RoomScale;

        private void Start()
        {
            if (!XRSettings.enabled || m_TrackingSpace == null)
            {
                Destroy(this);
                return;
            }

            XRDevice.SetTrackingSpaceType(m_TrackingSpaceType);

            if (m_TrackingSpaceType == TrackingSpaceType.Stationary)
                m_TrackingSpace.localPosition = Vector3.zero;
        }
    }
}
