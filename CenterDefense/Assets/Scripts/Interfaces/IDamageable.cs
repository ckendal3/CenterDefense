namespace WDIB.Interfaces
{
    public interface IDamageable
    {
        bool isAlive { get; }

        bool ApplyDamageGetNewHealth(float damage, out float newHealth);
        bool ApplyDamage(float damage);

        void ApplyHealing(float healthRestore);
    }
}