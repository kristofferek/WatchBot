﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WatchBot.Models;

namespace WatchBot.Models.ViewModels
{
    public class DiscoverViewModel
    {

        public DiscoverViewModel()
        {
            Carousels = new List<CarouselViewModel>();
        }

        public List<CarouselViewModel> Carousels { get; set; }


    }
}