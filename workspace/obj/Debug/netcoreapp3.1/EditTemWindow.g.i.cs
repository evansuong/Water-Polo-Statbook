﻿#pragma checksum "..\..\..\EditTemWindow.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "D23CAA9991C285D17F81DFFF871E442FB1FD229C"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using Water_Polo_Statbook;


namespace Water_Polo_Statbook {
    
    
    /// <summary>
    /// AddTeamWindow
    /// </summary>
    public partial class AddTeamWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 56 "..\..\..\EditTemWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button AddBTN;
        
        #line default
        #line hidden
        
        
        #line 57 "..\..\..\EditTemWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button RemoveBTN;
        
        #line default
        #line hidden
        
        
        #line 58 "..\..\..\EditTemWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button UpdateBTN;
        
        #line default
        #line hidden
        
        
        #line 59 "..\..\..\EditTemWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BackBTN;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.8.1.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Water Polo Statbook;component/edittemwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\EditTemWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.8.1.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 8 "..\..\..\EditTemWindow.xaml"
            ((Water_Polo_Statbook.AddTeamWindow)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.AddBTN = ((System.Windows.Controls.Button)(target));
            
            #line 56 "..\..\..\EditTemWindow.xaml"
            this.AddBTN.Click += new System.Windows.RoutedEventHandler(this.AddBTN_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.RemoveBTN = ((System.Windows.Controls.Button)(target));
            
            #line 57 "..\..\..\EditTemWindow.xaml"
            this.RemoveBTN.Click += new System.Windows.RoutedEventHandler(this.RemoveBTN_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.UpdateBTN = ((System.Windows.Controls.Button)(target));
            
            #line 58 "..\..\..\EditTemWindow.xaml"
            this.UpdateBTN.Click += new System.Windows.RoutedEventHandler(this.UpdateBTN_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.BackBTN = ((System.Windows.Controls.Button)(target));
            
            #line 59 "..\..\..\EditTemWindow.xaml"
            this.BackBTN.Click += new System.Windows.RoutedEventHandler(this.BackBTN_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
