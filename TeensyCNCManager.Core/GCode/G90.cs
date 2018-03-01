namespace TeensyCNCManager.Core.GCode
{
    using System.Globalization;
    using System.Linq;

    [Code(CodeName = "G90")]
    public class G90 : IGcode
    {
        public void ClearParams()
        {
            foreach (var propertyInfo in GetType().GetProperties().Where(p => p.GetCustomAttributes(typeof(CodeParameter), true).All(x => !((CodeParameter)x).Persistent)))
            {
                propertyInfo.SetValue(this, null);
            }
        }

        public void WipeOutParams()
        {
            foreach (var propertyInfo in GetType().GetProperties().Where(p => p.GetCustomAttributes(typeof(CodeParameter), true).Any()))
            {
                propertyInfo.SetValue(this, null);
            }
        }

        public void ApplyParam(string param)
        {
            foreach (var propertyInfo in GetType().GetProperties().Where(p => p.GetCustomAttributes(typeof(CodeParameter), true).Any()))
            {
                var req = propertyInfo.GetCustomAttributes(true);
                var separator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                foreach (CodeParameter o in req)
                {
                    if (param.StartsWith(o.ParamName))
                        propertyInfo.SetValue(this, double.Parse(param.Substring(1).Replace(".", separator).Replace(",", separator)));
                }
            }
        }

        public double? XStart { get; set; }

        public double? YStart { get; set; }

        public double? ZStart { get; set; }

        [CodeParameter(ParamName = "X")]
        [CodeParameter(ParamName = "x")]
        public double? XDestination { get; set; }

        [CodeParameter(ParamName = "Y")]
        [CodeParameter(ParamName = "y")]
        public double? YDestination { get; set; }

        [CodeParameter(ParamName = "Z")]
        [CodeParameter(ParamName = "z")]
        public double? ZDestination { get; set; }

        [CodeParameter(ParamName = "F", Persistent = true)]
        [CodeParameter(ParamName = "f")]
        public double? FSpeed { get; set; }
    }
}