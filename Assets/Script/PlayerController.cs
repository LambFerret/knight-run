using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Script
{
    public class PlayerController : MonoBehaviour
    {
        public float jumpForce = 5f;
        public int life = 10;
        public float invincibleTime = 2f;
        public int score;
        public int speed;
        public int boostSpeedMultiplier;
        public float boostTime;
        public float boostAddTime;
        public GameObject playerPrefab;
        public GameObject defeatPanel;
        public GameObject emergencyPanel;
        public TextMeshProUGUI emergencyPanelText;
        public Animator animator;

        private List<IPlayerObserver> _observers = new List<IPlayerObserver>();
        private List<GameObject> players = new List<GameObject>();
        private int _initialSpeed;
        private int _boostSpeed;
        private bool _isJumping;
        private bool _isInvincible;
        private bool _haveOneMoreCoin = true;
        private bool _isBoostMode;
        private Rigidbody2D _rb;
        private GridLayoutGroup _lifeGroup;

        private void Start()
        {
            _initialSpeed = speed;
            _boostSpeed = speed * boostSpeedMultiplier;
            _lifeGroup = transform.Find("LifeContainer").GetComponent<GridLayoutGroup>();
            _rb = GetComponent<Rigidbody2D>();
            for (int i = 0; i < life; i++)
            {
                AddLifeIntoGrid();
            }
        }

        public void Jump()
        {
            if (!_isJumping)
            {
                _rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
                _isJumping = true;
                animator.SetBool("isJump", true);
            }
        }

        private IEnumerator Blink()
        {
            _isInvincible = true;
            yield return new WaitForSeconds(invincibleTime);
            _isInvincible = false;
        }


        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                animator.SetBool("isJump", false);
                _isJumping = false;
            }
        }

        public void AddObserver(IPlayerObserver observer)
        {
            _observers.Add(observer);
        }

        public void RemoveObserver(IPlayerObserver observer)
        {
            _observers.Remove(observer);
        }

        private void NotifyScoreChanged(int value)
        {
            foreach (IPlayerObserver observer in _observers)
            {
                observer.OnScoreChange(score, value);
            }
        }

        private void NotifyLifeChanged(int value)
        {
            foreach (IPlayerObserver observer in _observers)
            {
                observer.OnLifeChange(life, value);
            }
        }

        public void IncreaseScore(int amount)
        {
            score += amount;
            NotifyScoreChanged(amount);
        }

        public void IncreaseLife(int amount)
        {
            life += amount;
            NotifyLifeChanged(amount);
            AddLifeIntoGrid();
        }

        public void DecreaseLife(int damage)
        {
            life -= damage;
            NotifyLifeChanged(-damage);

            if (life <= 0)
            {
                StartCoroutine(Dead());
            }

            RemoveLifeFromGrid();
            StartCoroutine(Blink());
        }

        private IEnumerator Dead()
        {
            Time.timeScale = 0.1F;
            yield return new WaitForSeconds(0.25f);
            Time.timeScale = 1;
            CheckOneCoin();
        }

        private void CheckOneCoin()
        {
            if (_haveOneMoreCoin)
            {
                SetBoostMode();
            }
            else
            {
                GameOver();
            }
        }

        private void SetBoostMode()
        {
            _isInvincible = true;
            animator.SetBool("hasOneCoin", true);
            boostSpeedMultiplier = speed * 3;
            _haveOneMoreCoin = false;
            _isBoostMode = true;
        }

        private void GameOver()
        {
            animator.SetBool("hasOneCoin", false);
            Time.timeScale = 0;
            defeatPanel.SetActive(true);
        }

        private void Update()
        {
            if (life > 0) Time.timeScale = 1;
            if (_isBoostMode)
            {
                emergencyPanel.SetActive(true);
                emergencyPanelText.text = boostTime.ToString("0.0");
                speed = boostSpeedMultiplier;
                if (boostTime <= 0)
                {
                    _isBoostMode = false;
                    GameOver();
                }
                if (life > 0)
                {
                    speed = _initialSpeed;
                    _isBoostMode = false;
                }
            }
            else
            {
                emergencyPanel.SetActive(false);
            }



            if (life > 0 && !_haveOneMoreCoin)
            {
                _isInvincible = false;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                DecreaseLife(1);
            }
        }

        private void AddLifeIntoGrid()
        {
            GameObject newPlayer = Instantiate(playerPrefab, _lifeGroup.transform);
            players.Add(newPlayer);
        }

        private void RemoveLifeFromGrid()
        {
            if (players.Count > 0)
            {
                GameObject lastPlayer = players[^1];
                players.RemoveAt(players.Count - 1);
                Destroy(lastPlayer);
            }
            else
            {
                Debug.Log("No players to remove");
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.parent.gameObject.CompareTag("Obstacle") && !_isInvincible)
            {
                DecreaseLife(1);
            }

            if (collision.transform.parent.gameObject.CompareTag("Score"))
            {
                IncreaseScore(1);
                if (_isBoostMode) boostTime += boostAddTime;
                Destroy(collision.gameObject);
            }

            if (collision.transform.parent.gameObject.CompareTag("Life"))
            {
                IncreaseLife(1);
                Destroy(collision.gameObject);
            }
        }
    }
}