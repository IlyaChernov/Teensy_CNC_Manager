using System.Windows;

namespace TeensyCncManager
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    using HidLibrary;

    using Microsoft.Win32;

    using TeensyCNCManager.Core.GCode;

    using TeensyCncManager.ViewModel;
    using System.Xml;
    using ICSharpCode.AvalonEdit.Highlighting.Xshd;
    using ICSharpCode.AvalonEdit.Highlighting;
    using System.Reflection;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        readonly CNCViewModel vm = new CNCViewModel();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = vm;
            ButtonReloadCNCDevicesList_OnClick(this, new RoutedEventArgs());

            var assembly = Assembly.GetExecutingAssembly();
            using (Stream s = assembly.GetManifestResourceStream("TeensyCncManager.GCodeHighlighting.xshd"))
            {
                using (XmlTextReader reader = new XmlTextReader(s))
                {
                    GCodeEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }

            //var ri = new RadialInterpolation(new Point3D(0, 0, 0), new Point3D(30, -17.32050808, 20), 20d, RadialInterpolationDirection.CounterClockWise);
            //ri.GetArcPointDeg(60d);
            //var pts = ri.GetArcPoints(1.5 / 400, 0.00001);
        }

        private void Button_OpenGCodeFile_OnClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog { Filter = "All files (*.*)|*.*" };
            if (openFileDialog.ShowDialog() == true)
            {
                vm.FileName = openFileDialog.FileName;
                vm.GCode = File.ReadAllLines(openFileDialog.FileName).ToList();
                vm.AddToLog($"File '{Path.GetFileName(vm.FileName)}' Opened.");
                Button_PreprocessGCode_OnClick(sender, e);
            }
        }

        private void Button_PreprocessGCode_OnClick(object sender, RoutedEventArgs e)
        {
            vm.AddToLog("GCode Preprocessing Started");

            Observable.Start(
                () =>
                {
                    var preprop = new GCodePreprocessor();
                    vm.PreprocessedGCodes = preprop.Preprocess(vm.GCode, new G00 { FSpeed = vm.Gs.DefaultSpeed }, vm.Gs.LeadscrewPitch / vm.Gs.StepsPerRevolution, vm.Gs.DefaultSpeed).ToList();
                    vm.MovementPoints = preprop.MovementPoints.ToArray();
                    vm.AddToLog("GCode Preprocessing Finished");
                    vm.ProgressMaximum = vm.PreprocessedGCodes.Count;
                });
        }

        public void Connect(int connectionId, object target)
        {
            throw new System.NotImplementedException();
        }

        private void UIElement_OnLostFocus(object sender, RoutedEventArgs e)
        {
            ((TextBox)sender).Text = ((TextBox)sender).Text.Replace('.', CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator[0]).Replace(',', CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator[0]);
        }

        private void ButtonReloadCNCDevicesList_OnClick(object sender, RoutedEventArgs e)
        {
            vm.HIDDevices = HidDevices.Enumerate(5824).Where(x => x.DevicePath.Contains("mi_00")).Select(x => new KeyValuePair<string, string>(x.DevicePath, "Teensy USBCNC"));
        }

        private void UIElement_OnMouseWheel(object sender, MouseWheelEventArgs e)
        {

            var matrix = FlatPlotPath.LayoutTransform.Value;

            if (e.Delta > 0)
            {
                matrix.ScaleAt(1.5, 1.5, e.GetPosition(this).X, e.GetPosition(this).Y);
            }
            else
            {
                matrix.ScaleAt(1.0 / 1.5, 1.0 / 1.5, e.GetPosition(this).X, e.GetPosition(this).Y);
            }

            FlatPlotPath.LayoutTransform = new MatrixTransform(matrix);            
        }
    }
}