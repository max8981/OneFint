using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Client_VR.Models
{
    internal class GallerProperty
    {
        /// <summary>
        /// 
        /// </summary>
        public string? OriginAuthor { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int UpdateDate { get; set; }
        /// <summary>
        /// 山,2019年度大赛,地面,空中
        /// </summary>
        public string? Keywords { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Channel? Channel { get; set; }
        /// <summary>
        /// 龙川霍山景区 — 全景VR漫游
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// 霍山风景区位于龙川县内中部，是广东七大名山之一，是省级森林公园，是国家AAA级旅游区，以险峻的丹崖赤壁和奇岩秀石而早已闻名遐迩。相传，远古时代，这儿没有大山，后因当年的女娲补天时，将剩下的一点沙浆，撒落人间，刚好落到霍山，故使霍山呈现出悬崖高耸，绝壁万丈，横空屹立，怪石嶙峋、千姿百态，令人叹为观止的奇特山峰。
        /// </summary>
        public string? Remark { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? Pid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? ThumbUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int TemplateId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Selected { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int CreateDate { get; set; }
    }
}
