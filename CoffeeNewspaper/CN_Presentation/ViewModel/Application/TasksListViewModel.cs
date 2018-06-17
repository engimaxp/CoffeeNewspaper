﻿using CN_Presentation.ViewModel.Base;
using CN_Presentation.ViewModel.Controls;
using CN_Presentation.ViewModel.Controls.Design;

namespace CN_Presentation.ViewModel.Application
{
    public class TasksListViewModel : BaseViewModel
    {
        public RatingViewModel StarRatingViewModel { get; set; }

        public TasksListViewModel()
        {
            StarRatingViewModel = RatingDesignModel.Instance;
        }
    }
}
