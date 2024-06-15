using System.Reflection;

namespace MetricUtils;

public class PofCalculator(List<DependencyTree> trees)
{
    private List<DependencyTree> Trees { get; } = trees;

    private Dictionary<string, bool> _objectClassMethodsToIgnore = new()
    {
        ["GetType"] = true,
        ["ToString"] = true,
        ["Equals"] = true,
        ["GetHashCode"] = true,
    };
    private bool IsOverride(MethodInfo method)
    {
        return !method.Equals(method.GetBaseDefinition());
    }

    private double Ma(DependencyTree tree)
    {
        var node = tree.Root;
        var res = 0;
        do
        {
            res += node.Value.GetMethods().Count(m => !_objectClassMethodsToIgnore.ContainsKey(m.Name));
            node = node.Child;
        } while (node is not null);
        return res;
    }
    
    private double Mo(DependencyTree tree)
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
            
            res += node.Value.GetMethods()
                .Count(m =>
                {
                    if (_objectClassMethodsToIgnore.ContainsKey(m.Name))
                        return false;

                    var parentMethod = node.Parent.Value.GetMethod(m.Name);
                    return parentMethod is not null && IsOverride(parentMethod);
                });
            node = node.Child;
        } while (node is not null);
        return res;
    }

    public double OverridesMethodsByDepth(DependencyTree tree)
    {
        var node = tree.Root;
        var res = 0;
        do
        {
            res += node.Value.GetMethods()
                .Count(m =>
                {
                    if (_objectClassMethodsToIgnore.ContainsKey(m.Name))
                        return false;

                    if (node.Parent is null)
                        return true;
                    
                    var parentMethod = node.Parent.Value.GetMethod(m.Name);
                    return parentMethod is null;
                }) * node.GetDepth();
            
            node = node.Child;
        } while (node is not null);
        return res;
    }

    public double Calculate()
    {
        var mo = 0.0;
        var factor = 0.0;
        foreach (var dependencyTree in Trees)
        {
            mo += Mo(dependencyTree);
            factor += OverridesMethodsByDepth(dependencyTree);
        }
        return mo / factor;
    }
    
}