using System;

namespace Pilipala.Data.Xbase
{
    public interface IField
    {
        int Length { get; }

        string Name { get; }

        Type Type { get; }

        string TypeName { get; }

        object Value { get; }

        void Parse(byte[] buffer, int offset);
    }
}
