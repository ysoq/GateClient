using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows;

namespace CodeCore
{
    public static class AnimationUtils
    {
        #region 扩展方法

        /// <summary>
        /// 旋转动画
        /// </summary>
        /// <param name="from">开始位置</param>
        /// <param name="to">结束位置</param>
        /// <param name="durationSeconds">播放秒数</param>
        /// <param name="setting">扩展配置</param>
        public static void BeginAngleAnimation(this UIElement el, double from, double to, double durationSeconds, AnimationSetting setting = null)
        {
            RotateTransform rotate = new RotateTransform();
            el.RenderTransform = rotate;
            el.RenderTransformOrigin = new Point(0.5, 0.5);//定义圆心位置        
            var scaleAnimation = GetAnimation(from, to, durationSeconds, setting);
            scaleAnimation.FillBehavior = FillBehavior.Stop;

            AnimationClock clock = scaleAnimation.CreateClock();
            rotate.ApplyAnimationClock(RotateTransform.AngleProperty, clock);
        }

        /// <summary>
        /// 播放旋转动画后自动复原
        /// </summary>
        /// <param name="from">开始位置</param>
        /// <param name="to">结束位置</param>
        /// <param name="durationSeconds">播放秒数</param>
        public static void BeginAngleAnimationRecovery(this UIElement el, double from, double to, double durationSeconds)
        {
            el.BeginAngleAnimation(from, to, durationSeconds, new AnimationSetting(true));
        }


        /// <summary>
        /// 缩放动画
        /// </summary>
        /// <param name="from">开始位置</param>
        /// <param name="to">结束位置</param>
        /// <param name="durationSeconds">播放秒数</param>
        /// <param name="setting">扩展配置</param>
        public static void BeginScaleAnimation(this UIElement el, double from, double to, double durationSeconds, AnimationSetting setting = null)
        {
            ScaleTransform scale = new ScaleTransform();
            el.RenderTransform = scale;
            el.RenderTransformOrigin = new Point(0.5, 0.5);//定义圆心位置        
            var scaleAnimation = GetAnimation(from, to, durationSeconds, setting);
            AnimationClock clock = scaleAnimation.CreateClock();
            scale.ApplyAnimationClock(ScaleTransform.ScaleXProperty, clock);
            scale.ApplyAnimationClock(ScaleTransform.ScaleYProperty, clock);
        }

        /// <summary>
        /// 播放缩放动画后自动复原
        /// </summary>
        /// <param name="from">开始位置</param>
        /// <param name="to">结束位置</param>
        /// <param name="durationSeconds">播放秒数</param>
        public static void BeginScaleAnimationRecovery(this UIElement el, double from, double to, double durationSeconds)
        {
            el.BeginScaleAnimation(from, to, durationSeconds, new AnimationSetting(true));
        }

        /// <summary>
        /// 播放动画
        /// </summary>
        /// <param name="dp">动画属性</param>
        /// <param name="from">开始位置</param>
        /// <param name="to">结束位置</param>
        /// <param name="durationSeconds">播放秒数</param>
        /// <param name="setting">扩展配置</param>
        public static void BeginAnimation(this UIElement el, DependencyProperty dp, Thickness from, Thickness to, double durationSeconds, AnimationSetting setting = null)
        {
            var animation = GetAnimation(from, to, durationSeconds, setting);
            el.BeginAnimation(dp, animation);
        }

        /// <summary>
        /// 播放动画后自动复原
        /// </summary>
        /// <param name="dp">动画属性</param>
        /// <param name="from">开始位置</param>
        /// <param name="to">结束位置</param>
        /// <param name="durationSeconds">播放秒数</param>
        public static void BeginAnimationRecovery(this UIElement el, DependencyProperty dp, Thickness from, Thickness to, double durationSeconds)
        {
            el.BeginAnimation(dp, from, to, durationSeconds, new AnimationSetting(true));
        }

        /// <summary>
        /// 播放动画
        /// </summary>
        /// <param name="dp">动画属性</param>
        /// <param name="from">开始位置</param>
        /// <param name="to">结束位置</param>
        /// <param name="durationSeconds">播放秒数</param>
        /// <param name="setting">扩展配置</param>
        public static void BeginAnimation(this UIElement el, DependencyProperty dp, double from, double to, double durationSeconds, AnimationSetting setting = null)
        {
            var storyboard = new Storyboard();
            var animation = GetAnimation(from, to, durationSeconds, setting);

            Storyboard.SetTarget(animation, el);
            Storyboard.SetTargetProperty(animation, new PropertyPath(dp));
            storyboard.Children.Add(animation);
            storyboard.Begin();
        }

        /// <summary>
        /// 播放动画后自动复原
        /// </summary>
        /// <param name="dp">动画属性</param>
        /// <param name="from">开始位置</param>
        /// <param name="to">结束位置</param>
        /// <param name="durationSeconds">播放秒数</param>
        public static void BeginAnimationRecovery(this UIElement el, DependencyProperty dp, double from, double to, double durationSeconds)
        {
            el.BeginAnimation(dp, from, to, durationSeconds, new AnimationSetting(true));
        }

        /// <summary>
        /// 播放动画
        /// </summary>
        /// <param name="dp">动画属性</param>
        /// <param name="from">开始位置</param>
        /// <param name="to">结束位置</param>
        /// <param name="durationSeconds">播放秒数</param>
        /// <param name="setting">扩展配置</param>
        public static void BeginAnimation(this UIElement el, DependencyProperty dp, Color from, Color to, double durationSeconds, AnimationSetting setting = null)
        {
            var animation = GetAnimation(from, to, durationSeconds, setting);
            el.BeginAnimation(dp, animation);
        }

        /// <summary>
        /// 播放动画后自动复原
        /// </summary>
        /// <param name="dp">动画属性</param>
        /// <param name="from">开始位置</param>
        /// <param name="to">结束位置</param>
        /// <param name="durationSeconds">播放秒数</param>
        public static void BeginAnimationRecovery(this UIElement el, DependencyProperty dp, Color from, Color to, double durationSeconds)
        {
            el.BeginAnimation(dp, from, to, durationSeconds, new AnimationSetting(true));
        }
        #endregion

        #region 创建动画

        /// <summary>
        /// 创建浮点动画
        /// </summary>
        /// <param name="from">开始位置</param>
        /// <param name="to">结束位置</param>
        /// <param name="durationSeconds">播放秒数</param>
        /// <param name="beginTimeSeconds">开始秒</param>
        /// <param name="completeAction">结束后回调函数</param>
        /// <returns></returns>
        public static DoubleAnimation GetAnimation(double from, double to, double durationSeconds, AnimationSetting setting = null)
        {
            var animation = new DoubleAnimation()
            {
                Duration = TimeSpan.FromSeconds(durationSeconds),
                EasingFunction = GetEasingFunction(setting),
            }.Set(setting);
            if (from >= 0)
            {
                animation.From = from;
            }
            if (to >= 0)
            {
                animation.To = to;
            }

            return animation;
        }

        /// <summary>
        /// 创建Thickness动画
        /// </summary>
        /// <param name="from">开始位置</param>
        /// <param name="to">结束位置</param>
        /// <param name="durationSeconds">播放秒数</param>
        /// <param name="beginTimeSeconds">开始秒</param>
        /// <param name="completeAction">结束后回调函数</param>
        /// <returns></returns>
        public static ThicknessAnimation GetAnimation(Thickness from, Thickness to, double durationSeconds, AnimationSetting setting = null)
        {
            var animation = new ThicknessAnimation()
            {
                From = from,
                To = to,
                Duration = TimeSpan.FromSeconds(durationSeconds),
                EasingFunction = GetEasingFunction(setting),
            }.Set(setting);

            return animation;
        }

        /// <summary>
        /// 创建color动画
        /// </summary>
        /// <param name="from">开始位置</param>
        /// <param name="to">结束位置</param>
        /// <param name="durationSeconds">播放秒数</param>
        /// <param name="beginTimeSeconds">开始秒</param>
        /// <param name="completeAction">结束后回调函数</param>
        /// <returns></returns>
        public static ColorAnimation GetAnimation(Color from, Color to, double durationSeconds, AnimationSetting setting = null)
        {
            var animation = new ColorAnimation()
            {
                From = from,
                To = to,
                Duration = TimeSpan.FromSeconds(durationSeconds),
                EasingFunction = GetEasingFunction(setting),
            }.Set(setting);

            return animation;
        }

        #endregion

        #region 配置


        /// <summary>
        /// 获取缓动函数
        /// </summary>
        /// <returns></returns>
        private static EasingFunctionBase GetEasingFunction(AnimationSetting setting)
        {
            if (setting == null || setting.RunCount != 1)
            {
                return null;
            }
            else
            {
                return new PowerEase()
                {
                    EasingMode = EasingMode.EaseOut,
                    Power = 5
                };
            }
        }
        /// <summary>
        /// 获取运行次数
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        private static RepeatBehavior GetRunCount(AnimationSetting setting)
        {
            if (setting == null || setting.RunCount == 0)
            {
                return new RepeatBehavior(1);
            }
            if (setting.RunCount == -1)
            {
                return RepeatBehavior.Forever;
            }
            else
            {
                return new RepeatBehavior(setting.RunCount);
            }
        }
        /// <summary>
        /// 加载扩展配置项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ani"></param>
        /// <param name="timeline"></param>
        /// <returns></returns>
        public static T Set<T>(this T ani, AnimationSetting timeline) where T : AnimationTimeline
        {
            if (timeline == null)
            {
                return ani;
            }
            ani.BeginTime = timeline.BeginTime;
            ani.RepeatBehavior = GetRunCount(timeline);
            ani.AutoReverse = timeline.RunCount != 1 || timeline.AutoReverse;

            if (timeline.Completed != null)
                ani.Completed += delegate { timeline.Completed(); };
            return ani;
        }
        #endregion
    }
}
