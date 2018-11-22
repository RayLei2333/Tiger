using System;
using System.Collections.Generic;
using System.Text;
using Tiger.ORM.ModelConfiguration;

namespace Tiger.Test.Model
{
    public class TB_Customer_Info
    {
        [Key(KeyType.GUID)]
        public string Id { get; set; }

        public string BrandId { get; set; }

        public string OpenId { get; set; }

        public string NickName { get; set; }

        public int Gender { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public string Province { get; set; }

        public string Language { get; set; }

        public string HeaderImg { get; set; }

        public DateTime SubscribeTime { get; set; }

        public DateTime? UnsubscribeTime { get; set; }

        public string UnionId { get; set; }

        public string Remark { get; set; }

        public string Groupid { get; set; }

        public string TagIdList { get; set; }

        public string SubscribeScene { get; set; }

        public string QRScene { get; set; }

        public string QRSceneStr { get; set; }

        public bool Subscribe { get; set; }

        public string RecruitingChannel { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }
    }
}
