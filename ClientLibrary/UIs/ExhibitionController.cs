using ClientLibrary.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ClientLibrary.UIs
{
    public class ExhibitionController
    {
        private readonly ConcurrentQueue<Models.Content> _defaultContents = new();
        private readonly ConcurrentQueue<Models.Content> _normalContents = new();
        private readonly ConcurrentQueue<Models.Content> _emergencyContents = new();
        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly IExhibition _exhibition;
        private bool _stopPlay;
        public ExhibitionController(IExhibition exhibition)
        {
            cancellationTokenSource = new();
            _exhibition = exhibition;
        }
        public void SetExhibition()
        {
            //AutoPlay(cancellationTokenSource.Token);
        }
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
            _stopPlay = true;
        }
        public void Play()
        {
            _stopPlay = false;
            Models.Content content;
            if (!GetContent(_emergencyContents, out content!))
                if (!GetContent(_normalContents, out content!))
                    if (!GetContent(_defaultContents, out content!))
                        return;
            if (content != null)
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
                        _exhibition.ShowText(content.Id,content.Text);
                        break;
                }
                _ = DateTime.TryParse(content.EndedAt, out var end);
                SpinWait.SpinUntil(() => DateTime.Now > end || _stopPlay, content.PlayDuration * 1000);
                _exhibition.Hidden(content.Id);
            }
            if (!_stopPlay)
                Play();
        }
        private void PlayAudio(Models.Content content)
        {
            if (MaterialDownload(content.Material, content.PlayDuration, out var material))
                _exhibition.ShowAudio(content.Id, material);
        }
        private void PlayVideo(Models.Content content)
        {
            if (MaterialDownload(content.Material, content.PlayDuration, out var material))
                _exhibition.ShowVideo(content.Id, material, content.Mute);
        }
        private void PlayImage(Models.Content content)
        {
            if (MaterialDownload(content.Material, content.PlayDuration, out var material))
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
        private bool MaterialDownload(Models.Material material,int timeOut ,out string materialPath)
        {
            var result = false;
            var id = material.Id;
            var name = material.Name;
            var url = material.Content;
            var task = Downloader.GetOrAddTask(id, url, name);
            materialPath = task.FileName;
            result = SpinWait.SpinUntil(() =>
            {
                if (!task.IsComplete)
                {
                    var d = task.DownloadSize;
                    var t = task.FileSize;
                    var s = Downloader.GetByteString(task.DownloadSpeed);
                    _exhibition.ShowDownload(id, name, $"{s}/s", d / t);
                }
                return task.IsComplete;
            }, timeOut * 1000);
            _exhibition.Hidden(id);
            return result;
        }
        ~ExhibitionController()
        {
            cancellationTokenSource.Cancel();
        }
    }
}
