using UnityEngine;
using UnityEngine.Events;

public class MapStructure : MonoBehaviour
{
    [SerializeField] private GameObject model;

    [SerializeField] private UnityEvent OnStructureLoadedEvent;
    [SerializeField] private UnityEvent OnStructureUnloadedEvent;

    private bool isLoaded;

    private void Update()
    {
        bool isChunkLoaded = MapGenerator.Instance.IsChunkLoaded(transform.position);
        if (isChunkLoaded && !isLoaded)
        {
            isLoaded = true;
            if (model != null) model.SetActive(true); 
            OnStructureLoadedEvent?.Invoke();
        }
        else if (!isChunkLoaded && isLoaded)
        {
            isLoaded = false;
            if (model != null) model.SetActive(false); 
            OnStructureUnloadedEvent?.Invoke();
        }
    }
}