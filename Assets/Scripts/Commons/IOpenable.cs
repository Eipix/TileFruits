using System;

namespace Commons
{
    public interface IOpenable
    {
        event Action Opening;
        event Action Closing;
    }
}
