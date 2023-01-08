using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace 屏幕管理.Controllers
{
    internal partial class ClientController
    {
        private readonly Dictionary<System.Windows.Forms.Screen,Interfaces.ILayoutWindow> _screens;
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
                foreach (var newFlash in baseContent.NewFlashContentPayloads.OrderBy(_=>_.NewFlashContent?.Order))
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
                    Global.HiddenCode();
                    _exhibitions.Clear();
                    _layoutId = layout.Id;
                    foreach (var screen in _screens)
                    {
                        screen.Value.Clear();
                        screen.Value.ShowView();
                        if (layout.Width - screen.Key.Bounds.Width >= 0)
                            layout.Width -= screen.Key.Bounds.Width;
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
            var layoutIndex = GetPageIndex(ref component);
            var id = component.Id;
            var name = string.IsNullOrWhiteSpace(component.Name) ? $"Element{component.Id}" : component.Name;
            var w = component.W;
            var h = component.H;
            var x = component.X;
            var y = component.Y;
            var z = component.Z;
            var text = component.Text??new Models.BaseText();
            var rectangle = new System.Drawing.Rectangle(x, y, w, h);
            var exhibition = Layouts[layoutIndex].TryAddExhibition(id, name, rectangle, z);
            switch (component.ComponentType)
            {
                case Enums.ComponentTypeEnum.IMAGE:
                    break;
                case Enums.ComponentTypeEnum.BROWSER:
                    exhibition.SetWeb(text?.Text);
                    break;
                case Enums.ComponentTypeEnum.TEXT:
                    exhibition.SetText(text);
                    break;
                case Enums.ComponentTypeEnum.VIDEO:
                    break;
                case Enums.ComponentTypeEnum.CLOCK:
                    exhibition.SetClock(component.ClockType, text);
                    break;
                case Enums.ComponentTypeEnum.EXHIBITION_STAND:
                    exhibition.SetExhibition();
                    break;
            }
            _exhibitions.TryAdd(id, exhibition);
        }
        private int GetPageIndex(ref Models.Component component, int index = 0)
        {
            var result = index;
            if (Layouts.Length > result)
            {
                var size = Layouts[result].GetSize();
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
        public Interfaces.ILayoutWindow[] Layouts => _screens.Values.ToArray();
        public bool HasContent => _exhibitions.Count > 0;
        public void SetContents(Models.Content[]? contents)
        {
            if (contents != null)
            {
                var group = contents.GroupBy(_ => _.Component?.Id).ToArray();
                foreach (var item in group)
                    foreach (var content in item.OrderBy(_=>_.Order))
                        if (item.Key.HasValue)
                            _exhibitions[item.Key.Value].AddContent(content);
            }
        }
        internal void Close()
        {
            _timer.Close();
            foreach (var exhibition in _exhibitions.Values)
                exhibition.Close();
            foreach (var page in Layouts)
                page.Close();
            _exhibitions.Clear();
            _mqtt.Close();
        }
    }
}
