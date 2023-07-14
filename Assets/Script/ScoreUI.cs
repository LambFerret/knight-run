using System.Collections;
using TMPro;
using UnityEngine;

namespace Script
{
    public class ScoreUI : MonoBehaviour, IPlayerObserver
    {
        public TextMeshProUGUI text;
        public TextMeshProUGUI redText;

        public void Awake()
        {
            PlayerController player = GameObject.Find("Player").GetComponent<PlayerController>();
            player.AddObserver(this);
            text.text =  player.score.ToString();
        }

        private IEnumerator ScoreChangedValueText(int value)
        {
            redText.gameObject.SetActive(true);
            redText.text = value.ToString();
            yield return new WaitForSeconds(1f);
            redText.text = "";
            redText.gameObject.SetActive(false);
        }

        public void OnScoreChange(int score, int value)
        {
            StartCoroutine(ScoreChangedValueText(value));
            text.text = score.ToString();
        }

        public void OnLifeChange(int life, int value)
        {
        }
    }
}