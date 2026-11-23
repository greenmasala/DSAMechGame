using UnityEngine;

public class MapStructure : MonoBehaviour
{
    [SerializeField] private GameObject model;

    private void Update()
    {
        if (model == null) return;

        if (MapGenerator.Instance.IsChunkLoaded(transform.position))
        {
            if (!model.activeSelf) model.SetActive(true);
        }
        else
        {
            if (model.activeSelf) model.SetActive(false);
        }
    }
}