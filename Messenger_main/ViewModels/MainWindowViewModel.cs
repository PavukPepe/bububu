using Messenger_main.Views;
using PRACT_LAB_5.ViewModels.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Messenger_main.ViewModels
{
   
	internal class MainWindowViewModel : BindingHelper
    {
		private Page Curpage;

		public Page curpage
		{
			get { return Curpage; }
			set { Curpage = value; }
		}
	}
}
