public class KnightAI : TroopAI
{
    protected override void Awake()
    {
        base.Awake();
        moveSpeed = 2.3f;
        stoppingDistance = 1.2f;
        attackRange = 1.8f;
        damagePerSecond = 2.8f;
        attackCooldown = 1.35f;
        attackAnimationDelay = 0.25f;     // Feel free to tweak
    }
}