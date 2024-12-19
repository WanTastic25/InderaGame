using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public bool inRange;
    [SerializeField] GameObject talkBubble;
    public KeyCode interactKey;
    public UnityEvent interactAction;
    private GameObject talkbubbleInstance;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (talkbubbleInstance == null)
            {
                //talkbubbleInstance = Instantiate(talkBubble, transform.position, Quaternion.Euler(0, 0, 0)); // Adjust the rotation as needed
            }
            inRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            inRange = false;
            if (talkbubbleInstance != null)
            {
                Destroy(talkbubbleInstance);
                talkbubbleInstance = null; // Clear the reference
            }
        }
    }
}
