using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Script
{
    public class GroundController : MonoBehaviour
    {
        public SFXPlayer sfxPlayer;
        public List<GameObject> groundPrefabs;
        public PlayerController player;
        public float foregroundSpeedMultiplier = 3F;
        public ProgressBarBehavior progressBar;
        public int stageNumber;
        public List<GameObject> foreFront;
        public List<GameObject> foreBack;
        public GameObject background;

        private float _groundWidth;
        private float _totalGroundWidth;
        private float _currentDistance = 0;
        private float _foreFrontWidth;
        private float _foreBackWidth;
        private List<GameObject> _groundInstances;

        private void Start()
        {
            // sfxPlayer.Play("FireFly", 0.8F);
            _foreFrontWidth = foreFront[0].transform.GetComponent<SpriteRenderer>().bounds.size.x;
            _foreBackWidth = foreBack[0].transform.GetComponent<SpriteRenderer>().bounds.size.x;
            _groundInstances = new List<GameObject>();
            while (_groundInstances.Count < stageNumber)
            {
                int randomInt = UnityEngine.Random.Range(0, groundPrefabs.Count - 1);
                GameObject ground = Instantiate(groundPrefabs[randomInt], new Vector3(_totalGroundWidth, 0, -9.5f),
                    Quaternion.identity, transform);
                _totalGroundWidth += ground.transform.Find("Ground").GetComponent<SpriteRenderer>().bounds.size.x;
                _groundInstances.Add(ground);
            }

            _groundWidth = _groundInstances[0].transform.Find("Ground").GetComponent<SpriteRenderer>().bounds.size.x;

            progressBar.SetMaxValue(_totalGroundWidth);
        }

        private void Update()
        {
            Renderer rend = background.GetComponent<Renderer>();
            rend.material.mainTextureOffset += new Vector2(0.1f * Time.deltaTime, 0);
            Debug.Log(_groundInstances.Count);
            foreach (GameObject ground in _groundInstances)
            {
                ground.transform.position -= new Vector3(player.speed * Time.deltaTime, 0, 0);
            }

            foreach (var back in foreBack)
            {
                back.transform.position -= new Vector3(player.speed * Time.deltaTime * foregroundSpeedMultiplier, 0, 0);
                if (back.transform.position.x <= -_foreBackWidth)
                {
                    back.transform.position = new Vector3
                    (
                        _foreBackWidth - back.transform.position.x,
                        back.transform.position.y,
                        back.transform.position.z
                    );
                }
            }

            foreach (var front in foreFront)
            {
                front.transform.position -=
                    new Vector3(player.speed * Time.deltaTime * foregroundSpeedMultiplier * 1.1f, 0, 0);
                if (front.transform.position.x <= -_foreFrontWidth)
                {
                    front.transform.position = new Vector3
                    (
                        _foreFrontWidth - front.transform.position.x,
                        front.transform.position.y,
                        front.transform.position.z
                    );
                }
            }

            _currentDistance += player.speed * Time.deltaTime;
            progressBar.SetValue(_currentDistance);
        }
    }
}