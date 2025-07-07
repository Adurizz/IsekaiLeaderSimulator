using UnityEngine;

public class PlayerFollowerForCamera : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Vector3 offset;
    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = player.transform.position + offset;
    }
}
