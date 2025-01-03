using System;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class ComponentInject : Attribute
{
    public Required Required { get; set; }
    public SearchDirection SearchDirection { get; set; }

    public ComponentInject(Required required = Required.DEFAULT, SearchDirection searchDirection = SearchDirection.DEFAULT)
    {
        this.Required = required;
        this.SearchDirection = searchDirection;
    }
}

public enum SearchDirection
{
    DEFAULT,
    SELF,
    CHILDREN,
    PARENT,
    ALL
}

public enum Required
{
    DEFAULT,
    OPTIONAL,
    REQUIRED

}