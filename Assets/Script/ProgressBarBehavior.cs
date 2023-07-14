using UnityEngine;
using UnityEngine.SceneManagement;
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
            if (_progressBar.value >= _progressBar.maxValue)
            {
                SceneManager.LoadScene("Boss");
            }
        }

        public void SetMaxValue(float value)
        {
            _progressBar.maxValue = value;
        }
    }
}