using System;
using UnityEngine;

namespace Script
{
    public class ObstacleBehavior : MonoBehaviour
    {
        private void Start()
        {
            foreach (Transform child in transform)
            {
                if (child.gameObject.GetComponent<BoxCollider2D>() == null)
                {
                    var a = child.gameObject.AddComponent<BoxCollider2D>();
                    a.isTrigger = true;
                }
            }
        }
    }
}