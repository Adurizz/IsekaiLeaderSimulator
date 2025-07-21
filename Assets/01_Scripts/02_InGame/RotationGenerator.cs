using UnityEngine;

public class RotationGenerator : MonoBehaviour
{
    [SerializeField] private Vector3 rotationSpeed = new Vector3(0, 100, 0); // 초당 회전 각도(도 단위)

    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
