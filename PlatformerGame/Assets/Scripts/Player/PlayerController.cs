using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        private Rigidbody2D rigidbody2d;
        [SerializeField] private PlayerInputReader inputReader;
        [SerializeField] private PlayerGroundDetector groundCheck;
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float maxJumpVelocity = 8f;
        [SerializeField] private float minJumpVelocity = 3f;
        [SerializeField] private float coyoteTimeMax = 0.2f;
        private float coyoteTimeCounter;
        private float jumpPressTimer;
        private bool isChargingJump;

        [SerializeField] private AudioSource audioSource;

        private bool isBlockedByWall = false;

        private void Start()
        {
            rigidbody2d = GetComponent<Rigidbody2D>();
            if (inputReader == null) inputReader = GetComponent<PlayerInputReader>();
            if (groundCheck == null) groundCheck = GetComponentInChildren<PlayerGroundDetector>();
            if (audioSource == null) audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            // Gestion du Coyote Time
            if (groundCheck != null && groundCheck.IsGrounded())
            {
                coyoteTimeCounter = coyoteTimeMax;
            }
            else
            {
                coyoteTimeCounter -= Time.deltaTime;
            }

            // Logique du saut chargé
            HandleJumpInput();
        }

        private void FixedUpdate()
        {
            float targetSpeed;

            if (isBlockedByWall)
            {
                targetSpeed = 0f;
            }
            else
            {
                targetSpeed = inputReader.MoveInput.x * moveSpeed;
            }

            rigidbody2d.linearVelocity = new Vector2(targetSpeed, rigidbody2d.linearVelocity.y);
        }
        private void HandleJumpInput()
        {
            bool canJump = groundCheck.IsGrounded() || coyoteTimeCounter > 0f;

            if (inputReader.JumpTriggered && canJump && !isChargingJump)
            {
                isChargingJump = true;
                jumpPressTimer = 0f;
                rigidbody2d.linearVelocity = new Vector2(rigidbody2d.linearVelocity.x, minJumpVelocity);
                PlayJumpSound();
            }

            if (isChargingJump)
            {
                if (inputReader.JumpHeld && jumpPressTimer < 0.25f)
                {
                    jumpPressTimer += Time.deltaTime;
                    float additionalVelocity = Mathf.Lerp(minJumpVelocity, maxJumpVelocity, jumpPressTimer / 0.25f);
                    rigidbody2d.linearVelocity = new Vector2(rigidbody2d.linearVelocity.x, additionalVelocity);
                }
                else
                {
                    isChargingJump = false;
                }
            }

            if (!inputReader.JumpHeld)
            {
                isChargingJump = false;
            }

            inputReader.ResetJumpTrigger();
        }

        private void PlayJumpSound()
        {
            if (audioSource != null)
            {
                audioSource.PlayOneShot(SoundManager.Instance.PlayerJump);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            CheckWallCollision(collision);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            CheckWallCollision(collision);
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            isBlockedByWall = false;
        }

        private void CheckWallCollision(Collision2D collision)
        {
            if (groundCheck != null && !groundCheck.IsGrounded())
            {
                if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
                {
                    isBlockedByWall = true;
                }
            }
        }
    }
}