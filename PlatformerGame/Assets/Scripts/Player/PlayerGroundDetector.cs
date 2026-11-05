using UnityEngine;

public class PlayerGroundDetector : MonoBehaviour
{
    private int groundContacts = 0;

    public bool IsGrounded()
    {
        return groundContacts > 0;
    }

    private bool IsGround(Collider2D other)
    {
        return other.gameObject.tag == "HardBlock"
            || other.gameObject.tag == "Platform";
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsGround(other))
        {
            groundContacts++;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (IsGround(other))
        {
            groundContacts--;

            if (groundContacts < 0)
            {
                groundContacts = 0;
            }
        }
    }
}
