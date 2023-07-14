using System.Collections;
using TMPro;
using UnityEngine;

namespace Script
{
    public class LifeUI : MonoBehaviour, IPlayerObserver
    {
        public TextMeshProUGUI text;
        public TextMeshProUGUI redText;

        public void Start()
        {
            PlayerController player = GameObject.Find("Player").GetComponent<PlayerController>();
            player.AddObserver(this);
            text.text = "Life : " + player.life;
        }

        public void OnLifeChange(int life, int value)
        {
            StartCoroutine(LifeChangedValueText(value));
            text.text = "Life : " + life;
        }

        private IEnumerator LifeChangedValueText(int value)
        {
            redText.gameObject.SetActive(true);
            redText.text = value.ToString();
            yield return new WaitForSeconds(1f);
            redText.text = "";
            redText.gameObject.SetActive(false);
        }

        public void OnScoreChange(int score, int value)
        {
        }
    }
}