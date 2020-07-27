using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MHFoodBank.TimeClock.Views
{
    public partial class MainPage : ContentPage
    {
        private double _width;
        private double _height;

        public MainPage()
        {
            InitializeComponent();
            Title = "Medicine Hat Food Bank Time Clock";
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (width != _width || height != _height)
            {
                _width = width;
                _height = height;
                if (width < height)
                {
                    mainGrid.RowDefinitions.Clear();
                    mainGrid.ColumnDefinitions.Clear();
                    mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(4, GridUnitType.Star) });
                    mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2, GridUnitType.Star) });
                    mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2, GridUnitType.Star) });
                    mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2, GridUnitType.Star) });
                    mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2, GridUnitType.Star) });
                    mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2, GridUnitType.Star) });
                    mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    mainGrid.Children.Clear();
                    mainGrid.Children.Add(imgLogo, 0, 1);
                    mainGrid.Children.Add(lblResult, 0, 2);
                    mainGrid.Children.Add(txtEmail, 0, 3);
                    mainGrid.Children.Add(txtPass, 0, 4);
                    mainGrid.Children.Add(cmbPositions, 0, 5);
                    mainGrid.Children.Add(btnPunch, 0, 6);
                }
                else
                {
                    mainGrid.RowDefinitions.Clear();
                    mainGrid.ColumnDefinitions.Clear();
                    mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(3, GridUnitType.Star) });
                    mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2, GridUnitType.Star) });
                    mainGrid.Children.Clear();
                    mainGrid.Children.Add(imgLogo, 0, 1);
                    mainGrid.Children.Add(lblResult, 0, 2);
                    mainGrid.Children.Add(txtEmail, 0, 3);
                    mainGrid.Children.Add(txtPass, 0, 4);
                    mainGrid.Children.Add(cmbPositions, 0, 5);
                    mainGrid.Children.Add(btnPunch, 0, 6);
                }
            }
        }
    }
}