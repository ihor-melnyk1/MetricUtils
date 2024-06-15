namespace MetricUtils;

public class TreePrinter
{
    public DependencyTree Tree { get; }
    
    public TreePrinter(DependencyTree tree)
    {
        Tree = tree;
    }

    public void PrintMaxDepth()
    {
        Console.WriteLine($"Max depth of tree(DIT): {Tree.GetDepth()}");
    }

    public void PrintDepthOfEachNode()
    {
        var node = Tree.Root;

        do
        {
            Console.WriteLine($"Depth of {node.Value.Name}: {node.GetDepth()}");
            node = node.Child;
        } while (node is not null);

    }
}