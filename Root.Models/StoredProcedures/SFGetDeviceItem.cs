namespace Root.Models.StoredProcedures
{
    [StoredProcedureName("GetDeviceItem")]
    public class SFGetDeviceItem : SFParameters
    {
        public SFGetDeviceItem()
        {
            cols = "ReceivingDeviceDetailId";
            order = "DESC";
        }
        [QueryParam]
        public long? ReciverUserId { get; set; }
        [QueryParam]
        public long? PoDetailId { get; set; }
        [QueryParam]
        public long? PoItemDetilId { get; set; }
    }
    public class DeviceItems
    {
        public long? ReceivingDeviceDetailId { get; set; }
        public string? SerialNumber { get; set; }
        public string? PoNumber { get; set; }
        public string? DeviceDetails { get; set; }
    }
}
