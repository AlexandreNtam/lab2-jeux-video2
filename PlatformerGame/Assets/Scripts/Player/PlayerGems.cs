using UnityEngine;

namespace Player
{
    public class PlayerGems : MonoBehaviour
    {
        private AudioSource audioSource;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.tag == "Gem"
                && other.gameObject.activeSelf)
            {
                other.gameObject.SetActive(false);
                audioSource.PlayOneShot(SoundManager.Instance.GemCollected);
                GameManager.Instance.AddScore(50);
            }
        }
    }
}
