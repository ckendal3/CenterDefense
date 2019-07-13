namespace WDIB.Interfaces
{
    public interface IDamageable
    {
        bool isAlive { get; }

        bool ApplyDamage(float damage);

        void ApplyHealing(float healthRestore);
    }
}