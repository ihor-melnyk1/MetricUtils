namespace MetricUtils;

public class DependencyTree
{
    public DependencyNode Root { get; set; }

    public DependencyTree(DependencyNode root)
    {
        Root = root;
    }

    public DependencyNode? FindParentClassOfType(Type type)
    {
        var node = Root;
        DependencyNode? resNode = null;
        while (node is not null)
        {
            if (type.IsSubclassOf(node.Value))
            {
                resNode = node;
            }
            node = node.Child;
        }

        return resNode;
    }

    public DependencyNode FindChildByType(Type type)
    {
        return Root.FindByType(type);
    }

    public int GetDepth()
    {
        return Root.GetDepth();
    }
}