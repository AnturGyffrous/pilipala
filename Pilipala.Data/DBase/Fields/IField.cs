using System;

namespace Pilipala.Data.DBase.Fields
{
    internal interface IField
    {
        int Length { get; }

        string Name { get; }

        Type Type { get; }

        string TypeName { get; }

        object Value { get; }

        void Parse(byte[] buffer, int offset);
    }
}
