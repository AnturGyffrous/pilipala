namespace Pilipala.Data.Xbase.Xbase3
{
    internal class NumericField : Field
    {
        internal NumericField(byte[] buffer)
            : base(buffer)
        {
            Type = typeof(string);
            TypeName = "Character";
        }
    }
}
