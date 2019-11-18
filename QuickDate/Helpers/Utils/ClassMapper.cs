using System;
using AutoMapper;
using AutoMapper.Configuration;
using QuickDate.SQLite;
using QuickDateClient.Classes.Common;
using QuickDateClient.Classes.Global;

namespace QuickDate.Helpers.Utils
{
    public static class ClassMapper
    {
        public static void SetMappers()
        {
            try
            {
                var cfg = new MapperConfigurationExpression
                {
                    AllowNullCollections = true
                };

                cfg.CreateMap<GetOptionsObject.DataOptions, DataTables.SettingsTb>().ForMember(x => x.AutoId, opt => opt.Ignore());
                cfg.CreateMap<UserInfoObject, DataTables.InfoUsersTb>().ForMember(x => x.AutoId, opt => opt.Ignore());
                cfg.CreateMap<DataFile, DataTables.GiftsTb>().ForMember(x => x.AutoId, opt => opt.Ignore());
                cfg.CreateMap<DataFile, DataTables.StickersTb>().ForMember(x => x.AutoIdStickers, opt => opt.Ignore());
                cfg.CreateMap<UserInfoObject, DataTables.FavoriteUsersTb>().ForMember(x => x.AutoId, opt => opt.Ignore());
                
                Mapper.Initialize(cfg);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}