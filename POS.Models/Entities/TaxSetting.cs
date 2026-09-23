namespace POS.Models.Entities
{
    public class TaxSetting
    {
        public int TaxSettingId { get; set; }

        public string TaxName { get; set; } = string.Empty;

        public decimal TaxPercentage { get; set; }

        public bool IsActive { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
