using UnityEngine;
using UnityEngine.UI;

namespace Script
{
    public class ProgressBarBehavior : MonoBehaviour
    {
        private Slider _progressBar;

        private void Start()
        {
            _progressBar = GetComponent<Slider>();
        }

        public void SetValue(float value)
        {
            _progressBar.value = value;
        }

        public void SetMaxValue(float value)
        {
            _progressBar.maxValue = value;
        }
    }
}