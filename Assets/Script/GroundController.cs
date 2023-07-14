using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Script
{
    public class GroundController : MonoBehaviour
    {
        public List<GameObject> groundPrefabs;
        public List<GameObject> foregrounds;
        public PlayerController player;
        public float foregroundSpeedMultiplier = 3F;
        public ProgressBarBehavior progressBar;
        public int stageNumber;

        private float _groundWidth;
        private float _totalGroundWidth;
        private float _currentDistance = 0;
        private List<GameObject> _groundInstances;

        private void Start()
        {
            _groundInstances = new List<GameObject>();
            while (_groundInstances.Count < stageNumber)
            {
                int randomInt = UnityEngine.Random.Range(0, groundPrefabs.Count - 1);
                GameObject ground = Instantiate(groundPrefabs[randomInt], transform);
                ground.transform.position = new Vector3(_totalGroundWidth, 0, 0);
                // 여기서 array 안의 토탈 길이를 계산
                _totalGroundWidth += ground.transform.Find("Ground").GetComponent<SpriteRenderer>().bounds.size.x;
                _groundInstances.Add(ground);
            }

            // 첫번째 array에 있는 element 의 길이를 저장
            _groundWidth = _groundInstances[0].transform.Find("Ground").GetComponent<SpriteRenderer>().bounds.size.x;

            foreach (GameObject foreground in foregrounds)
            {
                Sequence sequence = DOTween.Sequence();
                sequence.Append(foreground.transform.DOMoveX(-_groundWidth, player.speed).SetEase(Ease.Linear));
                sequence.Append(foreground.transform.DOMoveX(foreground.transform.position.x, 0));
                sequence.SetLoops(-1);
            }
            progressBar.SetMaxValue(_totalGroundWidth);

        }

        private void Update()
        {
            foreach (GameObject ground in _groundInstances)
            {
                ground.transform.position -= new Vector3(player.speed * Time.deltaTime, 0, 0);
            }

            _currentDistance += player.speed * Time.deltaTime;
            progressBar.SetValue(_currentDistance);
        }
    }
}