﻿using DataService.Models;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler;

namespace WebCrawler
{
    class WebCrawler
    {

        static async Task Main(string[] args)
        {

            var ss = new CnyParser();
            //ss.ParserForeign("1101",null);
            //var s = new CnyParser();
            //await ss.RunAsync();
            await ss.ParserMarginAsync();
            //var ss =  s.ParseTrust("2330", "2017-01-01", "2019-10-29");

            Console.ReadLine();
        }
    }
}
