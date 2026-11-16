using UnityEngine;

public class CameraHolder : MonoBehaviour
{
    public Transform CameraPosition;

    private void Update()
    {
        transform.position = CameraPosition.position;
    }
}
