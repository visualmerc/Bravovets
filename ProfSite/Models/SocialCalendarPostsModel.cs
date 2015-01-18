using System.Collections.Generic;
using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;
using Newtonsoft.Json;

namespace ProfSite.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using LFW20.FrontEnd.PBAC;

    public class SocialCalendarPostModel
    {
        public int id { get; set; }

        public string title { get; set; }

        public string url { get; set; }

        [JsonProperty(PropertyName = "class")]
        public string cssclass { get; set; }

        public string network { get; set; }

        public string display_start { get; set; }

        public string start { get; set; }

        public string end { get; set; }

        public DateTime seedDate { get; set; }
    }

    public class SocialCalendarPostsModel
    {
        public int success { get; set; }
        public List<SocialCalendarPostModel> result { get; set; } 
    }
}

/*

{
	"success": 1,
 * 	"result": [
		{
			"id": "293",
			"title": "Lorem ipsum dolor sit amet, consectetur adipisicing elit. Dolores, deserunt!",
			"url": "http://www.example.com/",
			"class": "event-twitter",
			"network": "twitter",
			"display_start": "10:00",
			"start": "1397063217000",
			"end":   "1397063217000"
		},
		{
			"id": "294",
			"title": "Lorem ipsum dolor sit amet, consectetur adipisicing elit. Saepe, enim.",
			"url": "http://www.example.com/",
			"class": "event-facebook",
			"network": "facebook",
			"display_start": "11:00",
			"start": "1397063217000",
			"end":   "1397063217000"
		},
		{
			"id": "255",
			"title": "Lorem ipsum dolor sit amet, consectetur adipisicing elit. Odio, nesciunt.",
			"url": "http://www.example.com/",
			"class": "event-twitter",
			"network": "twitter",
			"display_start": "13:00",
			"start": "1397063217000",
			"end":   "1397063217000"
		},
		{
			"id": "256",
			"title": "Lorem ipsum dolor sit amet, consectetur adipisicing elit. Dolores, nulla!",
			"url": "http://www.example.com/",
			"class": "event-facebook",
			"network": "facebook",
			"display_start": "13:00",
			"start": "1392210000000",
			"end":   "1392210000000"
		},
		{
			"id": "257",
			"title": "Lorem ipsum dolor sit amet, consectetur adipisicing elit. Ratione, pariatur.",
			"url": "http://www.example.com/",
			"class": "event-twitter",
			"network": "twitter",
			"display_start": "15:00",
			"start": "1392382800000",
			"end":   "1392382800000"
		},
		{
			"id": "258",
			"title": "Lorem ipsum dolor sit amet, consectetur adipisicing elit. Amet, velit?",
			"url": "http://www.example.com/",
			"class": "event-twitter",
			"network": "twitter",
			"display_start": "15:00",
			"start": "1392969600000",
			"end":   "1392969600000"
		},
		{
			"id": "259",
			"title": "Lorem ipsum dolor sit amet, consectetur adipisicing elit. Ut, quibusdam.",
			"url": "http://www.example.com/",
			"class": "event-facebook",
			"network": "facebook",
			"display_start": "10:00",
			"start": "1392969600000",
			"end":   "1392969600000"
		},
		{
			"id": "276",
			"title": "Lorem ipsum dolor sit amet, consectetur adipisicing elit. Iste, cupiditate?",
			"url": "http://www.example.com/",
			"class": "event-facebook",
			"network": "facebook",
			"display_start": "12:00",
			"start": "1392634800000",
			"end":   "1392634800000"
		},
		{
			"id": "297",
			"title": "Lorem ipsum dolor sit amet, consectetur adipisicing elit. Reprehenderit, maxime.",
			"url": "http://www.example.com/",
			"class": "event-facebook",
			"network": "facebook",
			"display_start": "10:00",
			"start": "1393401600000",
			"end":   "1393401600000"
		},
		{
			"id": "295",
			"title": "Lorem ipsum dolor sit amet, consectetur adipisicing elit. Distinctio, accusantium.",
			"url": "http://www.example.com/",
			"class": "event-twitter",
			"network": "twitter",
			"display_start": "10:00",
			"start": "1393401600000",
			"end":   "1393401600000"
		}
	]
}




*/