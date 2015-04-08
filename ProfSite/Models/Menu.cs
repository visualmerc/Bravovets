using ProfSite.Resources;
using System;
using System.Collections.Generic;
using System.EnterpriseServices.Internal;
using System.Linq;
using System.Web;

namespace ProfSite.Models
{

    public class BravectoMenuItem
    {
        public string Display { get; set; }
        public string Url { get; set; }
        public string Code { get; set; }
        public string CssClass { get; set; }
    }

    public class BravectoMenu
    {

        public static BravectoMenu CreateBravectoMenu(string activeItem)
        {
            var menu = new BravectoMenu { MenuItems = new List<BravectoMenuItem>(), ActiveItem = activeItem };
            menu.MenuItems.Add(new BravectoMenuItem
            {
                Code = "innovation",
                Display = Resource.BravectoInnovation,
                Url = "/bravecto/innovation"
            });
            return menu;
        }

        public static BravectoMenu CreateBravovetsMenu(string activeItem)
        {
            var menu = new BravectoMenu { MenuItems = new List<BravectoMenuItem>(), ActiveItem = activeItem };
            menu.MenuItems.Add(new BravectoMenuItem { Code = "trendingtopics", Display = Resource.TrendingTopicsMenu, Url = "/trendingtopics" });
            menu.MenuItems.Add(new BravectoMenuItem { Code = "socialtips", Display = Resource.SocialTipsMenu, Url = "/socialtips" });
            menu.MenuItems.Add(new BravectoMenuItem { CssClass = "bravovets-link", Code = "bravecto", Display = "Bravecto", Url = "/bravecto" });
            return menu;
        }

        public BravectoMenu()
        {
            Layout = "_Layout_Bravecto";
        }
        
        public string Layout { get; set; }
        public List<BravectoMenuItem> MenuItems { get; set; }
        public string ActiveItem { get; set; }
    }
}