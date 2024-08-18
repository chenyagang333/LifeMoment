﻿using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouShowService.Domain.Entities;
using YouShowService.Domain.IRespository;

namespace YouShowService.Domain.DTO
{
    public record YouShowDTO : YouShow
    {
        // 下方是新增 前端需要展示的属性
        public bool LikeActive { get; set; }
        public bool StarActive { get; set; }
        public IEnumerable<YouShowFile>? Files { get; set; }

        public List<YouShowDTO> BuildList(IEnumerable<long> youshowIds)
        {
            List<YouShowDTO> youShowDTOs = new List<YouShowDTO>();
            youshowIds.ToList().ForEach(youshowId => youShowDTOs.Add(new() { Id = youshowId }));
            return youShowDTOs;
        }

        public YouShowDTO CheckLikeStar
            (IEnumerable<long> youShowLikeUsers, IEnumerable<long> youShowStarUsers)
        {
            if (youShowLikeUsers.Contains(this.Id))
            {
                LikeActive = true;
            }
            if (youShowStarUsers.Contains(this.Id))
            {
                StarActive = true;
            }
            return this;
        }
        public YouShowDTO UpdateFiles(IEnumerable<YouShowFile> youShowFiles,string baseUrl)
        {
            Files = youShowFiles.Where(x => x.YouShowId == Id).OrderBy(x => x.sort);
            foreach (var file in Files)
            {
                file.SpliceFirstURL(baseUrl).SpliceSecondURL(baseUrl); // 拼接文件地址
            }
            return this;
        }



    }
}
