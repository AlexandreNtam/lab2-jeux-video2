using UnityEngine;

namespace Player
{
    public class PlayerState : MonoBehaviour
    {
        private PlayerInputReader inputReader;
        private Rigidbody2D rigidbody2d;
        private PlayerBonus bonus;
        private AudioSource audioSource;

        private bool finished;
        private bool isDead;
        private bool restartRequested;
        private float deathTime;
        private Vector3 deathStartScale;

        private const float DEATH_DURATION = 1f;

        private void Start()
        {
            inputReader = GetComponent<PlayerInputReader>();
            rigidbody2d = GetComponent<Rigidbody2D>();
            bonus = GetComponent<PlayerBonus>();
            audioSource = GetComponent<AudioSource>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (finished)
            {
                return;
            }

            if (other.gameObject.tag != "Monster"
                && other.gameObject.tag != "Exit")
            {
                return;
            }

            finished = true;
            inputReader.StopReading();
            rigidbody2d.linearVelocity = Vector2.zero;
            bonus.StopCountdown();

            if (other.gameObject.tag == "Monster")
            {
                isDead = true;
                deathStartScale = transform.localScale;

                GameManager.Instance.PlayerDie();
                audioSource.PlayOneShot(SoundManager.Instance.PlayerKilled);
            }
            else
            {
                GameManager.Instance.AddScore(bonus.GetBonus());

                audioSource.PlayOneShot(SoundManager.Instance.ExitReached);

                GameManager.Instance.StartNextlevel(2f);
            }
        }

        private void Update()
        {
            if (!isDead || restartRequested)
            {
                return;
            }

            deathTime += Time.deltaTime;
            float progress = deathTime / DEATH_DURATION;

            if (progress > 1f)
            {
                progress = 1f;
            }

            transform.localScale = deathStartScale * (1f - progress);

            if (deathTime >= DEATH_DURATION)
            {
                restartRequested = true;
                GameManager.Instance.RestartLevel(0f);
            }
        }
    }
}
