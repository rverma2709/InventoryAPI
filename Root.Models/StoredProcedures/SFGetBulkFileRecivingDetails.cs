namespace Root.Models.StoredProcedures
{
    [StoredProcedureName("GetBulkFileRecivingDetails")]
    public class SFGetBulkFileRecivingDetails : SFParameters
    {
        public SFGetBulkFileRecivingDetails()
        {
            cols = "BulkFileRecivingDetailId";
            order = "DESC";
        }
        [QueryParam]
        public long? PoDetailId { get; set; }
        [QueryParam]
        public string?@PoItemDetilId { get; set; }
        
    }
}
