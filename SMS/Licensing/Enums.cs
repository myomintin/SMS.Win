using System;

namespace CodeflexSystems.SMS.Licensing
{
    internal delegate byte ConvertToByte(uint x, double factor, byte mask);

    internal enum ProductType : byte
    {
        Invalid = 0,
        SmartUI,
        SMS
    }

    internal enum LicenseType : byte
    {
        Trial = 0,
        Free,
        Commercial,
        Development
    }

    internal enum LicenseStatus : byte
    {
        Invalid = 0,
        Licensed,
        Trial,
        Expired,
        VersionMismatch,
        InternalError,
        MachineHashMismatch,
        NotFound
    }
}
