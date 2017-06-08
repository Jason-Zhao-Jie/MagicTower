using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public interface IJsonCollection : IUnit, IEnumerable<IUnit>
{
    bool AddChild(IUnit child, string tag);
    bool RemoveChild(string tag);
    IUnit GetChild(string tag);

    int Length { get; }
}
