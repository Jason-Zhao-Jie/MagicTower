public interface IJsonCollection : IUnit, System.Collections.Generic.IEnumerable<IUnit>
{
    bool AddChild(IUnit child, string tag);
    bool RemoveChild(string tag);
    IUnit GetChild(string tag);

    int Length { get; }
}
