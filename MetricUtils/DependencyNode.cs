namespace MetricUtils;

public class DependencyNode
{
    public Type Value { get; set; }
    public DependencyNode? Child { get; set; }
    public DependencyNode? Parent { get; set; }

    public DependencyNode(Type value, DependencyNode child, DependencyNode parent)
    {
        Child = child;
        Parent = parent;
        Value = value;
    }

    public void AddChild(DependencyNode child)
    {
        Child = child;
        child.Parent = this;
    }
    
    public DependencyNode FindByType(Type type)
    {
        if (Value == type)
        {
            return this;
        }

        return Child is null ? null : Child.FindByType(type);
    }
    
    public int GetDepth()
    {
        var res = 0;
        var node = this;
        do
        {
            res += 1;
            node = node.Child;
        } while (node is not null);

        return res;
    }
}