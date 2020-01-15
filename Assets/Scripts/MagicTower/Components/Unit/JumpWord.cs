using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MagicTower.Components.Unit
{
    public class JumpWord : MonoBehaviour
    {
        // Start is called before the first frame update
        private void Start()
        {
            totalTime = 0f;
        }

        // Update is called once per frame
        private void Update()
        {
            totalTime += Time.deltaTime;
            if (totalTime < jumpSeconds)
            {
                var jumpPercent = Time.deltaTime / jumpSeconds;
                if(Direction == Present.Player.Controller.Direction.Left)
                {
                    transform.localPosition += new Vector3(-jumpWidth * jumpPercent, jumpHeight * jumpPercent, 0);
                }
                else
                {
                    transform.localPosition += new Vector3(jumpWidth * jumpPercent, jumpHeight * jumpPercent, 0);
                }
            }
            if(totalTime < scaleSeconds)
            {
                var scale = scaleTimesFirst + (scaleTimesLast - scaleTimesFirst) * totalTime / scaleSeconds;
                transform.localScale = new Vector3(scale, scale, 0);
            }
            if(totalTime > showSeconds)
            {
                Destroy(gameObject);
            }
        }

        public string Word {
            get => txtWord.text;
            set => txtWord.text = value;
        }

        public Present.Player.Controller.Direction Direction {
            get;set;
        }

        public Text txtWord;

        private float totalTime;

        /// <summary> 纵向条约距离 </summary>
        private const float jumpHeight = 35f;
        /// <summary> 横向条约距离 </summary>
        private const float jumpWidth = 80f;
        /// <summary> 跳跃耗时 </summary>
        private const float jumpSeconds = 0.4f;
        /// <summary> 初始缩放 </summary>
        private const float scaleTimesFirst = 0.1f;
        /// <summary> 最终缩放 </summary>
        private const float scaleTimesLast = 0.8f;
        /// <summary> 缩放耗时 </summary>
        private const float scaleSeconds = 0.5f;
        /// <summary> 消失时间 </summary>
        private const float showSeconds = 0.7f;
    }

}
