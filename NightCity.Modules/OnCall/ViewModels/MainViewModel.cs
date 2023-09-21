using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using NightCity.Core.Events;
using NightCity.Core.Services;
using NightCity.Core.Services.Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Windows.Input;
using System.Windows.Media;

namespace OnCall.ViewModels
{
    public class MainViewModel : BindableBase, IDisposable
    {
        //内置延迟
        private readonly int internalDelay = 500;
        //事件聚合器
        private readonly IEventAggregator eventAggregator;
        //属性服务
        private readonly IPropertyService propertyService;
        //Http服务
        private readonly HttpService httpService;
        public MainViewModel(IEventAggregator eventAggregator, IPropertyService propertyService)
        {
            //依赖注入及初始化
            this.eventAggregator = eventAggregator;
            this.propertyService = propertyService;
            httpService = new HttpService();
            //监听事件 链接命令发布
            eventAggregator.GetEvent<BannerMessageLinkingEvent>().Subscribe(message =>
            {
                Console.WriteLine(message);
                //对命令进行判断
            }, ThreadOption.UIThread);

            #region P1
            SeriesCollection1 = new SeriesCollection
            {
                new StackedAreaSeries
                {
                    Title = "Africa",
                    Values = new ChartValues<DateTimePoint>
                    {
                        new DateTimePoint(new DateTime(1950, 1, 1), .228),
                        new DateTimePoint(new DateTime(1960, 1, 1), .285),
                        new DateTimePoint(new DateTime(1970, 1, 1), .366),
                        new DateTimePoint(new DateTime(1980, 1, 1), .478),
                        new DateTimePoint(new DateTime(1990, 1, 1), .629),
                        new DateTimePoint(new DateTime(2000, 1, 1), .808),
                        new DateTimePoint(new DateTime(2010, 1, 1), 1.031),
                        new DateTimePoint(new DateTime(2013, 1, 1), 1.110)
                    },
                    LineSmoothness = 0
                },
                new StackedAreaSeries
                {
                    Title = "N & S America",
                    Values = new ChartValues<DateTimePoint>
                    {
                        new DateTimePoint(new DateTime(1950, 1, 1), .339),
                        new DateTimePoint(new DateTime(1960, 1, 1), .424),
                        new DateTimePoint(new DateTime(1970, 1, 1), .519),
                        new DateTimePoint(new DateTime(1980, 1, 1), .618),
                        new DateTimePoint(new DateTime(1990, 1, 1), .727),
                        new DateTimePoint(new DateTime(2000, 1, 1), .841),
                        new DateTimePoint(new DateTime(2010, 1, 1), .942),
                        new DateTimePoint(new DateTime(2013, 1, 1), .972)
                    },
                    LineSmoothness = 0
                },
                new StackedAreaSeries
                {
                    Title = "Asia",
                    Values = new ChartValues<DateTimePoint>
                    {
                        new DateTimePoint(new DateTime(1950, 1, 1), 1.395),
                        new DateTimePoint(new DateTime(1960, 1, 1), 1.694),
                        new DateTimePoint(new DateTime(1970, 1, 1), 2.128),
                        new DateTimePoint(new DateTime(1980, 1, 1), 2.634),
                        new DateTimePoint(new DateTime(1990, 1, 1), 3.213),
                        new DateTimePoint(new DateTime(2000, 1, 1), 3.717),
                        new DateTimePoint(new DateTime(2010, 1, 1), 4.165),
                        new DateTimePoint(new DateTime(2013, 1, 1), 4.298)
                    },
                    LineSmoothness = 0
                },
                new StackedAreaSeries
                {
                    Title = "Europe",
                    Values = new ChartValues<DateTimePoint>
                    {
                        new DateTimePoint(new DateTime(1950, 1, 1), .549),
                        new DateTimePoint(new DateTime(1960, 1, 1), .605),
                        new DateTimePoint(new DateTime(1970, 1, 1), .657),
                        new DateTimePoint(new DateTime(1980, 1, 1), .694),
                        new DateTimePoint(new DateTime(1990, 1, 1), .723),
                        new DateTimePoint(new DateTime(2000, 1, 1), .729),
                        new DateTimePoint(new DateTime(2010, 1, 1), .740),
                        new DateTimePoint(new DateTime(2013, 1, 1), .742)
                    },
                    LineSmoothness = 0
                }
            };
            XFormatter1 = val => new DateTime((long)val).ToString("yyyy");
            YFormatter1 = val => val.ToString("N") + " M";
            #endregion

            #region P2
            SeriesCollection2 = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "2015",
                    Values = new ChartValues<double> { 10, 50, 39, 50 }
                }
            };

            //adding series will update and animate the chart automatically
            SeriesCollection2.Add(new ColumnSeries
            {
                Title = "2016",
                Values = new ChartValues<double> { 11, 56, 42 }
            });

            //also adding values updates and animates the chart automatically
            SeriesCollection2[1].Values.Add(48d);

            Labels2 = new[] { "Maria", "Susan", "Charles", "Frida" };
            Formatter2 = value => value.ToString("N");
            #endregion

            #region P3
            SeriesCollection3 = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Series 1",
                    Values = new ChartValues<double> { 4, 6, 5, 2 ,4 }
                },
                new LineSeries
                {
                    Title = "Series 2",
                    Values = new ChartValues<double> { 6, 7, 3, 4 ,6 },
                    PointGeometry = null
                },
                new LineSeries
                {
                    Title = "Series 3",
                    Values = new ChartValues<double> { 4,2,7,2,7 },
                    PointGeometry = DefaultGeometries.Square,
                    PointGeometrySize = 15
                }
            };

            Labels3 = new[] { "Jan", "Feb", "Mar", "Apr", "May" };
            YFormatter3 = value => value.ToString("C");
            #endregion

            #region P4
            PointLabel4 = chartPoint => string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);
            #endregion

            #region P5
            SeriesCollection5 = new SeriesCollection
            {
                new OhlcSeries()
                {
                    Values = new ChartValues<OhlcPoint>
                    {
                        new OhlcPoint(32, 35, 30, 32),
                        new OhlcPoint(33, 38, 31, 37),
                        new OhlcPoint(35, 42, 30, 40),
                        new OhlcPoint(37, 40, 35, 38),
                        new OhlcPoint(35, 38, 32, 33)
                    }
                },
                new LineSeries
                {
                    Values = new ChartValues<double> {30, 32, 35, 30, 28},
                    Fill = Brushes.Transparent
                }
            };

            Labels5 = new[]
            {
                DateTime.Now.ToString("dd MMM"),
                DateTime.Now.AddDays(1).ToString("dd MMM"),
                DateTime.Now.AddDays(2).ToString("dd MMM"),
                DateTime.Now.AddDays(3).ToString("dd MMM"),
                DateTime.Now.AddDays(4).ToString("dd MMM"),
            };
            #endregion

        }

        #region 命令集合

        #region 命令：同步横幅信息
        public ICommand SyncBannerMessagesCommand
        {
            get => new DelegateCommand(SyncBannerMessages);
        }
        private void SyncBannerMessages()
        {
            eventAggregator.GetEvent<BannerMessageSyncingEvent>().Publish();
        }
        #endregion

        #region 命令：切换视图
        public ICommand SwitchViewCommand
        {
            get => new DelegateCommand<string>(SwitchView);
        }
        private void SwitchView(string view)
        {
            View = view;
        }
        #endregion

        #endregion

        #region 可视化属性集合

        #region 视图类型
        private string view = "Dashboard";
        public string View
        {
            get => view;
            set
            {
                SetProperty(ref view, value);
            }
        }
        #endregion



        #endregion

        #region P1
        private Func<double, string> _yFormatter1;
        public SeriesCollection SeriesCollection1 { get; set; }
        public Func<double, string> XFormatter1 { get; set; }
        public Func<double, string> YFormatter1
        {
            get { return _yFormatter1; }
            set
            {
                _yFormatter1 = value;
            }
        }
        #endregion

        #region P2
        public SeriesCollection SeriesCollection2 { get; set; }
        public string[] Labels2 { get; set; }
        public Func<double, string> Formatter2 { get; set; }
        #endregion

        #region P3
        public SeriesCollection SeriesCollection3 { get; set; }
        public string[] Labels3 { get; set; }
        public Func<double, string> YFormatter3 { get; set; }
        #endregion

        #region P4
        public Func<ChartPoint, string> PointLabel4 { get; set; }
        #endregion

        #region P5
        public SeriesCollection SeriesCollection5 { get; set; }
        private string[] _labels5;
        public string[] Labels5
        {
            get { return _labels5; }
            set
            {
                _labels5 = value;
            }
        }
        #endregion
        public void Dispose()
        {

        }
    }
}
