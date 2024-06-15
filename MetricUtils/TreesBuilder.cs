using System.Reflection;

namespace MetricUtils;

public class TreesBuilder
{
    public Assembly Assembly { get; }
    
    public TreesBuilder(Assembly assembly)
    {
        Assembly = assembly;
    }
    private bool TryToAddChildToTree(Type type, List<DependencyTree> trees)
    {
        foreach (var dependencyTree in trees)
        {
            var t = dependencyTree.FindParentClassOfType(type);
            if (t is not null)
            {
                var newNode = new DependencyNode(type, null, t);
                t.AddChild(newNode);
                return true;
            }
        }

        return false;
    }

    public List<DependencyTree> BuildTrees()
    {
        var types = Assembly.GetTypes();
        
        List<DependencyTree> trees = [];

        foreach (var type in types)
        {
            if (!type.IsClass) continue;
            var createNode = () =>
            {
                var rootNode = new DependencyNode(type, null, null);
                var tree = new DependencyTree(rootNode);
                trees.Add(tree);
            };
            if (trees.Count == 0)
            {
                createNode();
                continue;
            }

            var isAddedToTree = TryToAddChildToTree(type, trees);

            if (!isAddedToTree)
            {
                createNode();
            }
        }
        return trees;
    }
}