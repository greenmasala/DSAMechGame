using UnityEngine;

public class Player : MonoBehaviour
{
    public Equipment[] Weapons;
    float energy;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Weapons[0].OnUse(this);
        }

        if (Input.GetMouseButtonDown(1))
        {
            Weapons[1].OnUse(this);
        }
    }
}
