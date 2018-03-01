namespace TeensyCNCManager.Core.GCode
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class GParser
    {
        public static IGcode Parse(string frame, IGcode current)
        {
            if (current == null) throw new ArgumentNullException("current");
            if (string.IsNullOrEmpty(frame)) throw new ArgumentException("Frame is empty", "frame");
            IGcode result;
            var paramRegex = new Regex(@"([XxYyZzFfSsRrIiJjKk])((-?\d+)([,.]\d+)?|([,.]\d+))");
            var gcodeRegex = new Regex(@"([GgMm])((-?\d+)([,.]\d+)?|([,.]\d+))");
            var paramMatches = paramRegex.Matches(frame);
            var gcodeMatches = gcodeRegex.Matches(frame);

            if (gcodeMatches.Count == 0)
            {
                result = CloneCode(current);
            }
            else
            {
                switch (gcodeMatches.Cast<Match>().Last().Value.ToUpper())
                {
                    case "G0":
                    case "G00":
                        result = new G00();
                        break;
                    case "G1":
                    case "G01":
                        result = new G01();
                        break;
                    case "G2":
                    case "G02":
                        result = new G02();
                        break;
                    case "G3":
                    case "G03":
                        result = new G03();
                        break;
                    case "G90":
                        result = new G90();
                        break;
                    case "G91":
                        result = new G91();
                        break;
                    default:
                        result = CloneCode(current);
                        break;
                }
            }

            return ApplyParameters(result, current, paramMatches.Cast<Match>().Select(x => x.Value));
        }

        private static IGcode CloneCode(IGcode originalGcode)
        {
            var type = originalGcode.GetType();
            var result = (IGcode)Activator.CreateInstance(type);

            foreach (var propertyInfo in type.GetProperties().Where(p => p.GetCustomAttributes(typeof(CodeParameter), true).Any(x => ((CodeParameter)x).Persistent)))
            {
                propertyInfo.SetValue(result, propertyInfo.GetValue(originalGcode));
            }
            return result;
        }

        private static IGcode ApplyParameters(IGcode code, IGcode current, IEnumerable<string> parameters)
        {
            foreach (var paramMatch in parameters)
            {
                code.ApplyParam(paramMatch);
            }

            code.XStart = current.XDestination;
            code.YStart = current.YDestination;
            code.ZStart = current.ZDestination;

            code.XDestination = code.XDestination ?? code.XStart;
            code.YDestination = code.YDestination ?? code.YStart;
            code.ZDestination = code.ZDestination ?? code.ZStart;

            return code;
        }
    }
}