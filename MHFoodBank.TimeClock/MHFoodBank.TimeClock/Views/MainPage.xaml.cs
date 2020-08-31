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
                    authGrid.RowDefinitions.Clear();
                    authGrid.ColumnDefinitions.Clear();
                    authGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    authGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(4, GridUnitType.Star) });
                    authGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2, GridUnitType.Star) });
                    authGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2, GridUnitType.Star) });
                    authGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2, GridUnitType.Star) });
                    authGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2, GridUnitType.Star) });
                    authGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    authGrid.Children.Clear();
                    authGrid.Children.Add(authImgLogo, 0, 1);
                    authGrid.Children.Add(authLblResult, 0, 2);
                    authGrid.Children.Add(txtEmail, 0, 3);
                    authGrid.Children.Add(txtPass, 0, 4);
                    authGrid.Children.Add(btnSignIn, 0, 5);

                    clockGrid.RowDefinitions.Clear();
                    clockGrid.ColumnDefinitions.Clear();
                    clockGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    clockGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(4, GridUnitType.Star) });
                    clockGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2, GridUnitType.Star) });
                    clockGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2, GridUnitType.Star) });
                    clockGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2, GridUnitType.Star) });
                    clockGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2, GridUnitType.Star) });
                    clockGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    clockGrid.Children.Clear();
                    clockGrid.Children.Add(clockImgLogo, 0, 1);
                    clockGrid.Children.Add(clockLblResult, 0, 2);
                    clockGrid.Children.Add(cmbPositions, 0, 3);
                    clockGrid.Children.Add(btnPunch, 0, 4);
                    clockGrid.Children.Add(btnSignOut, 0, 5);
                }
                else
                {
                    authGrid.RowDefinitions.Clear();
                    authGrid.ColumnDefinitions.Clear();
                    authGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    authGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(3, GridUnitType.Star) });
                    authGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    authGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    authGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    authGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2, GridUnitType.Star) });
                    authGrid.Children.Clear();
                    authGrid.Children.Add(authImgLogo, 0, 1);
                    authGrid.Children.Add(authLblResult, 0, 2);
                    authGrid.Children.Add(txtEmail, 0, 3);
                    authGrid.Children.Add(txtPass, 0, 4);
                    authGrid.Children.Add(btnSignIn, 0, 5);

                    clockGrid.RowDefinitions.Clear();
                    clockGrid.ColumnDefinitions.Clear();
                    authGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    authGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(3, GridUnitType.Star) });
                    authGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    authGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2, GridUnitType.Star) });
                    authGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2, GridUnitType.Star) });
                    authGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2, GridUnitType.Star) });
                    clockGrid.Children.Clear();
                    clockGrid.Children.Add(clockImgLogo, 0, 1);
                    clockGrid.Children.Add(clockLblResult, 0, 2);
                    clockGrid.Children.Add(cmbPositions, 0, 3);
                    clockGrid.Children.Add(btnPunch, 0, 4);
                    clockGrid.Children.Add(btnSignOut, 0, 5);
                }
            }
        }
    }
}