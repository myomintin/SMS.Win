using System;

namespace SMS.Data
{
    public enum DbSaveOption : short
    {
        ClientWins,
        StoreWins,
        Merges
    };
}
