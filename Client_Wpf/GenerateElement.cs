using Client_Wpf.CustomControls;
using ClientLibrary;
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
using Brush = System.Windows.Media.Brush;
using ColorConverter = System.Windows.Media.ColorConverter;

namespace Client_Wpf
{
    internal class GenerateElement
    {
        private readonly CustomControls.View _view;
        private readonly Dictionary<int, ExhibitionControl> _exhibitions = new();
        public Action<BaseContent> Content => SetLayout;
        public GenerateElement(CustomControls.View view)
        {
            _view = view;
            if(TryLoadLayout(out var content))
            {
                SetLayout(content);
            }
        }
        public void NormalContent(BaseContent baseContent)
        {
            foreach (var page in baseContent.Layout.Content.Pages)
            {
                SetLayout(page.Id, page);
            }
            SetContent(baseContent.NormalContents);
        }
        private void SetLayout(int index,ClientLibrary.Page page)
        {
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
                var element = _view.AddElement(name, w, h, x, y, z);
                switch (compnent.ComponentType)
                {
                    case Component.ComponentTypeEnum.BROWSER:
                        element.ShowWebBrowser(id, text.Text);
                        break;
                    case Component.ComponentTypeEnum.TEXT:
                        element.ShowText(id, text.Text, GetBrush(text.FontColor), GetBrush(text.BackgroundColor), GetFontSize(text.FontSize), (int)text.Horizontal, (int)text.Horizontal, (int)text.Vertical);
                        break;
                    case Component.ComponentTypeEnum.CLOCK:
                        element.ShowClock((int)compnent.ClockType, GetBrush(text.FontColor), GetBrush(text.BackgroundColor), GetFontSize(text.FontSize));
                        break;
                    case Component.ComponentTypeEnum.EXHIBITION_STAND:
                        element.ShowExhibition();
                        _exhibitions.TryAdd(id, element);
                        break;
                }
            }
        }
        private void SetLayout(BaseContent baseContent)
        {
            var isLocal = TryLoadLayout(out var local);
            SaveLayout(baseContent);
            _view.Dispatcher.Invoke(() =>
            {
                _view.Clear();
                var layouts = baseContent.Layout.Content.Pages;
                foreach (var layout in layouts)
                {
                    foreach (var compnent in layout.Components)
                    {
                        if (isLocal)
                        {
                            if (layout.Id == local.Layout.Id)
                            {
                                if (compnent.Equals(local.Layout.Content.Pages[0].Components.First(_ => _.Id == compnent.Id)))
                                {
                                    continue;
                                }
                            }
                        }
                        var id = compnent.Id;
                        var name = compnent.Name ?? $"Element{compnent.Id}";
                        var w = compnent.W;
                        var h = compnent.H;
                        var x = compnent.X;
                        var y = compnent.Y;
                        var z = compnent.Z;
                        var text = compnent.Text;
                        var element = _view.AddElement(name, w, h, x, y, z);
                        switch (compnent.ComponentType)
                        {
                            //case Component.ComponentTypeEnum.IMAGE:
                            //    element.ShowImage(text.Text);
                            //    break;
                            case Component.ComponentTypeEnum.BROWSER:
                                element.ShowWebBrowser(id,text.Text);
                                break;
                            case Component.ComponentTypeEnum.TEXT:
                                element.ShowText(id, text.Text, GetBrush(text.FontColor), GetBrush(text.BackgroundColor), GetFontSize(text.FontSize), (int)text.Horizontal, (int)text.Horizontal, (int)text.Vertical);
                                break;
                            //case Component.ComponentTypeEnum.VIDEO:
                            //    element.ShowVideo(text.Text);
                            //    break;
                            case Component.ComponentTypeEnum.CLOCK:
                                element.ShowClock((int)compnent.ClockType, GetBrush(text.FontColor), GetBrush(text.BackgroundColor), GetFontSize(text.FontSize));
                                break;
                            case Component.ComponentTypeEnum.EXHIBITION_STAND:
                                element.ShowExhibition();
                                _exhibitions.TryAdd(id, element);
                                break;
                        }
                    }
                }
                SetContent(baseContent.DefaultContents);
                SetContent(baseContent.NormalContents);
                SetContent(baseContent.EmergencyContents);
            });
        }
        private void SetContent(Content[]? contents)
        {
            if (contents != null)
            {
                foreach (var exhibition in _exhibitions)
                {
                    var element = exhibition.Value;
                    var list = contents
                        .Where(_=>_.Component.Id==exhibition.Key)
                        .OrderBy(_ => _.Order)
                        .ToArray();
                    foreach (var content in list)
                    {
                        var media = new ExhibitionControl.MediaContent
                        {
                            Name=content.Name,
                            StartAt = DateTime.Parse(content.StartedAt),
                            EndAt = DateTime.Parse(content.EndedAt),
                            Duration = content.PlayDuration,
                            Mute = content.Mute,
                            Id = content.Id,
                            MediaType = GetMediaType((int)content.Material.MaterialType),
                            Text = content.Text.Text,
                            FontSize = GetFontSize(content.Text.FontSize),
                            FontColor = GetBrush(content.Text.FontColor),
                            BackRound = GetBrush(content.Text.BackgroundColor),
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
                        element.AddMedia(media);
                    }
                }
            }
        }
        public static BaseContent FromJson(string json)
        {
            var baseContent = System.Text.Json.JsonSerializer.Deserialize<BaseContent>(json);
            return baseContent ?? new BaseContent();
        }
        private static void SaveLayout(BaseContent baseContent)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(baseContent);
            System.IO.File.WriteAllText("./save", json);
        }
        public static bool TryLoadLayout(out BaseContent content)
        {
            BaseContent result = null!;
            if (System.IO.File.Exists("./save"))
            {
                var json = System.IO.File.ReadAllText("./save");
                result = System.Text.Json.JsonSerializer.Deserialize<BaseContent>(json)!;
            }
            content = result ?? (new());
            return result != null;
        }
        private static Brush GetBrush(string colorString)
        {
            colorString = string.IsNullOrEmpty(colorString) ? "#FFFFFF" : colorString;
            var color = (System.Windows.Media.Color)ColorConverter.ConvertFromString(colorString);
            return new SolidColorBrush(color);
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
                5 => ExhibitionControl.MediaContent.MediaTypeEnum.Text,
                _ => ExhibitionControl.MediaContent.MediaTypeEnum.Text,
            };
        }
    }
}