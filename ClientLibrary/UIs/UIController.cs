using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientLibrary.ServerToClient;

namespace ClientLibrary.UIs
{
    internal class UIController
    {
        private readonly IPageController _pageController;
        private readonly Dictionary<int, ExhibitionController> _exhibitions;
        public UIController(IPageController pageController)
        {
            _exhibitions = new();
            _pageController = pageController;
        }
        public void NormalAndDefaultContent(BaseContent baseContent)
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
        public void EmergencyContent(BaseContent baseContent)
        {
            foreach (var exhibition in _exhibitions.Values)
            {
                exhibition.RemoveContent(Enums.ContentTypeEnum.EMERGENCY);
            }
            SetLayout(baseContent.Layout);
            SetContents(baseContent.EmergencyContents);
        }
        public void NewFlashContent(BaseContent baseContent)
        {
            SetLayout(baseContent.Layout);
            if (baseContent.NewFlashContentPayloads != null)
                foreach (var newFlash in baseContent.NewFlashContentPayloads)
                    if (newFlash.NewFlashContent != null && newFlash.NewFlashContent.Component != null)
                        _exhibitions[newFlash.NewFlashContent.Component.Id].AddNewFlashContent(newFlash);
        }
        public void DeleteNewFlashContent(DeleteNewFlashContent deleteNewFlash)
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
            if(layout!=null&&layout.Content!=null&&layout.Content.Pages!=null)
                foreach (var page in layout.Content.Pages)
                    if (page.Components != null)
                        foreach (var component in page.Components)
                            AddComponent(component);
        }
        private void AddComponent(Models.Component component)
        {
            var id = component.Id;
            var name = component.Name ?? $"Element{component.Id}";
            var w = component.W;
            var h = component.H;
            var x = component.X;
            var y = component.Y;
            var z = component.Z;
            var text = component.Text;
            var rectangle = new System.Drawing.Rectangle(x, y, w, h);
            var exhibition = _pageController.TryAddExhibition(id, name, rectangle, z);
            var controller = new ExhibitionController(exhibition);
            switch (component.ComponentType)
            {
                case Enums.ComponentTypeEnum.IMAGE:
                    break;
                case Enums.ComponentTypeEnum.BROWSER:
                    exhibition.ShowWeb(component.Id, component.Text?.Text);
                    break;
                case Enums.ComponentTypeEnum.TEXT:
                    exhibition.ShowText(component.Id, component.Text);
                    break;
                case Enums.ComponentTypeEnum.VIDEO:
                    break;
                case Enums.ComponentTypeEnum.CLOCK:
                    exhibition.ShowClock(component.Id, component.Text, component.ClockType);
                    break;
                case Enums.ComponentTypeEnum.EXHIBITION_STAND:
                    controller.SetExhibition();
                    break;
            }
            _exhibitions.TryAdd(id, controller);
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
    }
}
