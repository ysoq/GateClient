﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeCore;

namespace GateClient.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {

        [ObservableProperty]
        private string? title;

        [ObservableProperty]
        private string? leftTopText;

        [ObservableProperty]
        private string? rightBottomText;

        [ObservableProperty]
        private Color? startBg;

        [ObservableProperty]
        private Color? endBg;

        [ObservableProperty]
        private Brush? themeBg;

        [ObservableProperty]
        private Geometry? themeIcon;
        private readonly Appsettings appsettings;

        public MainViewModel(IQuartz quartz, Appsettings appsettings)
        {
            this.appsettings = appsettings;

            ChangePage1();
            ChangeVersionText();
            quartz.CreateJob(this, nameof(MainViewModel.ChangeVersionText), 1, ChangeVersionText);
        }

        private void ChangeVersionText()
        {
            var date = DateTime.Now.ToString("HH:mm");
            RightBottomText = $"{appsettings.Node("account").Value<string>("code")} {date} v:1.02";
        }

        [RelayCommand]
        void ChangePage1()
        {
            Title = "请检票";
            LeftTopText = "东风号航班\r\n始:山咀港\r\n终:下川独湾港码头\r\n00:00-23:00";
            StartBg = Util.ToColor("#CBE8FC");
            EndBg = Util.ToColor("#3BB9F3");
            ThemeBg = Util.ToBrush("#02A7F0");
            ThemeIcon = Geometry.Parse("M315.3 58C322 58 327.4 63.5 327.4 70.3L327.4 144L351.7 144L351.7 94.8C351.7 88 357.1 82.5 363.8 82.5C370.5 82.5 376 88 376 94.8L376 143.9L431.5 143.9L451.7 269L364 269L364 415L452.6 417.3L452.6 463.2L497.8 463.2C504.5 463.2 509.9 468.7 509.9 475.5C509.9 482.2 504.5 487.7 497.8 487.8L364.9 487.8L344.3 504.1C340.2 507.3 334.5 507.6 330.2 504.7L304.6 487.8L181.5 487.8C174.8 487.8 169.3 482.3 169.4 475.5C169.4 468.7 174.8 463.2 181.5 463.2L229.2 463.2L229.2 417.3L169.4 316.4L221.2 294.2L248.1 143.9L303.2 143.9L303.2 70.3C303.1 63.5 308.6 58 315.3 58ZM376.1 193.1L303.1 193.1C296.4 193.1 290.9 198.6 290.9 205.4C290.9 211.7 295.6 216.9 301.6 217.6L303 217.7L376 217.7C382.7 217.7 388.1 212.2 388.1 205.4C388.3 198.6 382.9 193.1 376.1 193.1ZM524.4 272L610.2 347C612.1 348.7 612.3 351.6 610.7 353.5C610.6 353.7 610.4 353.8 610.2 354L524.4 429C522.5 430.7 519.6 430.5 517.9 428.6C517.2 427.8 516.8 426.7 516.8 425.6L516.8 404.2L373.8 404.2L373.8 285.6L516.8 285.6L516.8 275.5C516.8 272.9 518.9 270.9 521.4 270.9C522.4 270.9 523.5 271.3 524.4 272ZM424.9 317.5C422.7 317.5 420.8 319 420.4 321.1L420.3 321.9L420.3 371.9C420.3 374.5 422.4 376.5 424.9 376.5C427.1 376.5 429 374.9 429.4 372.7L429.5 371.9L429.5 354C429.4 350.6 432.1 347.9 435.5 347.8C438.6 347.7 441.2 350 441.6 353.1L441.7 354L441.7 372C441.7 374.6 443.8 376.6 446.4 376.5C448.5 376.5 450.4 375 450.8 372.9L450.9 372L450.9 354.1C450.9 345.6 444.1 338.8 435.6 338.8C434 338.8 432.4 339 430.9 339.5L429.4 340L429.4 322C429.4 319.5 427.3 317.5 424.9 317.5ZM539.2 317.5C537 317.5 535.1 319 534.7 321.1L534.6 321.9L534.6 371.9C534.6 374.5 536.7 376.5 539.3 376.4C541.4 376.4 543.3 374.9 543.7 372.8L543.8 371.9L543.8 362.7L553.5 374.8C555.1 376.8 558 377.1 560 375.5C562 373.9 562.3 371 560.7 369L555.5 363C553.4 360.6 551.5 358.5 549.9 356.7L548.7 355.4L559.9 347C561.9 345.5 562.3 342.6 560.8 340.5C559.5 338.8 557.1 338.2 555.1 339.1L554.3 339.6L543.8 347.5L543.8 322C543.7 319.5 541.7 317.5 539.2 317.5ZM399.8 320.9L396.2 320.9C388.3 320.9 381.7 326.9 380.9 334.8L380.8 336.2L380.8 361.2C380.8 369.1 386.8 375.8 394.7 376.5L396.1 376.6L399.7 376.6C408.2 376.6 415 369.7 415 361.3C415 358.7 412.9 356.7 410.4 356.7C408.2 356.7 406.3 358.3 405.9 360.5L405.8 361.3C405.8 364.3 403.6 366.8 400.7 367.3L399.7 367.4L396.1 367.4C393.1 367.4 390.6 365.2 390.1 362.3L390 361.3L390 336.3C390 333.3 392.2 330.8 395.1 330.3L396.1 330.2L399.7 330.2C403.1 330.2 405.8 332.9 405.8 336.3C405.8 338.9 407.9 340.9 410.4 340.9C413 340.9 415 338.8 415 336.3C415 328.4 408.9 321.7 401 321L399.8 320.9ZM483 340.6C473.6 336.1 462.3 340.1 457.8 349.5C453.3 358.9 457.3 370.2 466.7 374.7C473.6 378 481.7 376.8 487.4 371.8C489.4 370.2 489.8 367.3 488.2 365.3C486.6 363.3 483.7 362.9 481.7 364.5C481.5 364.6 481.4 364.7 481.3 364.9C477.2 368.4 471.1 368 467.5 364C467.3 363.8 467.1 363.5 466.9 363.3L466.3 362.3L489.2 362.3C491.8 362.3 493.8 360.2 493.8 357.7C493.8 350.4 489.6 343.8 483 340.6ZM517.8 338.8L514.2 338.8C506.3 338.8 499.7 344.8 498.9 352.7L498.8 354.1L498.8 361.3C498.8 369.2 504.8 375.9 512.7 376.6L514.1 376.7L517.7 376.7C518.3 376.7 523.3 376.6 528.1 371.8C529.9 370 529.9 367.1 528.1 365.3C526.3 363.5 523.4 363.5 521.6 365.3C520.6 366.4 519.2 367.2 517.8 367.5L514.2 367.5C511.2 367.5 508.7 365.3 508.2 362.4L508.1 361.4L508.1 354.2C508.1 351.2 510.3 348.7 513.2 348.2L514.2 348.1L517.7 348.1L518.1 348.2C518.7 348.4 519.7 348.7 520.9 349.7L521.6 350.3C523.5 352 526.5 351.8 528.2 349.9C529.8 348.1 529.8 345.5 528.2 343.7C525.8 341.2 522.7 339.5 519.3 338.9L517.8 338.8ZM474.5 348L475.7 348C478.5 348.3 481.1 349.7 482.7 352L483.3 353.1L466.3 353.1L466.7 352.4L467.2 351.7C469 349.4 471.7 348.1 474.5 348ZM254.5 512.4C261.2 512.4 266.7 517.9 266.7 524.7C266.7 531.4 261.3 536.9 254.6 537L181.6 537C174.9 537 169.4 531.5 169.5 524.7C169.5 517.9 174.9 512.4 181.6 512.4L254.5 512.4ZM497.8 512.4C504.5 512.4 509.9 517.9 509.9 524.7C509.9 531.4 504.5 536.9 497.8 537L424.8 537C418.1 537 412.6 531.5 412.7 524.7C412.7 517.9 418.1 512.4 424.8 512.4L497.8 512.4Z");
        }

        [RelayCommand]
        void ChangePage2()
        {
            Title = "验票成功";
            LeftTopText = "东风号航班\r\n始:山咀港\r\n终:下川独湾港码头\r\n00:00-23:00";
            StartBg = Util.ToColor("#E3F2ED");
            EndBg = Util.ToColor("#7ADDAE");
            ThemeBg = Util.ToBrush("#3DD089");
            ThemeIcon = Geometry.Parse("M363.3 170.2L500.9 283.2C511.5 292.2 511.5 305.8 500.9 314.8C490.3 323.8 474.4 323.8 463.9 314.8L368.6 238L368.6 554.5C368.6 568.1 358 577.1 342.1 577.1C326.2 577.1 315.6 568.1 315.6 554.5L315.6 238L220.4 319.4C209.8 323.9 193.9 323.9 183.4 314.9C172.8 305.9 172.8 292.3 183.4 283.3L315.7 170.3C321 165.6 342.2 147.6 363.3 170.2Z");
        }

        [RelayCommand]
        void ChangePage3()
        {
            Title = "验票失败";
            LeftTopText = "";
            StartBg = Util.ToColor("#F9D3DD");
            EndBg = Util.ToColor("#E54E63");
            ThemeBg = Util.ToBrush("#D9001B");
            ThemeIcon = Geometry.Parse("M304.147 267.811L196.347 157.511L304.147 47.2108C316.447 37.3109 317.747 20.3109 307.047 9.11086C296.347 -2.08916 277.647 -3.08916 265.347 6.81085L157.147 117.411L48.9468 6.81085C36.6468 -3.08916 17.9468 -2.08916 7.24681 9.11086C-3.4532 20.3109 -2.1532 37.3109 10.1468 47.2108L117.947 157.511L10.1468 267.811C-2.1532 277.711 -3.4532 294.811 7.24681 305.911C17.9468 317.111 36.6468 318.111 48.9468 308.211L157.147 197.611L265.247 308.211C277.547 318.111 296.247 317.111 306.947 305.911C317.747 294.811 316.447 277.711 304.147 267.811Z");
        }
        [RelayCommand]
        void ChangePage4()
        {
            Title = "网络异常";
            LeftTopText = "";
            StartBg = Util.ToColor("#FDE6E4");
            EndBg = Util.ToColor("#F29772");
            ThemeBg = Util.ToBrush("#EC6B32");
            ThemeIcon = Geometry.Parse("M241.848 278.2C267.448 278.2 288.248 299 288.248 324.6C288.248 350.2 267.448 371 241.848 371C216.248 371 195.448 350.2 195.448 324.6C195.448 299 216.248 278.2 241.848 278.2ZM111.648 23.2L384.948 296.5C394.148 305.4 394.448 320.1 385.548 329.3C376.648 338.5 361.948 338.8 352.748 329.9C352.548 329.7 352.348 329.5 352.148 329.3L78.8476 55.9C69.8476 46.8 69.8476 32.1 78.8476 23.1C87.9476 14.1 102.648 14.1 111.648 23.2ZM152.148 148.9L192.448 189.2C162.748 199.7 137.348 220.3 116.348 251L87.3476 234.7C78.5476 229.9 75.2476 218.9 80.0476 210.1C80.5476 209.1 81.1476 208.2 81.8476 207.3C102.648 181.6 126.048 162.1 152.148 148.9ZM242.948 128.1C314.348 128.1 372.148 160.2 416.248 224.5L384.048 242.1C375.748 246.7 365.348 244.7 359.248 237.3C344.148 218.6 326.948 204.5 307.648 195.1L240.548 128.1L242.948 128.1ZM62.7476 59.5L103.348 100.1C80.7476 117.2 59.6476 139.1 39.9476 165.7L9.74757 149.2C0.847565 144.5 -2.55244 133.5 2.04757 124.7C2.44756 124 2.84756 123.2 3.34756 122.6C20.2476 98.9 40.1476 77.7 62.7476 59.5ZM243.348 0C351.148 0 434.948 46.5 494.748 139.5L460.648 157.9C452.448 162.4 442.148 160.4 436.148 153.3C382.248 88.2 318.048 55.1 243.448 53.8C220.448 53.3 197.548 56.4 175.548 62.9L132.248 19.6C165.848 6.5 202.848 0 243.348 0Z");
        }
    }
}
