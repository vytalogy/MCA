using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mca.model
{
    public class ProductUpdateDataModel
    {
        public int ID { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Defaultvalue { get; set; }
        public bool isYearly { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public int ModifyBy { get; set; }
        public string ModifyByName { get; set; }
        public List<SelectList> List { get; set; }
    }

    public class SelectList
    {
        public bool Selected { get; set; }

        public string Text { get; set; }

        public string Value { get; set; }
    }

    #region DB Model
    public class ForeCasting
    {
        public int ProductID { get; set; }
        public string MonthValue { get; set; }
        public string MonthName { get; set; }
        public DateTime MonthDate { get; set; }
        public bool Selected { get; set; }
    }

    #endregion
}
