namespace TeensyCNCManager.Core.GCode
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows;
    using TeensyCNCManager.Core.Extensions;

    public class GCodePreprocessor
    {
        private List<IGcode> parsedGcodes = new List<IGcode>();

        public List<Point> MovementPoints = new List<Point>();

        public IEnumerable<SCodeLine> Preprocess(List<string> codes, IGcode defaultCommand, double stepSize, double defaultSpeed)
        {
            int lineNumber = 1;
            parsedGcodes.Clear();
            MovementPoints.Clear();

            var cleanCodes = CleanUpComments(codes).Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x));

            //   var parsedGcodes = new List<IGcode>();
            var prevComm = defaultCommand;
            foreach (var cleanCode in cleanCodes)
            {
                var prs = GParser.Parse(cleanCode.Trim(), prevComm);

                parsedGcodes.Add(prs);

                if (prs != null)
                {
                    prevComm = prs;
                }
            }

            var prevFrame = "";

            prevComm = defaultCommand;

            foreach (var gcode in parsedGcodes)
            {
                var result = "";

                if (gcode == null)
                {
                    yield return new SCodeLine { lineNumber= lineNumber++, code = result };
                    continue;
                }

                var code = gcode.GetType().GetCustomAttributes(typeof(Code), true);
                if (code.Any())
                {
                    if (gcode is IExpandable)
                    {
                        var exCodes = (gcode as IExpandable).Expand(prevComm, defaultSpeed, stepSize, 0.0001).ToList();

                        foreach (var exCode in exCodes)
                        {
                            if (MovementPoints.Any() && !MovementPoints.Last().Equals(new Point((exCode.XStart ?? 0), (exCode.YStart ?? 0))))
                                MovementPoints.Add(new Point(exCode.XStart ?? 0, exCode.YStart ?? 0));

                            MovementPoints.Add(new Point(
                                                 (exCode.XDestination ?? 0),
                                                 (exCode.YDestination ?? 0)));

                            result = "";

                            var ecode = exCode.GetType().GetCustomAttributes(typeof(Code), true);
                            result += ecode.Cast<Code>().First().CodeName + " ";

                            foreach (
        var propertyInfo in
            exCode.GetType()
                .GetProperties()
                .Where(p => p.GetCustomAttributes(typeof(CodeParameter), true).Any()))
                            {
                                var req = propertyInfo.GetCustomAttributes(true);
                                var value = propertyInfo.GetValue(exCode);

                                if (value != null)
                                    foreach (CodeParameter o in req)
                                    {
                                        var val = Math.Round((double)value / stepSize) * stepSize;
                                        result += o.ParamName.ToUpper() + $"{val:F20}".TrimEnd(new[] { '0' }) + " ";
                                        break;
                                    }
                            }

                            if (prevFrame != result)
                            {
                                prevFrame = result;
                                yield return new SCodeLine { lineNumber = lineNumber++, code = result };
                            }
                        }

                    }
                    else
                    {
                        if (MovementPoints.Any() && !MovementPoints.Last().Equals(new Point(gcode.XStart ?? 0, gcode.YStart ?? 0)))
                            MovementPoints.Add(new Point(gcode.XStart ?? 0, gcode.YStart ?? 0));

                        MovementPoints.Add(new Point(
                                             gcode.XDestination ?? 0,
                                             gcode.YDestination ?? 0));

                        result += code.Cast<Code>().First().CodeName + " ";

                        foreach (
    var propertyInfo in
        gcode.GetType()
            .GetProperties()
            .Where(p => p.GetCustomAttributes(typeof(CodeParameter), true).Any()))
                        {
                            var req = propertyInfo.GetCustomAttributes(true);
                            var value = propertyInfo.GetValue(gcode);

                            if (value != null)
                                foreach (CodeParameter o in req)
                                {
                                    result += o.ParamName.ToUpper() + value + " ";
                                    break;
                                }
                        }
                    }
                    prevComm = gcode;
                }
                if (prevFrame != result)
                {
                    prevFrame = result;
                    yield return new SCodeLine { lineNumber = lineNumber++, code = result };
                }
            }
        }

        public static IEnumerable<string> CleanUpComments(List<string> codes)
        {
            var comments = new Regex(@"((\(.*\))|(;.*))");
            return from code in codes let cd = comments.Replace(code, "").Trim() where !string.IsNullOrEmpty(cd) select code;
        }
    }
}