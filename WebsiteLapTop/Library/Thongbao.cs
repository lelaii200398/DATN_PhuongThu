using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebsiteLaptop
{
    public static class Thongbao
    {
        public static bool has_flash()
        {
            if (System.Web.HttpContext.Current.Session["Thongbao"].Equals(""))
            {
                return false;
            }
            return true;
        }
        public static void set_flash(String msg, String msg_type)
        {
            ModelThongbao thongbao = new ModelThongbao();
            thongbao.msg = msg;
            thongbao.msg_type = msg_type;
            System.Web.HttpContext.Current.Session["Thongbao"] = thongbao;
        }
        public static ModelThongbao get_flash()
        {
            ModelThongbao thongbao = (ModelThongbao)System.Web.HttpContext.Current.Session["Thongbao"];
            System.Web.HttpContext.Current.Session["Thongbao"] = "";
            return thongbao;
        }
    }
}