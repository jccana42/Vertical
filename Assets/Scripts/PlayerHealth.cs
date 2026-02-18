using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private int startHealth = 5;

    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;

    public event Action<int, int> OnHealthChanged; // (current, max)
    public event Action OnDied;

    private int currentHealth;

    private void Awake()
    {
        maxHealth = Mathf.Max(1, maxHealth);
        startHealth = Mathf.Clamp(startHealth, 1, maxHealth);
        currentHealth = startHealth;

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public bool Damage(int amount)
    {
        if (amount <= 0) return false;
        if (currentHealth <= 0) return false;

        int before = currentHealth;
        currentHealth = Mathf.Max(0, currentHealth - amount);

        if (currentHealth != before)
            OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
            OnDied?.Invoke();

        return currentHealth != before;
    }

    public void Heal(int amount)
    {
        if (amount <= 0) return;
        if (currentHealth <= 0) return;

        int before = currentHealth;
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);

        if (currentHealth != before)
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void ResetHealth()
    {
        currentHealth = Mathf.Clamp(startHealth, 1, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void SetHealth(int value)
    {
        int clamped = Mathf.Clamp(value, 0, maxHealth);
        if (clamped == currentHealth) return;

        currentHealth = clamped;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
            OnDied?.Invoke();
    }
}
