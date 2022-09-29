using System.Collections.Generic;

namespace ProjectEternity.Core.Units
{
    public struct DataGridValue
    {
        public List<string> ListColumnName;
        public List<int> ListColumnWidth;
        public List<RowWithSelectedValue> ListRow;
    }

    public struct RowWithSelectedValue
    {
        public string RowName;
        public List<CellValue> ListValues;

        public RowWithSelectedValue(string RowName, List<CellValue> ListValues)
        {
            this.RowName = RowName;
            this.ListValues = ListValues;
        }
    }

    public struct CellValue
    {
        public string SelectedValue;
        public List<string> ListValues;

        public CellValue(string SelectedValue, List<string> ListValues)
        {
            this.SelectedValue = SelectedValue;
            this.ListValues = ListValues;
        }
    }
}
