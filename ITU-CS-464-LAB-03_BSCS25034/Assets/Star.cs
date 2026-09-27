using UnityEngine;

public class Star : MonoBehaviour
{
    [Header("Optional")]
    public GameObject collectEffect; // particle/prefab to spawn on pickup, leave empty to skip
    public AudioClip collectSound;   // sound to play on pickup, leave empty to skip

    void OnTriggerEnter(Collider other)
    {
        // Only react to the plane, not every random collider in the scene
        if (other.GetComponent<AirplaneController>() == null)
            return;

        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }

        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }

        Destroy(gameObject);
    }
}