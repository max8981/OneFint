using Client_Wpf.CustomControls;
using ClientLibrary.Enums;
using ClientLibrary.Models;
using ClientLibrary.ServerToClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml.Linq;

namespace Client_Wpf
{
    internal class GenerateElement
    {
        private static readonly Dictionary<int, ExhibitionControl> _exhibitions = new();
        private static readonly Dictionary<int, View> _views = new();
        public static void Normal(BaseContent content)
        {
            SetLayout(content.Layout);
            SetContent(content.NormalContents);
            SetContent(content.DefaultContents);
        }
        public static void Emergenyc(BaseContent content)
        {
            SetLayout(content.Layout);
            SetContent(content.EmergencyContents);
        }
        public static void Show(int id = 0)
        {
            if(_views.TryGetValue(id,out var view))
            {
                view.ShowView();
            }
            else
            {
                foreach (var item in _views.Values)
                {
                    item.ShowView();
                }
            }
        }
        public static void Close()
        {
            foreach (var item in _exhibitions.Values)
            {
                item.Close();
            }
            foreach (var item in _views.Values)
            {
                item.Close();
            }
        }
        private static View GetView(int id)
        {
            if(!_views.TryGetValue(id,out var view))
            {
                view = Application.Current.Dispatcher.Invoke(() => { return new View(); });
                _views.Add(id, view);
            }
            return view;
        }
        private static void SetLayout(ClientLibrary.Models.Layout layout)
        {
            var page = layout.Content.Pages.First();
            foreach (var compnent in page.Components)
            {
                var id = compnent.Id;
                var name = compnent.Name ?? $"Element{compnent.Id}";
                var w = compnent.W;
                var h = compnent.H;
                var x = compnent.X;
                var y = compnent.Y;
                var z = compnent.Z;
                var text = compnent.Text;
                var element = GetView(layout.Id).AddElement(name, w, h, x, y, z);
                switch (compnent.ComponentType)
                {
                    case ComponentTypeEnum.BROWSER:
                        element.ShowWebBrowser(id, text.Text);
                        break;
                    case ComponentTypeEnum.TEXT:
                        element.ShowText(id, text.Text, text.FontColor, text.BackgroundColor, GetFontSize(text.FontSize), (int)text.Horizontal, (int)text.Horizontal, (int)text.Vertical);
                        break;
                    case ComponentTypeEnum.CLOCK:
                        element.ShowClock((int)compnent.ClockType, text.FontColor, text.BackgroundColor, GetFontSize(text.FontSize));
                        break;
                    case ComponentTypeEnum.EXHIBITION_STAND:
                        element.ShowExhibition();
                        _exhibitions.TryAdd(id, element);
                        break;
                }
            }
        }
        private static void SetContent(Content[]? contents)
        {
            if (contents != null)
            {
                var types = contents.GroupBy(_ => _.ContentType).ToArray();
                foreach (var typeGroup in types)
                {
                    var group = typeGroup
                    .GroupBy(_ => _.Component.Id)
                    .Select(_ => new { ExhibitionControl = _exhibitions[_.Key], Content = _ })
                    .ToArray();
                    foreach (var exhibitionGroup in group)
                    {
                        var exhibition = exhibitionGroup.ExhibitionControl;
                        exhibition.Clear(typeGroup.Key);
                        foreach (var content in exhibitionGroup.Content)
                        {
                            var media = new ExhibitionControl.MediaContent
                            {
                                Name = content.Name,
                                StartAt = DateTime.Parse(content.StartedAt),
                                EndAt = DateTime.Parse(content.EndedAt),
                                Duration = content.PlayDuration,
                                Mute = content.Mute,
                                Id = content.Id,
                                MediaType = GetMediaType((int)content.Material.MaterialType),
                                Text = content.Text.Text,
                                FontSize = GetFontSize(content.Text.FontSize),
                                FontColor = content.Text.FontColor,
                                BackRound = content.Text.BackgroundColor,
                                TextAlignment = GetTextAlignment((int)content.Text.Horizontal),
                                HorizontalAlignment = GetHorizontal((int)content.Text.Horizontal),
                                VerticalAlignment = GetVertical((int)content.Text.Vertical),
                            };
                            switch (media.MediaType)
                            {
                                case ExhibitionControl.MediaContent.MediaTypeEnum.Video:
                                    media.Source = content.Material.Content;
                                    break;
                                case ExhibitionControl.MediaContent.MediaTypeEnum.Image:
                                    media.Source = content.Material.Content;
                                    break;
                                case ExhibitionControl.MediaContent.MediaTypeEnum.Web:
                                    media.Source = content.Url;
                                    break;
                                default:
                                    media.Source = content.Text.Text;
                                    break;
                            }
                            exhibition.AddContent(typeGroup.Key, media);
                        }
                        exhibition.PlayNext();
                    }
                }
            }
        }
        private static double GetFontSize(int? value)
        {
            return value == null ? 14d : (double)value;
        }
        private static System.Windows.HorizontalAlignment GetHorizontal(int horizontal)
        {
            return horizontal switch
            {
                1 => System.Windows.HorizontalAlignment.Left,
                2 => System.Windows.HorizontalAlignment.Center,
                3 => System.Windows.HorizontalAlignment.Right,
                _ => System.Windows.HorizontalAlignment.Stretch,
            };
        }
        private static System.Windows.VerticalAlignment GetVertical(int vertical)
        {
            return vertical switch
            {
                1 => System.Windows.VerticalAlignment.Top,
                2 => System.Windows.VerticalAlignment.Center,
                3 => System.Windows.VerticalAlignment.Bottom,
                _ => System.Windows.VerticalAlignment.Stretch,
            };
        }
        private static System.Windows.TextAlignment GetTextAlignment(int horizontal)
        {
            return horizontal switch
            {
                1 => System.Windows.TextAlignment.Left,
                2 => System.Windows.TextAlignment.Center,
                3 => System.Windows.TextAlignment.Right,
                _ => System.Windows.TextAlignment.Justify,
            };
        }
        private static ExhibitionControl.MediaContent.MediaTypeEnum GetMediaType(int type)
        {
            return type switch
            {
                2 => ExhibitionControl.MediaContent.MediaTypeEnum.Video,
                3 => ExhibitionControl.MediaContent.MediaTypeEnum.Image,
                4 => ExhibitionControl.MediaContent.MediaTypeEnum.Web,
                5 => ExhibitionControl.MediaContent.MediaTypeEnum.Text,
                _ => ExhibitionControl.MediaContent.MediaTypeEnum.Text,
            };
        }
    }
}