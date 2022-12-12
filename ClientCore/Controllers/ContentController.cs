using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientCore.Controllers
{
    internal partial class ClientController
    {
        private readonly Dictionary<int, ExhibitionController> _exhibitions;
        private int _layoutId;
        private void NormalAndDefaultContent(ServerToClient.BaseContent baseContent)
        {
            foreach (var exhibition in _exhibitions.Values)
            {
                exhibition.RemoveContent(Enums.ContentTypeEnum.NORMAL);
                exhibition.RemoveContent(Enums.ContentTypeEnum.DEFAULT);
            }
            SetLayout(baseContent.Layout);
            SetContents(baseContent.DefaultContents);
            SetContents(baseContent.NormalContents);
        }
        private void EmergencyContent(ServerToClient.BaseContent baseContent)
        {
            foreach (var exhibition in _exhibitions.Values)
            {
                exhibition.RemoveContent(Enums.ContentTypeEnum.EMERGENCY);
            }
            SetLayout(baseContent.Layout);
            SetContents(baseContent.EmergencyContents);
        }
        private void NewFlashContent(ServerToClient.BaseContent baseContent)
        {
            SetLayout(baseContent.Layout);
            if (baseContent.NewFlashContentPayloads != null)
            {
                foreach (var newFlash in baseContent.NewFlashContentPayloads)
                    if (newFlash.NewFlashContent != null && newFlash.NewFlashContent.Component != null)
                        _exhibitions[newFlash.NewFlashContent.Component.Id].AddNewFlashContent(newFlash);
            }
        }
        private void DeleteNewFlashContent(ServerToClient.DeleteNewFlashContent deleteNewFlash)
        {
            if (deleteNewFlash.NewFlashContents != null)
                foreach (var content in deleteNewFlash.NewFlashContents)
                    if (content.Component != null)
                    {
                        var id = content.Id;
                        var componentId = content.Component.Id;
                        _exhibitions[componentId].RemoveNewFlashContent(id);
                    }
        }
        private void SetLayout(Models.Layout? layout)
        {
            if (layout != null)
                if (_layoutId != layout.Id)
                {
                    _exhibitions.Clear();
                    _layoutId = layout.Id;
                    foreach (var page in _layouts)
                        if (layout.Width - page.GetSize().Width > 0)
                        {
                            page.Clear();
                            page.ShowView();
                            layout.Width -= page.GetSize().Width;
                        }
                    if (layout != null && layout.Content != null && layout.Content.Pages != null)
                        foreach (var page in layout.Content.Pages)
                            if (page.Components != null)
                                foreach (var component in page.Components)
                                    AddComponent(component);
                }
        }
        private void AddComponent(Models.Component component)
        {
            var pageIndex = GetPageIndex(ref component);
            var id = component.Id;
            var name = component.Name ?? $"Element{component.Id}";
            var w = component.W;
            var h = component.H;
            var x = component.X;
            var y = component.Y;
            var z = component.Z;
            var text = component.Text??new Models.BaseText();
            var rectangle = new System.Drawing.Rectangle(x, y, w, h);
            var exhibition = _layouts[pageIndex].TryAddExhibition(id, name, rectangle, z);
            var controller = new ExhibitionController(exhibition);
            switch (component.ComponentType)
            {
                case Enums.ComponentTypeEnum.IMAGE:
                    break;
                case Enums.ComponentTypeEnum.BROWSER:
                    exhibition.ShowWeb(component.Text?.Text);
                    break;
                case Enums.ComponentTypeEnum.TEXT:
                    exhibition.ShowText(text.Text, text.FontColor, text.FontSize, text.BackgroundColor, (int)text.Horizontal, (int)text.Vertical);
                    break;
                case Enums.ComponentTypeEnum.VIDEO:
                    break;
                case Enums.ComponentTypeEnum.CLOCK:
                    exhibition.ShowClock((int)component.ClockType, text.FontColor, text.FontSize, text.BackgroundColor);
                    break;
                case Enums.ComponentTypeEnum.EXHIBITION_STAND:
                    controller.SetExhibition();
                    break;
            }
            _exhibitions.TryAdd(id, controller);
        }
        private int GetPageIndex(ref Models.Component component, int index = 0)
        {
            var result = index;
            if (_layouts.Length > result)
            {
                var size = _layouts[result].GetSize();
                if (component.X >= size.Width)
                {
                    component.X -= size.Width;
                    result++;
                    result = GetPageIndex(ref component, result);
                }
                if (component.Y >= size.Height)
                {
                    component.Y -= size.Height;
                    result++;
                    result = GetPageIndex(ref component, result);
                }
            }
            return result;
        }
        public void SetContents(Models.Content[]? contents)
        {
            if (contents != null)
            {
                var group = contents.GroupBy(_ => _.Component?.Id).ToArray();
                foreach (var item in group)
                    foreach (var content in item)
                        if (item.Key.HasValue)
                            _exhibitions[item.Key.Value].AddContent(content);
            }
        }
        public void StartAll()
        {
            foreach (var item in _exhibitions.Values)
                item.Start();
        }
        public void StopAll()
        {
            foreach (var item in _exhibitions)
                item.Value.Stop();
        }
        internal void Close()
        {
            foreach (var exhibition in _exhibitions.Values)
                exhibition.Close();
            foreach (var page in _layouts)
                page.Close();
            _exhibitions.Clear();
        }
    }
}
