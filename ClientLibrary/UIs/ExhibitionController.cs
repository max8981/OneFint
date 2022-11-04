using ClientLibrary.Models;
using Microsoft.VisualBasic;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ClientLibrary.UIs
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
        private bool _stopPlay;
        private static bool _isDelayedUpdate;
        private static bool _isShowDownloader;
        public ExhibitionController(IExhibition exhibition)
        {
            _exhibition = exhibition;
            cancellationTokenSource = new();
        }
        public void SetExhibition()
        {
            Task.Factory.StartNew(() => AutoPlay(cancellationTokenSource.Token), cancellationTokenSource.Token);
        }
        public static void SetDelayedUpdate(bool b) => _isDelayedUpdate = b;
        public static void SetShowDownloader(bool b) => _isShowDownloader = b;
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
                {
                    _newFlashContents.Enqueue(item);
                }
                _stopPlay = true;
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
            _stopPlay = !_isDelayedUpdate;
        }
        public void RemoveNewFlashContent(int id)
        {
            if(nfc.Remove(id, out var payload))
            {
                _newFlashContents.Clear();
                foreach (var item in nfc.Values.OrderBy(_ => _.NewFlashContent?.Order))
                {
                    _newFlashContents.Enqueue(item);
                }
            }
        }
        private void AutoPlay(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                _stopPlay = false;
                Models.Content content;
                bool isEmpty;
                //SpinWait.SpinUntil(() => !isEmpty, 100);
                _exhibition.Hidden();
                if (isEmpty = !GetContent(_emergencyContents, out content!))
                    if (isEmpty = !GetNewFlashContent(_newFlashContents, out content!))
                        if (isEmpty = !GetContent(_normalContents, out content!))
                            if (isEmpty = !GetContent(_defaultContents, out content!))
                                continue;
                if (content != null&&content.Material!=null)
                {
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
                            _exhibition.ShowWeb(content.Id, content.Url);
                            break;
                        case Enums.MaterialTypeEnum.MATERIAL_TYPE_TEXT:
                            _exhibition.ShowText(content.Id, content.Text);
                            break;
                    }
                    _ = DateTime.TryParse(content.EndedAt, out var end);
                    SpinWait.SpinUntil(() => DateTime.Now > end || _stopPlay, content.PlayDuration * 1000);
                    _exhibition.Hidden(content.Id);
                }
            }
        }
        private void PlayAudio(Models.Content content)
        {
            if(content.Material!=null)
                if (MaterialDownload(content, out var material))
                    _exhibition.ShowAudio(content.Id, material);
        }
        private void PlayVideo(Models.Content content)
        {
            if (content.Material != null)
                if (MaterialDownload(content, out var material))
                    _exhibition.ShowVideo(content.Id, material, content.Mute);
        }
        private void PlayImage(Models.Content content)
        {
            if (content.Material != null)
                if (MaterialDownload(content, out var material))
                    _exhibition.ShowImage(content.Id, material);
        }
        private static bool GetContent(ConcurrentQueue<Models.Content> contents,out Models.Content? content)
        {
            bool result = false;
            if (contents.TryDequeue(out content))
            {
                _ = DateTime.TryParse(content.StartedAt, out var start);
                _ = DateTime.TryParse(content.EndedAt, out var end);
                if (DateTime.Now < end)
                {
                    contents.Enqueue(content);
                    if (DateTime.Now > start)
                    {
                        result = true;
                    }
                }
                else
                {
                    result = GetContent(contents, out content);
                }
            }
            return result;
        }
        private bool GetNewFlashContent(ConcurrentQueue<Models.NewFlashContentPayload> newFlashContents, out Models.Content? content)
        {
            var result = false;
            content = null;
            if (newFlashContents.TryDequeue(out var Payload))
            {
                content = Payload.NewFlashContent;
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
        private bool MaterialDownload(Content content ,out string materialPath)
        {
            var result = false;
            materialPath = "";
            if (content.Material != null)
            {
                var material = content.Material;
                var id = material.Id;
                var name = material.Name ?? "";
                var url = material.Content;
                var task = Downloader.GetOrAddTask(id, url!,content.Id, content.Device?.Id,content.DeviceGroup?.Id);
                materialPath = task.FileName;
                if (_isShowDownloader)
                    result = SpinWait.SpinUntil(() =>
                    {
                        if (!task.IsComplete)
                        {
                            var speed = Downloader.GetByteString(task.DownloadSpeed);
                            _exhibition.ShowDownload(id, name, $"{speed}/s", task.Progress);
                        }
                        else
                        {
                            //ClientController.MaterialDownloadStatus(new ClientToServer.MaterialDownloadStatus(content.Id, true, content.Device?.Id, content.DeviceGroup?.Id));
                        }
                        return task.IsComplete;
                    }, content.PlayDuration * 1000);
                else
                    result = task.IsComplete;
                _exhibition.Hidden(id);
            }
            return result;
        }
        internal void Close()
        {
            cancellationTokenSource.Cancel();
            _stopPlay = true;
        }
        ~ExhibitionController()
        {
            Close();
        }
    }
}
