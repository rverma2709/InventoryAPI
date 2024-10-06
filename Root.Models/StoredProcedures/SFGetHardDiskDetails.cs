namespace Root.Models.StoredProcedures
{
    [StoredProcedureName("GetHardDiskDetails")]
    public class SFGetHardDiskDetails : SFParameters
    {
        public SFGetHardDiskDetails()
        {
            cols = "HardDiskDetailId";
            order = "DESC";
        }
        [QueryParam]
        public long? DeviceTypeId { get; set; }
        [QueryParam]
        public string? HardDiskCompanyName { get; set; }
        [QueryParam]
        public string? HardDiskSize { get; set; }
        [QueryParam]
        public string? HardDiskType { get; set; }
    }
}
