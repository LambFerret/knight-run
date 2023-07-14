using UnityEngine;

namespace Script
{
    public class CameraController : MonoBehaviour
    {
        public float speed = 5f; // Adjust to desired speed

        void Update()
        {
            transform.position += Vector3.right * speed * Time.deltaTime;
        }
    }
}