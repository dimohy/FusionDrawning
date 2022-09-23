using CommunityToolkit.Mvvm.ComponentModel;

using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FusionDrawning
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [INotifyPropertyChanged]
    public sealed partial class MainPage : Page
    {
        private ToolMode _toolMode;
        private bool _mustToolAction;
        private Polyline? newPath;

        public Point CursorPoint { get; } = new();


        public MainPage()
        {
            this.InitializeComponent();

            SetToolMode(ToolMode.Pen);
        }

        private void penButton_Click(object sender, RoutedEventArgs e)
        {
            SetToolMode(ToolMode.Pen);
        }

        private void eraseButton_Click(object sender, RoutedEventArgs e)
        {
            SetToolMode(ToolMode.Erase);
        }

        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            canvas.Children.Clear();

            SetToolMode(ToolMode.Pen);
        }

        private void SetToolMode(ToolMode toolMode)
        {
            if (toolMode is ToolMode.Pen)
            {
                penButton.IsChecked = true;
                eraseButton.IsChecked = false;
            }
            else if (toolMode is ToolMode.Erase)
            {
                eraseButton.IsChecked = true;
                penButton.IsChecked = false;
            }
            else
                return;

            _toolMode = toolMode;
        }

        private void canvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var point = e.GetCurrentPoint(canvas);

            if (point.Properties.IsLeftButtonPressed is true)
            {
                //canvas.CapturePointer(e.Pointer);

                _mustToolAction = true;
                ActionTool(true);
            }
            //else if (point.Properties.IsLeftButtonPressed is false)
            //{
            //    _mustToolAction = false;
            //}
        }

        private void canvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _mustToolAction = false;
        }

        private void canvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            var point = e.GetCurrentPoint(canvas);
            CursorPoint.X = point.Position.X;
            CursorPoint.Y = point.Position.Y;

            if (_mustToolAction is true)
                ActionTool(false);
        }

        private void ActionTool(bool isInit)
        {
            if (_toolMode is ToolMode.Pen)
            {
                if (isInit is true)
                {
                    var penStyle = GetPenStyle();
                    newPath = new Polyline
                    {
                        Stroke = penStyle.Brush,
                        Width = canvas.Width,
                        Height = canvas.Height,
                        StrokeThickness = penStyle.Thickness,
                    };
                    canvas.Children.Add(newPath);

                    newPath.PointerEntered += (s, e) =>
                    {
                        // 지우개 모드이므로 해당 개체를 삭제함
                        if (_mustToolAction is true && _toolMode is ToolMode.Erase)
                        {
                            canvas.Children.Remove(s as UIElement);
                        }
                    };
                   
                    return;
                }

                newPath!.Points.Add(new(CursorPoint.X, CursorPoint.Y));
            }
            //else if (_toolMode is ToolMode.Erase)
            //{
            //}
        }

        private (Brush Brush, double Thickness) GetPenStyle()
        {
            var brush = penColorComboBox.SelectedIndex switch
            {
                0 => BrushHelper.Black,
                1 => BrushHelper.White,
                2 => BrushHelper.Red,
                3 => BrushHelper.Yellow,
                4 => BrushHelper.Blue,
                5 => BrushHelper.Green,
                6 => BrushHelper.Orange,
                _ => BrushHelper.Black
            };

            var thickness = penThicknessComboBox.SelectedIndex switch
            {
                0 => 1,
                1 => 2,
                2 => 3,
                3 => 4,
                4 => 5,
                _ => 1,
            };

            return (brush, thickness);
        }
    }


    public enum ToolMode
    {
        Pen,
        Erase
    }

    public partial class Point : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IntX))]
        private double _x;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IntY))]
        private double _y;

        public int IntX => (int)X;
        public int IntY => (int)Y;
    }

    public static class BrushHelper
    {
        public static readonly Brush Black = new SolidColorBrush(Colors.Black);
        public static readonly Brush White = new SolidColorBrush(Colors.White);
        public static readonly Brush Red = new SolidColorBrush(Colors.Red);
        public static readonly Brush Yellow = new SolidColorBrush(Colors.Yellow);
        public static readonly Brush Blue = new SolidColorBrush(Colors.Blue);
        public static readonly Brush Green = new SolidColorBrush(Colors.Green);
        public static readonly Brush Orange = new SolidColorBrush(Colors.Orange);
    }
}
