using System;

namespace Pilipala.Data.DBase.Fields
{
    internal interface IField
    {
        string Name { get; }

        Type Type { get; }

        string TypeName { get; }
    }
}