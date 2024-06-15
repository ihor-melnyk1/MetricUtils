using System.Reflection;

namespace MetricUtils;

public class AifCalculator(List<DependencyTree> trees)
{
    private List<DependencyTree> Trees { get; } = trees;

    private Dictionary<string, bool> _objectClassMethodsToIgnore = new()
    {
        ["GetType"] = true,
        ["ToString"] = true,
        ["Equals"] = true,
        ["GetHashCode"] = true,
    };
    private bool IsFieldHiddenInSubclass(Type baseType, Type derivedType, string fieldName)
    {
        // Check if the base class contains the field
        var baseField = baseType.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (baseField == null)
        {
            //No such field in the base class
            return false;
        }

        // Check if the derived class contains the field
        var derivedField = derivedType.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (derivedField == null)
        {
            //No such field in the derived class
            return false;
        }

        // Check if the DeclaringType of the derived field is the derived class
        if (derivedField.DeclaringType == derivedType)
        {
            // The derived class declares a field with the same name
            return true;
        }

        return false;
    }

    private double Aa(DependencyTree tree)
    {
        var node = tree.Root;
        var res = 0;
        do
        {
            res += node.Value.GetFields().Count(m => !_objectClassMethodsToIgnore.ContainsKey(m.Name));
            node = node.Child;
        } while (node is not null);
        return res;
    }
    
    private double Ai(DependencyTree tree)
    {
        var node = tree.Root;
        var res = 0;
        do
        {
            if (node.Parent is null)
            {
                node = node.Child;
                continue;
            }
            
            res += node.Value.GetFields()
                .Count(p =>
                {
                    if (_objectClassMethodsToIgnore.ContainsKey(p.Name))
                        return false;

                    var parentProperty = node.Parent.Value.GetField(p.Name);
                    if (parentProperty is null) return false;
                    return !IsFieldHiddenInSubclass(node.Parent.Value, node.Value, p.Name);
                });
            node = node.Child;
        } while (node is not null);
        return res;
    }
    
    public double Calculate()
    {
        var aa = 0.0;
        var ai = 0.0;
        foreach (var dependencyTree in Trees)
        {
            aa += Aa(dependencyTree);
            ai += Ai(dependencyTree);
        }
        return ai / aa;
    }
}