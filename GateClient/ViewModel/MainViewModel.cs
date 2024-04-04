using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GateClient.ViewModel
{
    public partial class MainViewModel: ObservableObject
    {

        [ObservableProperty]
        private string? title;

        [ObservableProperty]
        private string? leftTopText;

        [ObservableProperty]
        private string? rightBottomText;


        public MainViewModel()
        {
            Title = "请检票";
            LeftTopText = "东风号航班\r\n始:山咀港\r\n终:下川独湾港码头\r\n00:00-23:00";
            RightBottomText = "山咀港 testGate1 14:53 v:1.02";
        }
    }
}
