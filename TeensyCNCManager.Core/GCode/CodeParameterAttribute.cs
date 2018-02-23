namespace TeensyCNCManager.Core.GCode
{
    using System;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class CodeParameter : Attribute
    {
        public string ParamName { get; set; }

        public bool Persistent { get; set; }
    }
}
