using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using Domain.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ahmadi.Controllers
{
    public class BaseController : Controller
    {
        private UnitOfWork.UnitOfWorkClass uow;
        public BaseController()
        {
            uow = new UnitOfWork.UnitOfWorkClass();
        }
      
        internal SettingDto  GetSetting(int? langid=null)
        {
            if (langid == null)
                langid = 1;
            SettingDto setting = null;
            if (Session["setting"+langid] == null)
            {
                var configuration = new MapperConfiguration(cfg =>
                {

                    cfg.CreateMap<Setting, SettingDto>()
                    .ForMember(dto => dto.attachmentFileName, conf => conf.MapFrom(ol => ol.attachment.FileName))
                    .ForMember(dto => dto.FaviconattachmentFileName, conf => conf.MapFrom(ol => ol.Faviconattachment.FileName))
                    .ForMember(dto => dto.WaterattachmentFileName, conf => conf.MapFrom(ol => ol.Waterattachment.FileName))
                    .ForMember(dto=>dto.WebSiteAdress,conf=>conf.MapFrom(ol=>ol.Address))
                    .ForMember(dto => dto.Address2, conf => conf.MapFrom(ol => ol.Address2))
                    .ForMember(dto => dto.WebSitePhoneNumber, conf => conf.MapFrom(ol => ol.Mobile));
                });
                setting = uow.SettingRepository.GetQueryList().AsNoTracking().Include(c => c.attachment).Include(c => c.Faviconattachment).Where(c => c.LanguageId == langid)
                    .ProjectTo<SettingDto>(configuration).FirstOrDefault();
                Session["setting" + langid] = setting;
            }
            else
            {
                setting = Session["setting" + langid] as SettingDto;
            }
            if (setting == null)
            {
                var configuration = new MapperConfiguration(cfg =>
                {

                    cfg.CreateMap<Setting, SettingDto>()
                    .ForMember(dto => dto.attachmentFileName, conf => conf.MapFrom(ol => ol.attachment.FileName))
                    .ForMember(dto => dto.FaviconattachmentFileName, conf => conf.MapFrom(ol => ol.Faviconattachment.FileName))
                    .ForMember(dto => dto.WaterattachmentFileName, conf => conf.MapFrom(ol => ol.Waterattachment.FileName));
                });
                setting = uow.SettingRepository.GetQueryList().AsNoTracking().Include(c => c.attachment).Include(c => c.Faviconattachment).Where(c => c.LanguageId == 1)
                    .ProjectTo<SettingDto>(configuration).FirstOrDefault();
                Session["setting" + langid] = setting;
            }
            return setting;
        }

    }
}