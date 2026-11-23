using UnityEngine;

public class Enemy : Entity
{
    // [Ben] Chai, I added these fields + Initialize() from the class diagram, my Jeep needed a player to chase
    // [Ben] I didn't add ChasePlayer() yet, Jeep has its own chasing style

    [SerializeField] protected Player player;
    [SerializeField] protected float attackingRange = 4f;
    [SerializeField] protected float damage = 40f;

    // [Ben] Call this after spawning an enemy (like in EnemyManager)
    public void Initialize(Player player)
    {
        this.player = player;
    }

    // [Ben] If nobody called Initialize() it finds the Player by itself, so enemies still work in test scenes

    protected virtual void Start()
    {
        if (player == null) player = FindAnyObjectByType<Player>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
