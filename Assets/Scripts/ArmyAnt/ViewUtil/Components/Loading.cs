using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArmyAnt.ViewUtil.Components
{
    public class Loading : MonoBehaviour
    {
        // Update is called once per frame
        void Update()
        {
            roundingRect.Rotate(0, 0, Time.deltaTime);
        }

        public RectTransform roundingRect;
    }

}
