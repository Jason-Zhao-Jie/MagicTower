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
                transform.localPosition += new Vector3(0, jumpHeight * Time.deltaTime / jumpSeconds, 0);
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

        public Text txtWord;

        private float totalTime;

        /// <summary> 跳跃高度 </summary>
        private const float jumpHeight = 80f;
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
