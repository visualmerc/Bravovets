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
                Display = "Innovation",
                Url = "/innovation"
            });
            menu.MenuItems.Add(new BravectoMenuItem
            {
                Code = "compliance",
                Display = "Compliance",
                Url = "/compliance"
            });
            menu.MenuItems.Add(new BravectoMenuItem
            {
                Code = "socialtips",
                Display = "Social Tips",
                Url = "/socialtips"
            });
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