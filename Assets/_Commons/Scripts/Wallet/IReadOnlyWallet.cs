using System;

namespace Commons.Wallet
{
    public interface IReadOnlyWallet
    {
        public event Action ValueChanged;

        public int Value { get; }
    }
}
