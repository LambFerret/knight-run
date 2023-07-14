using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Script
{
    public class PlayerController : MonoBehaviour
    {

        public float jumpForce = 5f;
        public int life = 10;
        public float invincibleTime = 2f;
        public int score;
        public int speed;

        private List<IPlayerObserver> _observers = new List<IPlayerObserver>();
        private bool _isJumping;
        private bool _isInvincible;
        private Rigidbody2D _rb;
        private SpriteRenderer _sprite;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _sprite = GetComponent<SpriteRenderer>();
        }

        public void Jump()
        {
            if (!_isJumping)
            {
                _rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
                _isJumping = true;
            }
        }

        private IEnumerator Blink()
        {
            _isInvincible = true;
            for (int i = 0; i < 3; i++)
            {
                _sprite.enabled = false;
                yield return new WaitForSeconds(0.1f);
                _sprite.enabled = true;
                yield return new WaitForSeconds(0.1f);
            }
            _isInvincible = false;
        }


        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
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
        }

        public void DecreaseLife(int damage)
        {
            life -= damage;
            NotifyLifeChanged(-damage);

            if (life <= 0)
            {
                Debug.Log("Player is dead");
            }

            StartCoroutine(Blink());
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