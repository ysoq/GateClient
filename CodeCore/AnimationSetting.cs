using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows;

namespace CodeCore
{
    public class AnimationSetting : Timeline
    {
        public AnimationSetting() { }
        public AnimationSetting(bool autoReverse)
        {
            this.AutoReverse = autoReverse;
        }
        public int RunCount { get; set; } = 1;

        public new Action Completed;

        protected override Freezable CreateInstanceCore()
        {
            return new AnimationSetting();
        }
    }
}
