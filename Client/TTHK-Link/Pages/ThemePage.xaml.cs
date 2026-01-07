using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TTHK_Link.ViewModels;

namespace TTHK_Link.Pages;

public partial class ThemePage : ContentPage
{
    public ThemePage(ThemeViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}