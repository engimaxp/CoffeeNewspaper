using System.Collections.ObjectModel;

namespace CN_Presentation.ViewModel.Controls.Design
{
    public class MemoListControlDesignModel: MemoListControlViewModel
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design model
        /// </summary>
        public static MemoListControlDesignModel Instance => new MemoListControlDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public MemoListControlDesignModel()
        {
            Items = new ObservableCollection<MemoListItemViewModel>()
            {
                new MemoListItemViewModel(),
                new MemoListItemViewModel(),
                new MemoListItemViewModel(),
                new MemoListItemViewModel(),
            };
            ActivatedSearchTxts = new ObservableCollection<string>()
            {
                "Hello","My","您好","Boun"
            };
        }
        #endregion
    }

    public class MemoListItemDesignModel : MemoListItemViewModel
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design model
        /// </summary>
        public static MemoListItemDesignModel Instance => new MemoListItemDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public MemoListItemDesignModel()
        {
            MemoTitle = "ninninininin";
            MemoContent = "asdasdasdasdasdasda";
        }
        #endregion
    }
}