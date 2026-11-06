using UnityEngine;

namespace Player
{
    public class PlayerBonus : MonoBehaviour
    {
        private int bonus = 600;
        private float elapsedTime;
        private bool counting = true;

        private void Start()
        {
            GameManager.Instance.SetBonus(bonus);
        }

        private void Update()
        {
            if (!counting || bonus <= 0)
            {
                return;
            }

            elapsedTime += Time.deltaTime;

            while (elapsedTime >= 1f && bonus > 0)
            {
                elapsedTime -= 1f;
                bonus -= 5;

                if (bonus < 0)
                {
                    bonus = 0;
                }

                GameManager.Instance.SetBonus(bonus);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.tag == "Monster"
                || other.gameObject.tag == "Exit")
            {
                StopCountdown();
            }
        }

        public void StopCountdown()
        {
            counting = false;
        }

        public int GetBonus()
        {
            return bonus;
        }
    }
}
