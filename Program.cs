using System;
using System.Collections.Generic;


abstract class Node
{
    protected Node()
        { 
        this.Children = new List<Node>();
        }
    
    public List<Node> Children { get; protected set; }//propiedad para acceder
    public abstract bool Execute();
    public virtual void AddChild(Node child)
    {
        throw new InvalidOperationException($"No se pueden agregar hijos a un nodo de tipo {GetType().Name}");
    }
}
class Root : Node
{

    public Root(Node child)
    {
        if (Children.Count == 0)
        {
            Children.Add(child);
        }
        else
        {
            throw new InvalidOperationException("Root solo puede tener un hijo");
        }
    }

    public override bool Execute()
    {
        return Children[0].Execute();
    }
}
abstract class Composite : Node
{
    public abstract bool Check();
    public override void AddChild(Node child)
    {
        Children.Add(child);
    }
}
class Selector : Composite
{
    public override bool Execute()
    {
        foreach (var child in Children)
        {
            if (child.Execute())
            {
                return true;
            }
        }
        return false;
    }

    public override bool Check()
    {
        return true;
    }
}

class Sequence : Composite
{
    public override bool Execute()
    {
        foreach (var child in Children)
        {
            if (!child.Execute())
            {
                return false;
            }
        }
        return true;
    }

    public override bool Check()
    {
        return true;
    }
}

class Task : Node
{
    public Task(Func<bool> action)
    {
        this.action = action;
    }

    private Func<bool> action;

    public override bool Execute()
    {
        return action();
    }

}

class Program
{
    static void Main()
    {
        // Creando nodos de tarea
        Task task1 = new Task(() => { Console.WriteLine("Tarea 1 ejecutada"); return false; });
        Task task2 = new Task(() => { Console.WriteLine("Tarea 2 ejecutada"); return false; });
        Task task3 = new Task(() => { Console.WriteLine("Tarea 3 ejecutada"); return true; });

        // Creando nodos compuestos
        Selector selector = new Selector();
        selector.Children.Add(task1);
        selector.Children.Add(task2);
        selector.Children.Add(task3);

        Sequence sequence = new Sequence();
        sequence.Children.Add(task1);
        sequence.Children.Add(task2);
        sequence.Children.Add(task3);
        sequence.Children.Add(selector);

        // Nodo raíz
        Root root = new Root(sequence);
        root.Children.Add(root);
        Console.WriteLine("Ejecutando árbol de comportamiento:");
        root.Execute();
    }
}

