using System;

namespace Commons.Wallet
{
    public interface IReadOnlyBalance
    {
        public event Action ValueChanged;

        public int Value { get; }
    }
}
