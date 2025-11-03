using UnityEngine;

public class teleport : MonoBehaviour
{
    [SerializeField] private float teleportX = 0;

    [SerializeField] private float teleportY = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player") collision.transform.position = new Vector3(teleportX, teleportY, 0);
    }
}