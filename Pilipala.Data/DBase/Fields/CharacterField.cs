namespace Pilipala.Data.DBase.Fields
{
    internal class CharacterField : Field
    {
        public CharacterField(byte[] buffer)
            : base(buffer)
        {
            Type = typeof(string);
            TypeName = "Character";
        }

        public override object Value { get; protected set; }
    }
}
