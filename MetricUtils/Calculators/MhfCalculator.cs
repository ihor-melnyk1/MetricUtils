using System.Reflection;

namespace MetricUtils;

public class MhfCalculator(List<DependencyTree> trees)
{
    private List<DependencyTree> Trees { get; } = trees;

    private Dictionary<string, bool> _objectClassMethodsToIgnore = new()
    {
        ["GetType"] = true,
        ["ToString"] = true,
        ["Equals"] = true,
        ["GetHashCode"] = true,
        ["MemberwiseClone"] = true,
        ["Finalize"] = true
    };
    private bool IsOverride(MethodInfo method)
    {
        return !method.Equals(method.GetBaseDefinition());
    }

    private double Mvi(DependencyTree tree)
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
                    return m.IsPublic;
                });
            node = node.Child;
        } while (node is not null);
        return res;
    }
    
    private double Mhi(DependencyTree tree)
    {
        var node = tree.Root;
        var res = 0;
        do
        {
            res += node.Value.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Count(m =>
                {
                    if (_objectClassMethodsToIgnore.ContainsKey(m.Name))
                        return false;
                    return m.IsPrivate;
                });
            node = node.Child;
        } while (node is not null);
        return res;
    }
    
    public double Calculate()
    {
        var res = 0.0;
        var mhiTotal = 0.0;
        var m_all = 0.0;
        foreach (var dependencyTree in Trees)
        {
            var mhi = Mhi(dependencyTree);
            mhiTotal += mhi;
            m_all += mhi + Mvi(dependencyTree);
        }
        return mhiTotal / m_all;
    }
    
}