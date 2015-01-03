namespace Pilipala.Data.Xbase.Xbase3
{
    internal class CharacterField : Field
    {
        public CharacterField(byte[] buffer)
            : base(buffer)
        {
            Type = typeof(string);
            TypeName = "Character";
        }
    }
}
