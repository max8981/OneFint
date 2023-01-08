using Microsoft.VisualBasic;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using 屏幕管理.Interfaces;

namespace 屏幕管理.Controllers
{
    public class ExhibitionController
    {
        private readonly ConcurrentDictionary<int, Models.NewFlashContentPayload> nfc = new();
        private readonly ConcurrentQueue<Models.Content> _defaultContents = new();
        private readonly ConcurrentQueue<Models.Content> _normalContents = new();
        private readonly ConcurrentQueue<Models.NewFlashContentPayload> _newFlashContents = new();
        private readonly ConcurrentQueue<Models.Content> _emergencyContents = new();
        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly IExhibition _exhibition;
        private bool _pause;
        public ExhibitionController(IExhibition exhibition)
        {
            _exhibition = exhibition;
            cancellationTokenSource = new();
        }
        public void SetExhibition()
        {
            _ = Task.Factory.StartNew(() => AutoPlay(cancellationTokenSource.Token));
        }
        internal void Pause(bool pause) => _pause = pause;
        public void AddContent(Models.Content content)
        {
            switch (content.ContentType)
            {
                case Enums.ContentTypeEnum.NORMAL:
                    _normalContents.Enqueue(content);
                    break;
                case Enums.ContentTypeEnum.EMERGENCY:
                    _emergencyContents.Enqueue(content);
                    break;
                case Enums.ContentTypeEnum.NEWFLASH:
                    break;
                case Enums.ContentTypeEnum.DEFAULT:
                    _defaultContents.Enqueue(content);
                    break;
            }
        }
        public void AddNewFlashContent(Models.NewFlashContentPayload payload)
        {
            if (payload.NewFlashContent != null)
            {
                int id = payload.NewFlashContent.Id;
                nfc.AddOrUpdate(id, payload, (key, value) => value = payload);
                _newFlashContents.Clear();
                foreach (var item in nfc.Values.OrderBy(_=>_.NewFlashContent?.Order))
                    _newFlashContents.Enqueue(item);
            }
        }
        public void RemoveContent(Enums.ContentTypeEnum contentType)
        {
            switch (contentType)
            {
                case Enums.ContentTypeEnum.NORMAL:
                    _normalContents.Clear();
                    break;
                case Enums.ContentTypeEnum.EMERGENCY:
                    _emergencyContents.Clear();
                    break;
                case Enums.ContentTypeEnum.NEWFLASH:
                    break;
                case Enums.ContentTypeEnum.DEFAULT:
                    _defaultContents.Clear();
                    break;
                default:
                    _defaultContents.Clear();
                    _normalContents.Clear();
                    _emergencyContents.Clear();
                    break;
            }
        }
        public void RemoveNewFlashContent(int id)
        {
            if(nfc.Remove(id, out var payload))
            {
                _newFlashContents.Clear();
                foreach (var item in nfc.Values.OrderBy(_ => _.NewFlashContent?.Order))
                    _newFlashContents.Enqueue(item);
            }
        }
        public void SetWeb(string? url) => _exhibition.SetWeb(url);
        public void SetText(Models.BaseText text) => _exhibition.SetText(text);
        public void SetClock(Enums.ClockTypeEnum clockType, Models.BaseText text) => _exhibition.SetClock(clockType, text);
        private void AutoPlay(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                SpinWait.SpinUntil(() => !_pause);
                bool isEmpty;
                if (isEmpty=!GetContent(_emergencyContents, out Models.Content content))
                    if (isEmpty=!GetNewFlashContent(_newFlashContents, out content))
                        if (isEmpty = !GetContent(_normalContents, out content))
                            if (isEmpty = !GetContent(_defaultContents, out content))
                                Task.Delay(1000, token).Wait(token);
                if (!isEmpty && content.Material!=null)
                    switch (content.Material.MaterialType)
                    {
                        case Enums.MaterialTypeEnum.UNKNOWN_MATERIAL_TYPE:
                            break;
                        case Enums.MaterialTypeEnum.MATERIAL_TYPE_AUDIO:
                            PlayAudio(content);
                            break;
                        case Enums.MaterialTypeEnum.MATERIAL_TYPE_VIDEO:
                            PlayVideo(content);
                            break;
                        case Enums.MaterialTypeEnum.MATERIAL_TYPE_IMAGE:
                            PlayImage(content);
                            break;
                        case Enums.MaterialTypeEnum.MATERIAL_TYPE_WEB:
                            PlayWeb(content);
                            break;
                        case Enums.MaterialTypeEnum.MATERIAL_TYPE_TEXT:
                            PlayText(content);
                            break;
                    }
            }
        }
        private void PlayAudio(Models.Content content)
        {
            try
            {
                if (content.Material != null)
                    if (MaterialDownload(content, out var material))
                        _exhibition.ShowAudio(material, content.PlayDuration);
            }
            catch(Exception ex)
            {
                Systems.Log.Default.Error(ex, "PlayAudio", $"Parameter:{JsonSerializer.Serialize(content)}");
            }
        }
        private void PlayVideo(Models.Content content)
        {
            try
            {
                if (content.Material != null)
                    if (MaterialDownload(content, out var material))
                        _exhibition.ShowVideo(material, content.Mute, content.PlayDuration);
            }
            catch(Exception ex)
            {
                Systems.Log.Default.Error(ex, "PlayVideo", $"Parameter:{JsonSerializer.Serialize(content)}");
            }
        }
        private void PlayImage(Models.Content content)
        {
            try
            {
                if (content.Material != null)
                    if (MaterialDownload(content, out var material))
                        _exhibition.ShowImage(material, content.PlayDuration);
            }
            catch(Exception ex)
            {
                Systems.Log.Default.Error(ex, "PlayImage", $"Parameter:{JsonSerializer.Serialize(content)}");
            }
        }
        private void PlayWeb(Models.Content content)
        {
            try
            {
                var url = content.Url;
                if (url != null)
                    _exhibition.ShowWeb(url, content.PlayDuration);
            }
            catch (Exception ex)
            {
                Systems.Log.Default.Error(ex, "PlayWeb", $"Parameter:{JsonSerializer.Serialize(content)}");
            }
        }
        private void PlayText(Models.Content content)
        {
            try
            {
                var text = content.Text;
                if (text != null)
                    _exhibition.ShowText(text, content.PlayDuration);
            }
            catch (Exception ex)
            {
                Systems.Log.Default.Error(ex, "PlayText", $"Parameter:{JsonSerializer.Serialize(content)}");
            }
        }
        private static bool GetContent(ConcurrentQueue<Models.Content> contents,out Models.Content content)
        {
            bool result = false;
            if (contents.TryDequeue(out content!))
            {
                _ = DateTime.TryParse(content.StartedAt, out var start);
                _ = DateTime.TryParse(content.EndedAt, out var end);
                result = DateTime.Now > start;
                if (DateTime.Now < end) contents.Enqueue(content);
                else result = GetContent(contents, out content);
            }
            return result;
        }
        private bool GetNewFlashContent(ConcurrentQueue<Models.NewFlashContentPayload> newFlashContents, out Models.Content content)
        {
            var result = false;
            content = null!;
            if (newFlashContents.TryDequeue(out var Payload))
            {
                content = Payload.NewFlashContent!;
                if (content != null)
                {
                    _ = DateTime.TryParse(content.StartedAt, out var start);
                    _ = DateTime.TryParse(content.EndedAt, out var end);
                    if (DateTime.Now < end)
                    {
                        if (DateTime.Now > start)
                        {
                            if (Payload.LoopTime.HasValue && Payload.LoopTime > 0)
                            {
                                result = true;
                                Payload.LoopTime--;
                            } else if (DateTime.TryParse(Payload.EndAt, out var endtime))
                                result = DateTime.Now < endtime;
                            else
                            {
                                RemoveNewFlashContent(content.Id);
                                result = GetNewFlashContent(newFlashContents, out content);
                            }
                        }
                        newFlashContents.Enqueue(Payload);
                    }
                    else
                    {
                        result = GetNewFlashContent(newFlashContents, out content);
                    }
                }
            }
            return result;
        }
        private bool MaterialDownload(Models.Content content ,out string materialPath)
        {
            var result = true;
            materialPath = "";
            if (content.Material != null)
            {
                if (!DownloadController.TryGetMaterialFilePath(content.Material, out materialPath))
                {
                    var download = DownloadController.GetOrAddTask(content.Material.Id, content.Material.Content!, content.Id, content.Device?.Id, content.DeviceGroup?.Id);
                    result = _exhibition.ShowDownload(download.Name, download, content.PlayDuration);
                }
            }
            return result;
        }
        internal void Close()
        {
            cancellationTokenSource.Cancel();
            _pause = true;
        }
    }
}
