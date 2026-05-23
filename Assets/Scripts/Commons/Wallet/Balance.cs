using System;
using Commons.Systems.Save;
using UnityEngine;
using Zenject;

namespace Commons.Wallet
{
    public class Balance : IReadOnlyWallet
    {
        public event Action ValueChanged;

        private ISaveSystem _saveSystem;

        public int Value { get; private set; }

        [Inject]
        private void Construct(ISaveSystem saveSystem)
        {
            _saveSystem = saveSystem;
            Value = _saveSystem.Load(CommonSaveKeys.CurrencyInt, 0);
        }

        public void Add(int value)
        {
            if (value < 0)
                throw new InvalidOperationException("Count cannot be negative");

            if (value is 0)
            {
                Debug.LogWarning("Add(0) called on wallet — no change applied. This may indicate redundant logic.");
                return;
            }

            Value += value;
            _saveSystem.Save(CommonSaveKeys.CurrencyInt, Value);
            ValueChanged?.Invoke();
        }

        public bool TrySpent(int price)
        {
            if (IsEnough(price) is false)
                return false;

            Value -= price;
            _saveSystem.Save(CommonSaveKeys.CurrencyInt, Value);
            ValueChanged?.Invoke();
            return true;
        }

        public bool IsEnough(int price)
        {
            if (price < 0)
                throw new InvalidOperationException("Price cannot be negative");

            return Value >= price;
        }
    }
}
