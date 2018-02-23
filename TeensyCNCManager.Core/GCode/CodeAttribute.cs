namespace TeensyCNCManager.Core.GCode
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class Code : Attribute
    {
        public string CodeName { get; set; }
    }
}
