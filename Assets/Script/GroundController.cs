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

        private float _groundWidth;
        private float _totalGroundWidth;
        private List<GameObject> _groundInstances;

        private void Awake()
        {
            _groundInstances = new List<GameObject>();
            foreach (GameObject groundInstance in groundPrefabs)
            {
                GameObject ground = Instantiate(groundInstance, transform);
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
                sequence.Append(foreground.transform.DOMoveX(-_groundWidth, player.speed / foregroundSpeedMultiplier).SetEase(Ease.Linear));
                sequence.Append(foreground.transform.DOMoveX(foreground.transform.position.x, 0));
                sequence.SetLoops(-1);

            }
        }

        private void Update()
        {
            // 무시
            foreach (GameObject ground in _groundInstances)
            {
                ground.transform.position -= new Vector3(player.speed * Time.deltaTime, 0, 0);
            }

            // 만일 array에서 첫번째 element의 x 좌표가 -_groundWidth 보다 작거나 같다면
            if (_groundInstances[0].transform.position.x <= -_groundWidth)
            {
                // 첫번째 element의 좌표를 토탈 길이를 참조하여 변경
                _groundInstances[0].transform.position = new Vector3
                (
                    _totalGroundWidth - _groundInstances[0].transform.position.x,
                    _groundInstances[0].transform.position.y,
                    _groundInstances[0].transform.position.z
                );
                // 첫번째 element를 array의 마지막 element로 옮김
                GameObject obj = _groundInstances[0];
                _groundInstances.RemoveAt(0);
                _groundInstances.Add(obj);
                // 0번째 element 길이 재저장
                _groundWidth = _groundInstances[0].transform.Find("Ground").GetComponent<SpriteRenderer>().bounds.size.x;
            }
        }
    }
}