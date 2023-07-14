using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
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
        public GameObject soldierEffect;
        public SpriteRenderer starSprite;
        public SFXPlayer sfxPlayer;

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

            float gravity = Mathf.Abs(Physics2D.gravity.y);
            float initialSpeed = jumpForce / _rb.mass;
            float timeToReachMaxHeight = initialSpeed / gravity;
            float totalJumpTime = 2 * timeToReachMaxHeight;
            float animationSpeed = 1 / totalJumpTime;
            animator.SetFloat("jumpTime", animationSpeed);
        }

        public void Jump()
        {
            if (!_isJumping)
            {
                sfxPlayer.Play("jump");
                _rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
                _isJumping = true;
                animator.SetBool("isJump", true);
            }
        }

        private IEnumerator Blink()
        {
            _isInvincible = true;
            yield return new WaitForSeconds(invincibleTime);
            if (!_isBoostMode) _isInvincible = false;
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
            sfxPlayer.Play("kill", 0.3F);
            score += amount;
            NotifyScoreChanged(amount);
        }

        public void IncreaseLife(int amount)
        {
            sfxPlayer.Play("cage");
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
            sfxPlayer.Play("playerDeath");
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
                boostTime -= Time.deltaTime;
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
                Vector3 destination = collision.transform.position;
                int b = Random.Range(7, 20);
                destination.x += 10;
                Instantiate(soldierEffect, collision.transform.position, Quaternion.identity);
                collision.transform.DOJump(destination, b, 1, 2f);
                collision.transform.DOScale(new Vector3(0.001F, 0.001F, 0.001F), 2F)
                    .OnComplete(() => Destroy(collision));
                StartCoroutine(MakeStar(destination/2));
            }

            if (collision.transform.parent.gameObject.CompareTag("Life"))
            {
                IncreaseLife(1);
                Destroy(collision.gameObject);
            }
        }

        public IEnumerator MakeStar(Vector3 pos)
        {
            yield return new WaitForSeconds(2f);
            starSprite.transform.position = pos;
            starSprite.transform.DORotate(new Vector3(0, 0, 360), 1f, RotateMode.FastBeyond360).SetLoops(-1);
            var sequence = DOTween.Sequence();
            sequence.Append(starSprite.transform.DOScale(new Vector3(0.05F, 0.05F, 0.05F), 0.5F));
            sequence.Append(starSprite.transform.DOScale(new Vector3(0.01F, 0.01F, 0.01F), 0.5F));
        }
    }
}