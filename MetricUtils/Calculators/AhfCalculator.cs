using System.Reflection;

namespace MetricUtils;

public class AhfCalculator(List<DependencyTree> trees)
{
    private List<DependencyTree> Trees { get; } = trees;
    
    private bool IsFieldOverridenInSubclass(Type baseType, Type derivedType, string fieldName)
    {
        // Check if the base class contains the field
        var baseField = baseType.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (baseField == null)
        {
            return false;
        }

        // Check if the derived class contains the field
        var derivedField = derivedType.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (derivedField == null)
        {
            return false;
        }

        // Check if the DeclaringType of the derived field is the derived class
        if (derivedField.DeclaringType == derivedType)
        {
            return true;
        }

        return false;
    }

    private double Ad(DependencyTree tree)
    {
        var node = tree.Root;
        var res = 0;
        do
        {
            var fields = node.Value.GetFields();
            res += node.Parent is null ? fields.Length : fields.Count(f => !IsFieldOverridenInSubclass(node.Parent.Value, node.Value, f.Name));
            node = node.Child;
        } while (node is not null);
        return res;
    }
    
    private double Ahi(DependencyTree tree)
    {
        var node = tree.Root;
        var res = 0;
        do
        {
            res += node.Value.GetFields(BindingFlags.Instance | BindingFlags.NonPublic).Count(p => p.IsPrivate);
            node = node.Child;
        } while (node is not null);
        return res;
    }
    
    public double Calculate()
    {
        var ad = 0.0;
        var ahi = 0.0;
        foreach (var dependencyTree in Trees)
        {
            ad += Ad(dependencyTree);
            ahi += Ahi(dependencyTree);
        }
        return ahi / ad;
    }
}