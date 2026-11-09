using UnityEngine;

public class Player : Entity
{
    public Equipment[] Weapons;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("LAUNCHING");
            Weapons[0].OnUse(this);
        }
        //else if (Input.GetMouseButtonDown(0) & !Weapons[0].SemiAuto)
        //{
        //    Weapons[0].OnUse(this);
        //}

        if (Input.GetMouseButtonDown(1))
        {
            Weapons[1].OnUse(this);
        }
        //else if (Input.GetMouseButtonDown(1) & !Weapons[1].SemiAuto)
        //{
        //    Weapons[1].OnUse(this);
        //}
    }
}
