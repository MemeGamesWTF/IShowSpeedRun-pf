using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Rigidbody2D and Animator components
    private Rigidbody2D rb;
    private Animator animator;

    [Space(10)]
    // Gravitational force, jump force, and movement speed
    public float gravity = 9.81f * 2f;
    public float jumpForce = 8f;
    public float moveSpeed = 5f;

    [Space(10)]
    // Ground check and sound effects
    public LayerMask groundLayer;
    public AudioClip jumpSound;
    public AudioClip gameOverSound;

    public AudioSource audioSource;
    private bool isGrounded;

    // Called when the script instance is being loaded
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
    }

    // Called every frame
    private void Update()
    {
        if (!isGrounded)
        {
            rb.gravityScale = gravity;
        }
        else
        {
            rb.gravityScale = 0f;
        }

        animator.SetBool("isGrounded", isGrounded);

        float horizontalMove = Input.GetAxis("Horizontal");
        Vector2 move = new Vector2(horizontalMove, 0f) * moveSpeed * Time.deltaTime;
        rb.velocity = new Vector2(move.x, rb.velocity.y);

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        if (GameManager.Instance.isGameOver && Input.GetKeyDown(KeyCode.R))
        {
            GameManager.Instance.NewGame();
        }
    }

    // Jump method
    public void Jump()
    {
        if (isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            animator.SetTrigger("Jump");
            PlaySound(jumpSound); // Play jump sound
            Debug.Log("Jump Triggered");
        }
        else
        {
            Debug.Log("Not grounded, cannot jump");
        }
    }

    // Check if the player is grounded
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            TriggerGameOver();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            Debug.Log("Obstacle");
            TriggerGameOver();
        }
    }

    // Play sound using the AudioSource
    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    // Trigger Game Over logic
    private void TriggerGameOver()
    {
        PlaySound(gameOverSound); // Play game over sound
        GameManager.Instance.GameOver();
    }

    // Quit the game
    private void QuitGame()
    {
#if UNITY_STANDALONE
        Application.Quit();
#endif

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
