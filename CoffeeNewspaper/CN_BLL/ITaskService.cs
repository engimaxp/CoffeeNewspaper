using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CN_Model;

namespace CN_BLL
{
    public interface ITaskService
    {
        int CreateATask(CNTask task);
    }
}
