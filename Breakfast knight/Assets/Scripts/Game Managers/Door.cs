using UnityEngine;

public class Door : MonoBehaviour
{
    public bool isLocked;
    public Animator animator;

    private void Start()
    {
        if (isLocked)
        {
            Lock();
        }
        else
        {
            Unlock();
        }
    }

    public void Lock()
    {
        isLocked = true;
        if (animator != null)
        {
            animator.SetBool("isLocked", true);
        }
        gameObject.SetActive(true); // Activar la puerta
    }

    public void Unlock()
    {
        isLocked = false;
        if (animator != null)
        {
            animator.SetBool("isLocked", false);
        }
        gameObject.SetActive(false); // Desactivar la puerta
    }

}

